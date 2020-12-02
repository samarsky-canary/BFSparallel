using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultNamespace
{
    public class MergeList
    {
        public List<List<int>> Nodes { get; set; }

        public MergeList(string filename)
        {
            using (var file = new StreamReader(filename))
            {
                var n = Convert.ToInt32(file.ReadLine());
                InitDist(n);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var nodes = line.Split();
                    Nodes[Convert.ToInt32(nodes[0])].Add(Convert.ToInt32(nodes[1]));
                }

                file.Close();
            }
        }

        public override string ToString()
        {
            var response = "";
            foreach (var node in Nodes)
            {
                response += $"{Environment.NewLine}{Nodes.IndexOf(node)}:";
                Parallel.For(0, node.Count, i => { response += $"{node[i]} "; });
            }

            return response;
        }

        private void InitDist(int n)
        {
            Nodes = new List<List<int>>();
            for (int i = 0; i < n; i++)
                Nodes.Add(new List<int>());
        }


        public int Serial(int vertex)
        {
            var queue = new Queue<int>();
            var visited = new List<bool>();
            for (int i = 0; i < Nodes.Count; i++)
                visited.Add(false);

            queue.Enqueue(vertex);

            while (queue.Count != 0)
            {
                var v = queue.Dequeue();
                visited[v] = true;
                foreach (var neighb in Nodes[v])
                {
                    if (!visited[neighb])
                        queue.Enqueue(neighb);
                }
            }

            return visited.Count(b => b);
        }

        public void Leveled(int initial_vertex)
        {
            // init distances for our vertex
            var dist = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
                dist.Add(-1);

            dist[initial_vertex] = 0;
            // set level to zero
            var level = 0;

            var this_node = new List<int>();
            this_node.Add(initial_vertex);
            var buffer = new List<int>();

            while (this_node.Count != 0)
            {
                // foreach (var node in this_node)
                // {
                //     Console.Write($"{node} ");
                // }
                // Console.Write(Environment.NewLine);
                Parallel.For(0, this_node.Count, node =>
                {
                    var vert = this_node[node];
                    // если вершина текущего уровня
                    if (dist[vert] == level)
                    {
                        foreach (var neighbor in Nodes[vert])
                        {
                            // всех непосещенных соседей помечаем
                            if (dist[neighbor] == -1)
                                lock ("neighbor")
                                {
                                    dist[neighbor] = level + 1; 
                                    buffer.Add(neighbor);
                                }
                        }
                    }
                });
                this_node = buffer;
                buffer = new List<int>();
                level++;
            }
        }
        
        public List<int> LeveledCheck(int initial_vertex)
        {
            // init distances for our vertex
            var dist = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
                dist.Add(-1);

            dist[initial_vertex] = 0;
            // set level to zero
            var level = 0;

            var this_node = new List<int>();
            this_node.Add(initial_vertex);
            var buffer = new List<int>();
            
            
            
            var result = new List<int>();
            result.Add(initial_vertex);
            
            while (this_node.Count != 0)
            {
                Parallel.For(0, this_node.Count, node =>
                {
                    var vert = this_node[node];
                    // если вершина текущего уровня
                    if (dist[vert] == level)
                    {
                        foreach (var neighbor in Nodes[vert])
                        {
                            // всех непосещенных соседей помечаем
                            if (dist[neighbor] == -1)
                                lock ("neighbor")
                                {
                                    dist[neighbor] = level + 1; 
                                    buffer.Add(neighbor);
                                }
                        }
                    }
                });
                this_node = buffer;
                result.AddRange(buffer);
                buffer = new List<int>();
                level++;
            }

            return result.Distinct().ToList();
        }
        
    }
}