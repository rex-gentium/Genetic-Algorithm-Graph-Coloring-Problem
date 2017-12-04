using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Graph_Coloring_Problem
{
    public class Chromosome : IComparable<Chromosome>
    {
        static Random random = new Random();

        int[] colors;
        int badEdgesCount;
        public int[] Colors
        {
            get
            {
                int[] res = new int[colors.Length];
                Array.Copy(colors, res, colors.Length);
                return res;
            }
        }
        public int BadEdgesCount { get => badEdgesCount; }
        
        public Chromosome(int[] colors, int badEdgesCount)
        {
            this.colors = new int[colors.Length];
            Array.Copy(colors, this.colors, colors.Length);
            this.badEdgesCount = badEdgesCount;
        }

        public void SetColoring(int[] colors, int badEdgesCount)
        {
            if (this.colors.Length != colors.Length)
                this.colors = new int[colors.Length];
            Array.Copy(colors, this.colors, colors.Length);
            this.badEdgesCount = badEdgesCount;
        }

        public Chromosome[] BreedWith(Chromosome other, Func<int[], int> badEdgesFunction)
        {
            List<Chromosome> children = new List<Chromosome>();
            int crossNode = random.Next(1, colors.Length);
            int[] child1Coloring = this.colors.Take(crossNode)
                .Concat(other.colors.Skip(crossNode))
                .ToArray();
            int[] child2Coloring = other.colors.Take(crossNode)
                .Concat(this.colors.Skip(crossNode))
                .ToArray();
            children.Add(new Chromosome(child1Coloring, badEdgesFunction.Invoke(child1Coloring)));
            children.Add(new Chromosome(child2Coloring, badEdgesFunction.Invoke(child2Coloring)));
            return children.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Chromosome other = obj as Chromosome;
            if (other == null) return false;
            return other.colors.SequenceEqual(this.colors) && other.badEdgesCount.Equals(this.badEdgesCount);
        }

        public override string ToString()
        {
            string s = "Coloring: ";
            if (colors == null)
                s += "none";
            else
            {
                for (int i = 0; i < colors.Length - 1; ++i)
                    s += colors[i].ToString() + "-";
                s += colors[colors.Length - 1] + ", bad edges count: ";
                s += badEdgesCount.ToString();
            }
            return s;
        }

        public int CompareTo(Chromosome other)
        {
            return this.badEdgesCount.CompareTo(other.badEdgesCount);
        }

        public override int GetHashCode()
        {
            var hashCode = -974168056;
            hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(colors);
            hashCode = hashCode * -1521134295 + badEdgesCount.GetHashCode();
            return hashCode;
        }

    }
}
