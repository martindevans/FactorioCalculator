﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class FlowBuilding : FlowStep, IPhysicalBuilding
    {
        public Point Position { get; protected set; }
        public Building Building { get; protected set; }

        public FlowBuilding(ItemAmount item, Building building, Point position)
            : base(item)
        {
            Item = item;
            Building = building;
            Position = position;
        }
    }
}