using System;
using System.IO;
using DefaultNamespace;

namespace bfs_parallel
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = args.Length != 0 ? args[0] : "0.edges";
            var graph = new MergeList(Path.Combine(Environment.CurrentDirectory, filename));
            Console.WriteLine("start serial");
            Console.WriteLine(graph.Serial(4000));
            Console.WriteLine("start leveled");
            Console.WriteLine("end leveled");
            var res = graph.LeveledCheck(4000);
            Console.WriteLine(res.Count);
        }
    }
}