using System;
using FactorioCalculator.Models;
using Microsoft.Z3;

namespace FactorioCalculator.Solver
{
    public class RecipeSolver
        : IDisposable
    {
        private readonly ItemAmount _target;
        private readonly Item[] _inputs;

        private readonly Context _context;

        private readonly Expr _iron;
        private readonly Expr _steel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">Target item amount to produce per second</param>
        /// <param name="inputs">All resources which are available in effectively infinite quantities (i.e. consider these additional raw resources)</param>
        public RecipeSolver(ItemAmount target, params Item[] inputs)
        {
            _target = target;
            _inputs = inputs;

            _context = new Context();

            #region setup context
            _iron = _context.MkConst("iron", _context.MkIntSort());
            _steel = _context.MkConst("steel", _context.MkIntSort());
            #endregion
        }

        public void Solve()
        {
            var solver = _context.MkSolver();

            solver.Assert(_context.MkImplies(_context.MkEq(_iron, _context.MkInt(3)), _context.MkEq(_steel, _context.MkInt(1))));
            solver.Assert(_context.MkEq(_iron, _context.MkReal(3)));
            solver.Assert(_context.MkEq(_steel, _context.MkReal(1)));

            var stat = solver.Check();

            foreach (var d in solver.Model.Decls)
                Console.WriteLine(d.Name + " -> " + solver.Model.ConstInterp(d));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
