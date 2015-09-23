﻿using FactorioCalculator.Helper;
using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioCalculator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ModImporter a = new ModImporter(@"C:\Program Files\Factorio", "base");
            a.Load();

            //var assembler = a.Library.Buildings.Where((b) => b.Name == "assembling-machine-1").First();
            //var recipe = a.Library.Recipes.Where((r) => r.Name == "light-oil").First();
            //var step = new ProductionStep(null, new IStep[] { }, recipe, 1, assembler);

            var item = a.Library.Items.Where((i) => i.Name == "speed-module-3").First();
            var copperPlate = a.Library.Items.Where((i) => i.Name == "copper-plate").First();
            var ironPlate = a.Library.Items.Where((i) => i.Name == "iron-plate").First();
            var coal = a.Library.Items.Where((i) => i.Name == "coal").First();
            var oil = a.Library.Items.Where((i) => i.Name == "crude-oil").First();
            var water = a.Library.Items.Where((i) => i.Name == "water").First();
            var electronicCircuit = a.Library.Items.Where((i) => i.Name == "electronic-circuit").First();
            var science1 = a.Library.Items.Where((i) => i.Name == "science-pack-1").First();
            var science2 = a.Library.Items.Where((i) => i.Name == "science-pack-2").First();
            var science3 = a.Library.Items.Where((i) => i.Name == "science-pack-3").First();
            var speed3 = a.Library.Items.Where((i) => i.Name == "speed-module-3").First();
            var productivity3 = a.Library.Items.Where((i) => i.Name == "productivity-module-3").First();
            var efficiency3 = a.Library.Items.Where((i) => i.Name == "effectivity-module-3").First();
            var advancedCircuit = a.Library.Items.Where((i) => i.Name == "advanced-circuit").First();
            var alienArtifact = a.Library.Items.Where((i) => i.Name == "alien-artifact").First();
            var stone = a.Library.Items.Where((i) => i.Name == "stone").First();

            var results = new Dictionary<String, double> {
                {"science-pack-1", 0.3}, 
                {"science-pack-2", 0.3}, 
                {"science-pack-3", 0.3}, 
                {"alien-science-pack", 0.3}, 
                {"advanced-circuit", 1}, 
                {"lab", 1.0 / 60 / 15}, 
                {"assembling-machine-3", 1.0 / 60 / 15}, 
                {"electric-furnace", 1.0 / 60 / 15}, 
                {"basic-transport-belt-to-ground", 1.0 / 60 / 15}, 
                {"basic-splitter", 1.0 / 60 / 15}, 
                {"pipe", 1.0 / 60 / 15}, 
                {"pipe-to-ground", 1.0 / 60 / 15}, 
                {"chemical-plant", 1.0 / 60 / 15}, 
                {"oil-refinery", 1.0 / 60 / 15}, 
                {"long-handed-inserter", 1.0 / 60 / 15}, 
                {"basic-transport-belt", 0.25}, 
                {"fast-inserter", 1.0 / 60 / 15}, 
                {"basic-inserter", 1.0 / 60 / 15}, 
                {"medium-electric-pole", 1.0 / 60 / 15}, 
                {"steel-chest", 1.0 / 60 / 15}, 
                {"basic-mining-drill", 1.0 / 60 / 15},
                //{"advanced-circuit", 1.16388888888889},
            };

            
            var graph = RecipeGraph.FromLibrary(a.Library,
                new Item[] { copperPlate, ironPlate, coal, oil, alienArtifact, stone, water },
                results.Select((s) => new ItemAmount(a.Library.Items.Where((i) => i.Name == s.Key.ToLowerInvariant()).First(), s.Value)),
                (r) => 1);

            //graph.Children.PrintDot();

            var result = TrivialSolutionFactory.CreateFactory(graph);
            result.Children.PrintDot();

            //TrivialSolutionFactory.GenerateProductionLayer(a.Library, item, 2).PrintDot();

            //Console.WriteLine(a.Library.RecipeChains.Last().Value.First().Waste);


            //var solution = new TrivialSolutionFactory(a.Library, graph.Children);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
