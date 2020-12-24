using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DefaultNamespace;

namespace bfs_parallel
{
    class Program
    {

        public static void Log(string logMessage, TextWriter rw)
        {
            rw.Write("\r\nLog Entry : ");
            rw.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            rw.WriteLine("  :");
            rw.WriteLine($"  :{logMessage}");
            rw.WriteLine ("-------------------------------");
        }
        static void Info()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var info =
                "\t Данный программный продукт реализован в рамках выполнения КП по дисциплине \"Параллельное программирование\"\n" +
                "\t Тема: \"Учебно-демонстрационная программа по параллельному поиску всех путей в графе\"\n" +
                "\t Выполнил студент группы ДИПРБ-41 Самарский В.В. 2020г.";
            Console.WriteLine(info);
            Console.ForegroundColor = ConsoleColor.Black;
        }


        static List<int> StandMeasure(Func<int,List<int>> method, int start_point, string name = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (name != null)
                Console.WriteLine($"\t{name}");
            Console.ForegroundColor = ConsoleColor.Green;

            var timer = new Stopwatch();
            timer.Start();
            var res = method(start_point);
            timer.Stop();
            using (StreamWriter rw = File.AppendText("log.txt"))
            {
                var logmessage = name != null ? $"{name}\n" : "";
                logmessage += $"{timer.Elapsed}";
                Log(logmessage, rw);    
            }
            Console.WriteLine($"{timer.Elapsed:g}");
            return res;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="start_point"></param>
        /// <param name="name"></param>
        /// <returns> timer for futher comparison</returns>
        static Stopwatch StandMeasure(Action<int> method, int start_point, string name = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (name != null)
                Console.WriteLine($"\t{name}");
            Console.ForegroundColor = ConsoleColor.Green;
            
            var timer = new Stopwatch();
            timer.Start();
            method(start_point);
            timer.Stop();


            using (StreamWriter rw = File.AppendText("log.txt"))
            {
                var logmessage = name != null ? $"{name}\n" : "";
                logmessage += $"{timer.Elapsed}";
                Log(logmessage, rw);    
            }
            Console.WriteLine($"{timer.Elapsed:g}");
            return timer;
        }
        
        
        static TimeSpan StandMeasureTimeDiffer(Action<int> action1, Action<int> action2, int start_point)
        {
            var timer = new Stopwatch();
            timer.Start();
            action1(start_point);
            timer.Stop();
            
            var timer2 = new Stopwatch();
            timer2.Start();
            action2(start_point);
            timer2.Stop();

            Console.WriteLine(timer.Elapsed);
            Console.WriteLine(timer2.Elapsed);
            Console.WriteLine($"{timer.Elapsed - timer2.Elapsed}");
            Console.WriteLine("______");
            return timer.Elapsed - timer2.Elapsed;

        }
        static void Main(string[] args)
        {
            Info();

            string filename;
            int start_point;
            try
            {
                filename = args.Length != 0 ? args[0] : "0.edges";
                start_point = args.Length >= 1 ? Convert.ToInt32(args[1]) : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("give correct args [0] - filename [1]- correct index");
                using (StreamWriter rw = File.AppendText("log.txt"))
                {
                    var logmessage = "Error occured\n";
                    logmessage += $"{e.Message}";
                    Log(logmessage, rw);    
                }
                throw;
            }

            MergeList graph;
            try
            {
                graph = new MergeList(Path.Combine(Environment.CurrentDirectory, filename));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("check max vertex index and edges. each vertex should be < maxVertexIndex");
                using (StreamWriter rw = File.AppendText("log.txt"))
                {
                    var logmessage = "Cannot detect file\n";
                    logmessage += $"{e.Message}";
                    Log(logmessage, rw);    
                }
                throw;
            }
            var graphParams = graph.GraphSizeInfo().Split();
            Console.Write($"Vertexes: {graphParams[0]}\n" +
                          $"Edges: {graphParams[1]}\n");

            MeasureTest(graph, start_point);
        }

        private static void MeasureTest(MergeList graph, int start_point)
        {
            StandMeasure(graph.Serial, start_point, "serial without check 5 times");
            StandMeasure(graph.Serial, start_point);
            StandMeasure(graph.Serial, start_point);
            StandMeasure(graph.Serial, start_point);
            StandMeasure(graph.Serial, start_point);

            StandMeasure(graph.Leveled, start_point, "leveled parallel without check 5 times");
            StandMeasure(graph.Leveled, start_point);
            StandMeasure(graph.Leveled, start_point);
            StandMeasure(graph.Leveled, start_point);
            StandMeasure(graph.Leveled, start_point);


            var Results = new List<List<int>>();
            Results.Add(StandMeasure(graph.SerialCheck, start_point, "serial with check 2 times"));
            Results.Add(StandMeasure(graph.SerialCheck, start_point));

            Results.Add(StandMeasure(graph.LeveledCheck, start_point, "leveled with check 2 times"));
            Results.Add(StandMeasure(graph.LeveledCheck, start_point));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Counted visited vertexes for each call with check above: ");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var result in Results)
            {
                Console.Write($"{result.Count} ");
            }

            Console.WriteLine(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Above is comparison each list between each other, if one of them differs, you will see False");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var sequenceEqual in Results
                .SelectMany(result1 => Results.Select(result2 => Enumerable
                    .SequenceEqual(result1.OrderBy(t => t), result2.OrderBy(t => t)))))
            {
                Console.ForegroundColor = sequenceEqual ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"{sequenceEqual} ");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}