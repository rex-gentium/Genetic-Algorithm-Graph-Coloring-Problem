using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Graph_Coloring_Problem
{
    public class GeneticSolver
    {
        Random random;

        int maxGenerations;
        int minPopulationSize, maxPopulationSize;
        float mutationRate;
        int reportFrequency;

        EdgeList graph;
        int[] allColors;
        int maxColors;
        int[] bestColoring;
        int bestBadEdgesCount = -1;

        List<Chromosome> population;
        List<Chromosome> parentPool;

        public GeneticSolver(int maxGenerations, int minPopulationSize, int maxPopulationSize,
            float mutationRate, EdgeList graph, int maxColors, int reportFrequency)
        {
            this.random = new Random();
            this.maxGenerations = maxGenerations;
            this.mutationRate = mutationRate;
            this.population = new List<Chromosome>(maxPopulationSize);
            this.parentPool = new List<Chromosome>();
            this.graph = graph;
            this.maxColors = maxColors;
            this.reportFrequency = reportFrequency;
            this.minPopulationSize = minPopulationSize;
            this.maxPopulationSize = maxPopulationSize;
            this.bestColoring = new int[graph.VerticeCount];
            this.allColors = new int[maxColors];
            for (int i = 0; i < maxColors; ++i)
                allColors[i] = i + 1;
        }

        public void Solve()
        {
            CreateFirstGeneration();
            int generation = 0;
            while (generation < maxGenerations)
            {
                KeepBestResult();
                if (bestBadEdgesCount == 0)
                    break;
                if (generation % reportFrequency == 0)
                {
                    Console.WriteLine("Generation " + generation.ToString());
                    Console.WriteLine(ToString() + "\n");
                }
                ProcessSelection();
                ProcessCrossover();
                ProcessMutation();                
                ++generation;
            }
            Console.WriteLine("Generation " + generation.ToString());
            Console.WriteLine(ToString() + "\n");
        }

        private void KeepBestResult()
        {   // запоминает лучшее решение поколения (если оно лучше решений предыдущего)
            foreach (Chromosome chromosome in population)
            {
                if (bestBadEdgesCount < 0 || chromosome.BadEdgesCount < bestBadEdgesCount)
                {
                    Array.Copy(chromosome.Colors, bestColoring, chromosome.Colors.Length);
                    bestBadEdgesCount = chromosome.BadEdgesCount;
                }
            }
        }

        private void CreateFirstGeneration()
        {   // генерирует изначальную популяцию со случайными решениями
            population.Clear();
            int initialPopulationSize = random.Next(minPopulationSize, maxPopulationSize);
            for (int i = 0; i < initialPopulationSize; ++i)
                AddRandomIndividual();
        }

        private void AddRandomIndividual()
        {
            int[] randomColoring = new int[graph.VerticeCount];
            for (int i = 0; i < randomColoring.Length; ++i)
                randomColoring[i] = allColors[random.Next(maxColors)];
            int badEdgesCount = graph.CountBadEdges(randomColoring);
            Chromosome individual = new Chromosome(randomColoring, badEdgesCount);
            population.Add(individual);
        }

        private void ProcessSelection()
        {   // производит отбор отсечением среди особей
            parentPool.Clear();
            // вся популяция ранжируется по значениям целевой функции
            population.Sort();
            // 20-50% лидеров попадают в пул родителей, остальные вымирают
            float survivorsPercent = random.Next(20, 50) / 100.0f;
            int poolSize = Convert.ToInt32(Math.Round(population.Count * survivorsPercent));
            parentPool.AddRange(population.Take(poolSize));
            population.Clear();
        }

        private void ProcessCrossover()
        {   // прозводит попарное скрещивание особей, прошедших отбор
            // на выходе популяция состоит из родителей и получившихся потомков
            int nextGenerationSize = random.Next(minPopulationSize, maxPopulationSize);
            int requiredChildrenCount = nextGenerationSize - parentPool.Count;
            while (requiredChildrenCount > 0 && parentPool.Count > 1)
            {
                Chromosome parent = parentPool[random.Next(parentPool.Count)];
                parentPool.Remove(parent);
                population.Add(parent);
                Chromosome partner = parentPool[random.Next(parentPool.Count)];
                parentPool.Remove(partner);
                population.Add(partner);
                Chromosome[] children = parent.BreedWith(partner, graph.CountBadEdges);
                requiredChildrenCount -= children.Length;
                foreach (Chromosome child in children)
                    population.Add(child);
            }
            // принудительное пополнение новыми особями, если популяция вымирает
            while (requiredChildrenCount-- > 0)   
                AddRandomIndividual();
        }

        private void ProcessMutation()
        {   // производит мутацию в некоторой доле популяции
            int mutantsCount = Convert.ToInt32(Math.Round(population.Count * mutationRate));
            for (int i = 0; i < mutantsCount; ++i)
            {
                int mutantIndex = random.Next(population.Count);
                Chromosome mutant = population.ElementAt(mutantIndex);
                int[] mutantColors = MutateColoring(mutant.Colors);
                int mutantBadEdges = graph.CountBadEdges(mutantColors);
                mutant.SetColoring(mutantColors, mutantBadEdges);
            }
        }

        private int[] MutateColoring(int[] colors)
        {   // мутирует раскраску в новую раскраску
            // старается изменять цвета "плохо" раскрашенных вершин 
            int[] mutantColors = colors;
            for (int i = 0; i < mutantColors.Length; ++i)
            {
                int v = i + 1;
                int currentColor = mutantColors[i];
                int[] adjacentColors = GetAdjacentColors(v, mutantColors);
                bool isSameColorAsAdjacent = adjacentColors.Contains(currentColor);
                if (isSameColorAsAdjacent)
                {
                    int[] validColors = allColors.Except(adjacentColors).ToArray();
                    if (validColors.Length == 0)
                        validColors = allColors;
                    int newColor = validColors[random.Next(validColors.Length)];
                    mutantColors[i] = newColor;
                }
            }
            return mutantColors;
        }

        private int[] GetAdjacentColors(int v, int[] colors)
        {   // возвращает массив цветов смежных для v вершин
            int[] adjacentVertices = graph.GetNeighbours(v);
            int[] result = new int[adjacentVertices.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                int adjV = adjacentVertices[i];
                result[i] = colors[adjV - 1];
            }
            return result.Distinct().ToArray();
        }

        public override string ToString()
        {
            string s = "Population size: " + population.Count.ToString() + " chromosomes\n";
            s += "Best coloring: ";
            if (bestBadEdgesCount < 0)
                s += "none";
            else
            {
                for (int i = 0; i < bestColoring.Length - 1; ++i)
                    s += bestColoring[i].ToString() + "-";
                s += bestColoring[bestColoring.Length - 1] + ", ";
                s += "bad edges count: " + bestBadEdgesCount.ToString();
            }
            return s;
        }
    }
}
