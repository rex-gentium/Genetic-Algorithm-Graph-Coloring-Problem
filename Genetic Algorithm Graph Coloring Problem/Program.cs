using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Graph_Coloring_Problem
{
    class Program
    {
        static void Main(string[] args)
        {
            int colorsCount = 0;
            EdgeList graph = LoadGraph("d:\\dump\\graph2colors.txt", ref colorsCount);
            Console.WriteLine("Loaded graph:\n");
            Console.WriteLine(graph.ToString());

            Random rand = new Random();
            int maxGenerations = 100;
            int minPopulationSize = 10, maxPopulationSize = 30;
            float mutationRate = rand.Next(5, 10) / 100.0f;
            int reportEvery = 1;

            GeneticSolver solver = new GeneticSolver(maxGenerations, minPopulationSize,
                maxPopulationSize, mutationRate, graph, colorsCount, reportEvery);
            solver.Solve();

            Console.ReadLine();
        }

        private static EdgeList LoadGraph(string filePath, ref int colorsCount)
        {
            Random rand = new Random(0);
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = sr.ReadLine();
                string[] parameters = line.Split(' ');
                int vCount = Int32.Parse(parameters[0]);
                colorsCount = Int32.Parse(parameters[1]);
                EdgeList result = new EdgeList(vCount);
                while ((line = sr.ReadLine()) != null)
                {
                    parameters = line.Split('-');
                    int from = Int32.Parse(parameters[0]);
                    int to = Int32.Parse(parameters[1]);
                    int weight = rand.Next(1, 10);
                    result.AddEdge(new Edge(from, to, weight));
                }
                return result;
            }
        }
    }
}
