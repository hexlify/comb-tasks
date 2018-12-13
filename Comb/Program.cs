using System;
using System.Collections.Generic;
using System.Linq;

namespace Comb
{
    public class Edge
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public int Weight { get; set; }

        public Edge(int vertex1, int vertex2, int weight)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("V1={0}; V2={1}; Weight={2}", Vertex1, Vertex2, Weight);
        }
    }

    public class Program
    {
        private const int LastElement = 32767;

        private static List<int> ReadInput()
        {
            var arr = new List<int>();
            IEnumerable<int> newLine;

            do
            {
                newLine = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                arr.AddRange(newLine);

            } while (newLine.Last() != LastElement);

            return arr;
        }

        private static void Merge(int v, int w, int p, int q, int[] name, int[] next, int[] size)
        {
            name[w] = p;
            var u = next[w];
            while (name[u] != p)
            {
                name[u] = p;
                u = next[u];
            }

            size[p] += size[q];

            var temp = next[w];
            next[w] = next[v];
            next[v] = temp;
        }

        private static List<Edge> KruskalAlgo(List<Edge> edges, int vertexCount)
        {
            var queue = new Queue<Edge>(edges);

            var name = new int[vertexCount];
            var next = new int[vertexCount];
            var size = new int[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                name[i] = i;
                next[i] = i;
                size[i] = 1;
            }

            var minSpanningTree = new List<Edge>();

            while (minSpanningTree.Count < vertexCount - 1)
            {
                var edge = queue.Dequeue();
                if (edge == null)
                    break;

                var p = name[edge.Vertex1];
                var q = name[edge.Vertex2];

                if (p != q)
                {
                    if (size[p] > size[q])
                        Merge(edge.Vertex1, edge.Vertex2, p, q, name, next, size);
                    else Merge(edge.Vertex2, edge.Vertex1, q, p, name, next, size);

                    minSpanningTree.Add(edge);
                }
            }

            return minSpanningTree;
        }

        public static List<int>[] GetAdjacencyList(List<Edge> edges, int vertexCount)
        {
            var result = new List<int>[vertexCount];
            for (var i = 0; i < vertexCount; i++)
                result[i] = new List<int>();

            foreach (var edge in edges)
            {
                result[edge.Vertex1].Add(edge.Vertex2);
                result[edge.Vertex2].Add(edge.Vertex1);
            }

            return result;
        }

        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var arr = ReadInput();

            var vertexCount = arr.FindIndex(i => i == n);
            var edges = new List<Edge>();

            for (var i = 0; i < vertexCount; i++)
                for (var j = arr[i] - 1; j < arr[i + 1] - 1; j += 2)
                    edges.Add(new Edge(i, arr[j] - 1, arr[j + 1]));

            edges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

            var minSpanningTree = KruskalAlgo(edges, vertexCount);
            var result = GetAdjacencyList(minSpanningTree, vertexCount);

            // output

            for (var i = 0; i < vertexCount; i++)
            {
                result[i].Sort();
                Console.WriteLine(string.Join(" ", result[i].Select(v => v + 1))+ " 0");
            }
            Console.WriteLine(minSpanningTree.Sum(e => e.Weight));
        }
    }
}
