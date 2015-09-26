﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Helper;

namespace FactorioCalculator.Models.Factory
{
    public class FlowBuilding : FlowStep, IPhysicalBuilding
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Size { get; protected set; }
        public BuildingRotation Rotation { get; protected set; }
        public Building Building { get; protected set; }

        public FlowBuilding(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item)
        {
            Item = item;
            Building = building;
            Position = position;
            Size = building.Size.RotateAbsolute(rotation);
            Rotation = rotation;
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>", Building.Name, Item);
        }
    }
}
