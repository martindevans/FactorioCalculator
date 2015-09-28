﻿using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    struct SolidRouteState : IRouteState
    {
        public double Cost { get { return _cost; } }
        private double _cost;

        public Vector2 Position { get { return _position; } }
        private Vector2 _position;

        public SearchSpace Space { get { return _space; } }
        private SearchSpace _space;

        public Depth Depth { get { return _depth; } }
        private Depth _depth;

        public BuildingRotation Direction { get { return _direction; } }
        private BuildingRotation _direction;

        public IPhysicalBuilding Building { get { return _building; } }
        private IPhysicalBuilding _building;
 
        public int UndergroundLength { get { return _undergroundLength; } }
        private int _undergroundLength;

        public FlowBuilding FlowBuilding { get { return _building as FlowBuilding; } }

        public RoutingCoord.CoordType TransportState { get { return _transportState; } }
        private RoutingCoord.CoordType _transportState;

        public RoutingCoord RoutingCoord { get { return new RoutingCoord(_position, _transportState, _direction); } }

        public SolidRouteState(IPhysicalBuilding building, double cost, Vector2 position, SearchSpace space, RoutingCoord.CoordType transportState, Depth depth = Depth.None, BuildingRotation direction = BuildingRotation.North, int undergroundLength = 0)
        {
            _building = building;
            _cost = cost;
            _position = position;
            _space = space;
            _depth = depth;
            _direction = direction;
            _undergroundLength = undergroundLength;
            _transportState = transportState;
        }

        public IEnumerable<SolidRouteState> NextStates(Func<SearchSpace, IPhysicalBuilding, double> costFunction,
            Building belt, Building beltGroundNormal, Building beltGroundFast,
            Building beltGroundExpress, Building inserter, Building longInserter,
            Building fastInserter)
        {
            switch (_transportState)
            {
                case RoutingCoord.CoordType.Belt:
                    return GenerateBeltStates(costFunction, belt)
                        .Concat(GeneratePlacedStates(costFunction))
                        .Concat(GenerateBeltToGround(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress));
                case RoutingCoord.CoordType.Inserter:
                    return GeneratePlacedStates(costFunction)
                        .Concat(GenerateBeltStates(costFunction, belt))
                        .Concat(GenerateBeltToGround(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress));
                case RoutingCoord.CoordType.PlacedItem:
                    return GenerateInserters(costFunction, inserter, longInserter, fastInserter);
                case RoutingCoord.CoordType.Underflow:
                    return GenerateUnderFlow(costFunction)
                        .Concat(GenerateGroundToBelt(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress));
            }

            return new SolidRouteState[] { };
        }

        private IEnumerable<SolidRouteState> GeneratePlacedStates(Func<SearchSpace, IPhysicalBuilding, double> costFunction)
        {
            var placedBuilding = new FlowBuilding(((FlowBuilding)_building).Item, new Building("placed-item"), _position, BuildingRotation.North);
            placedBuilding.Previous.Add(Building);
            yield return new SolidRouteState(placedBuilding, _cost + costFunction(_space, placedBuilding), _position, _space.AddRoute(placedBuilding), RoutingCoord.CoordType.PlacedItem);
        }

        private IEnumerable<SolidRouteState> GenerateBeltStates(Func<SearchSpace, IPhysicalBuilding, double> costFunction, Building belt)
        {
            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
            Vector2 nextpos = _position + (_transportState == RoutingCoord.CoordType.Belt ? _direction.ToVector() : Vector2.Zero);
            foreach (var rotation in rotations)
            {
                var building = new FlowBuilding(((FlowBuilding)_building).Item, belt, nextpos, rotation);
                building.Previous.Add(Building);
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), nextpos, _space.AddRoute(building), RoutingCoord.CoordType.Belt, Depth.None, rotation);
            }
        }

        private IEnumerable<SolidRouteState> GenerateBeltToGround(Func<SearchSpace, IPhysicalBuilding, double> costFunction, Building groundNormal, Building groundFast, Building groundExpress)
        {
            Vector2 nextpos = _position + (_transportState == RoutingCoord.CoordType.Belt ? _direction.ToVector() : Vector2.Zero);
            var buildingNormal = new GroundToUnderground(((FlowBuilding)_building).Item, groundNormal, nextpos, _direction, Depth.Normal);
            var buildingFast = new GroundToUnderground(((FlowBuilding)_building).Item, groundFast, nextpos, _direction, Depth.Fast);
            var buildingExpress = new GroundToUnderground(((FlowBuilding)_building).Item, groundExpress, nextpos, _direction, Depth.Express);
            buildingNormal.Previous.Add(Building);
            buildingFast.Previous.Add(Building);
            buildingExpress.Previous.Add(Building);
            yield return new SolidRouteState(buildingNormal, _cost + costFunction(_space, buildingNormal), nextpos, _space.AddRoute(buildingNormal), RoutingCoord.CoordType.Underflow, Depth.Normal, _direction);
            yield return new SolidRouteState(buildingFast, _cost + costFunction(_space, buildingFast), nextpos, _space.AddRoute(buildingFast), RoutingCoord.CoordType.Underflow, Depth.Fast, _direction);
            yield return new SolidRouteState(buildingExpress, _cost + costFunction(_space, buildingExpress), nextpos, _space.AddRoute(buildingExpress), RoutingCoord.CoordType.Underflow, Depth.Express, _direction);
        }

        private IEnumerable<SolidRouteState> GenerateUnderFlow(Func<SearchSpace, IPhysicalBuilding, double> costFunction)
        {
            Vector2 nextpos = _position + _direction.ToVector();
            var building = new UndergroundFlow(((FlowBuilding)_building).Item, nextpos, _depth, _direction);
            building.Previous.Add(Building);
            if (_undergroundLength < 4)
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), nextpos, _space.AddRoute(building), RoutingCoord.CoordType.Underflow, _depth, _direction, _undergroundLength + 1);
        }

        private IEnumerable<SolidRouteState> GenerateGroundToBelt(Func<SearchSpace, IPhysicalBuilding, double> costFunction, Building groundNormal, Building groundFast, Building groundExpress)
        {
            Vector2 nextpos = _position + _direction.ToVector();
            var buildingNormal = new GroundToUnderground(((FlowBuilding)_building).Item, groundNormal, nextpos, _direction, Depth.Normal);
            var buildingFast = new GroundToUnderground(((FlowBuilding)_building).Item, groundFast, nextpos, _direction, Depth.Fast);
            var buildingExpress = new GroundToUnderground(((FlowBuilding)_building).Item, groundExpress, nextpos, _direction, Depth.Express);
            buildingNormal.Previous.Add(Building);
            buildingFast.Previous.Add(Building);
            buildingExpress.Previous.Add(Building);
            
            if (_depth == Depth.Normal)
                yield return new SolidRouteState(buildingNormal, _cost + costFunction(_space, buildingNormal), nextpos, _space.AddRoute(buildingNormal), RoutingCoord.CoordType.Belt, Depth.None, _direction);
            if (_depth == Depth.Fast)
                yield return new SolidRouteState(buildingFast, _cost + costFunction(_space, buildingFast), nextpos, _space.AddRoute(buildingFast), RoutingCoord.CoordType.Belt, Depth.None, _direction);
            if (_depth == Depth.Express)
                yield return new SolidRouteState(buildingExpress, _cost + costFunction(_space, buildingExpress), nextpos, _space.AddRoute(buildingExpress), RoutingCoord.CoordType.Belt, Depth.None, _direction);
        }

        private IEnumerable<SolidRouteState> GenerateInserters(Func<SearchSpace, IPhysicalBuilding, double> costFunction, Building inserter, Building longInserter, Building fastInserter)
        {
            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };

            foreach (var rotation in rotations)
            {
                Vector2 nextpos = _position;
                var buildingInserter = new FlowBuilding(((FlowBuilding)_building).Item, inserter, nextpos + rotation.ToVector(), rotation);
                var buildingLongInserter = new FlowBuilding(((FlowBuilding)_building).Item, longInserter, nextpos + 2 * rotation.ToVector(), rotation);
                var buildingFastInserter = new FlowBuilding(((FlowBuilding)_building).Item, fastInserter, nextpos + rotation.ToVector(), rotation);

                buildingInserter.Previous.Add(Building);
                buildingLongInserter.Previous.Add(Building);
                buildingFastInserter.Previous.Add(Building);

                yield return new SolidRouteState(buildingInserter, _cost + costFunction(_space, buildingInserter), nextpos + 2 * rotation.ToVector(), _space.AddRoute(buildingInserter), RoutingCoord.CoordType.Inserter, Depth.None, BuildingRotation.North);
                yield return new SolidRouteState(buildingLongInserter, _cost + costFunction(_space, buildingLongInserter), nextpos + 4 * rotation.ToVector(), _space.AddRoute(buildingLongInserter), RoutingCoord.CoordType.Inserter, Depth.None, BuildingRotation.North);
                yield return new SolidRouteState(buildingFastInserter, _cost + costFunction(_space, buildingFastInserter), nextpos + 2 * rotation.ToVector(), _space.AddRoute(buildingFastInserter), RoutingCoord.CoordType.Inserter, Depth.None, BuildingRotation.North);
            }
        }
    }
}