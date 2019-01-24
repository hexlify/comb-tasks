using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Task2
{
    class Program
    {
        private const string InputFile = "input.txt";
        private const string OutputFile = "output.txt";

        private static int[,] ReadInput(out int vertexCount, out int start, out int destination)
        {
            using (var file = new StreamReader(InputFile))
            {
                vertexCount = int.Parse(file.ReadLine());
                var capacityConstraints = new int[vertexCount, vertexCount];
                int[] newValues;

                for (var i = 0; i < vertexCount; i++)
                {
                    newValues = file.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    for (var j = 0; j < vertexCount; j++)
                        capacityConstraints[i, j] = newValues[j];
                }

                start = int.Parse(file.ReadLine());
                destination = int.Parse(file.ReadLine());
                return capacityConstraints;
            }
        }

        private static int[,] ReadConsoleInput(out int vertexCount)
        {
            vertexCount = int.Parse(Console.ReadLine());
            var capacityConstraints = new int[vertexCount, vertexCount];
            int[] newValues;

            for (var i = 0; i < vertexCount; i++)
            {
                newValues = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                for (var j = 0; j < vertexCount; j++)
                    capacityConstraints[i, j] = newValues[j];
            }

            return capacityConstraints;
        }

        private static Tuple<int[], int[], int[]> Labeling(int s, int t, int[,] A, int[,] F, int n)
        {
            var h = Enumerable.Repeat(int.MaxValue, n).ToArray();
            var prev = Enumerable.Repeat(-1, n).ToArray();
            var choice = Enumerable.Repeat(0, n).ToArray();
            var queue = new Queue<int>();
            queue.Enqueue(s - 1);

            while (h[t - 1] == int.MaxValue && queue.Count != 0)
            {
                var w = queue.Dequeue();
                for (var v = 0; v < n; v++)
                {
                    if (h[v] == int.MaxValue && A[w, v] - F[w, v] > 0)
                    {
                        h[v] = Math.Min(h[w], A[w, v] - F[w, v]);
                        prev[v] = w;
                        queue.Enqueue(v);
                        choice[v] = 1;
                    }
                }
                for (var v = 0; v < n; v++)
                {
                    if (v == s - 1)
                        continue;
                    if (h[v] == int.MaxValue && A[v, w] > 0)
                    {
                        h[v] = Math.Min(h[w], F[v, w]);
                        prev[v] = w;
                        queue.Enqueue(v);
                        choice[v] = -1;
                    }
                }
            }
            return Tuple.Create(h, prev, choice);
        }

        private static int[,] FordFulkersonAlgo(int s, int t, int[,] A, int n, out int flowSize)
        {
            var F = new int[n, n];
            flowSize = 0;

            int[] h;
            do
            {
                var res = Labeling(s, t, A, F, n);
                h = res.Item1;
                var prev = res.Item2;
                var choice = res.Item3;

                if (h[t - 1] < int.MaxValue)
                {
                    flowSize += h[t - 1];
                    var v = t - 1;
                    while (v != s - 1)
                    {
                        var w = prev[v];
                        if (choice[v] == 1)
                            F[w, v] = F[w, v] + h[t - 1];
                        else
                            F[v, w] = F[v, w] - h[t - 1];
                        v = w;
                    }
                }
            } while (h[t - 1] < int.MaxValue);

            return F;
        }

        static void Main(string[] args)
        {
            int vertexCount;
            int start;
            int destination;
            var capacityConstraints = ReadInput(out vertexCount, out start, out destination);
            int flowSize;

            var flow = FordFulkersonAlgo(start, destination, capacityConstraints, vertexCount, out flowSize);

            using (var file = new StreamWriter(OutputFile))
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    var line = new List<int>();
                    for (var j = 0; j < vertexCount; j++)
                        line.Add(flow[i, j]);

                    file.WriteLine(string.Join(" ", line));
                }

                file.WriteLine(flowSize);
            }
        }

        static void Main1(string[] args)
        {
            int vertexCount;
            var capacityConstraints = ReadConsoleInput(out vertexCount);
            int flowSize;

            var flow = FordFulkersonAlgo(1, vertexCount, capacityConstraints, vertexCount, out flowSize);
            Console.WriteLine(flowSize);
        }
    }
}