using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Edge
{
    public int From { set; get;}
    public int To { set; get; }
    public int Weight { set; get; }

    public Edge(int from, int to, int weight)
    {
        this.From = from;
        this.To = to;
        this.Weight = weight;
    }

}

public class EdgeList
{
    private int verticeCount;
    public int VerticeCount { get => verticeCount; }
    public int EdgeCount { get => edges.Count; }
    private List<Edge> edges;
    private Random rand = new Random();

    public EdgeList(int numVertices, int numEdges = 10)
    {
        this.edges = new List<Edge>(numEdges);
        this.verticeCount = numVertices;
    }

    public bool HasEdge(int from, int to)
        => GetEdge(from, to) != null;

    public bool AddEdge(Edge edge)
    {
        if (HasEdge(edge.From, edge.To)) return false;
        edges.Add(edge);
        return true;
    }

    public Edge GetEdge(int from, int to)
    {
        foreach (Edge edge in edges)
            if (edge.From == from && edge.To == to
                || edge.From == to && edge.To == from)
                return edge;
        return null;
    }

    public Int32 GetWeight(int from, int to)
    {
        Edge edge = GetEdge(from, to);
        return (edge == null) ? -1 : edge.Weight;
    }

    public int[] GetNeighbours(int vertex)
    {
        HashSet<int> result = new HashSet<int>();
        foreach (Edge edge in edges)
        {
            if (edge.From == vertex) result.Add(edge.To);
            if (edge.To == vertex) result.Add(edge.From);
        }
        return result.ToArray();
    }

    public int CountBadEdges(int[] coloring)
    {
        int result = 0;
        foreach (Edge edge in edges)
        {
            if (coloring[edge.From - 1] == coloring[edge.To - 1])
                ++result;
        }
        return result;
    }

    public override string ToString()
    {
        string s = VerticeCount.ToString() + " vertices, "
            + EdgeCount.ToString() + " edges:\n";
        foreach (Edge edge in edges)
            s += edge.From.ToString() + " - " 
                + edge.To.ToString() 
                + ", weighted " + edge.Weight.ToString() 
                + "\n";
        return s;
    }
}

