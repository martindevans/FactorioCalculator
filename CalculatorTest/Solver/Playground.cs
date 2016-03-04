using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Models;
using FactorioCalculator.Solver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorTest.Solver
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void MethodName()
        {
            RecipeSolver r = new RecipeSolver(new ItemAmount(new Item("plate"), 1));
            r.Solve();
        }
    }
}
