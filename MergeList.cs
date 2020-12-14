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
                InitNodes(n);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var nodes = line.Split();
                    Nodes[Convert.ToInt32(nodes[0])].Add(Convert.ToInt32(nodes[1]));
                }

                file.Close();
            }
        }

        public int GetMaxVertex()
        {
            return Nodes.Count;
        }
        public string GraphSizeInfo()
        {
            var vertexes = Nodes.Count;
            var edges = Nodes.Sum(node => node.Count);
            return $"{vertexes} {edges}";
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

        private void InitNodes(int n)
        {
            Nodes = new List<List<int>>();
            for (int i = 0; i < n; i++)
                Nodes.Add(new List<int>());
        }


        public void Serial(int vertex)
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

            // for (var i = 0; i < Nodes.Count; i++)
            // {
            //     if (visited[i] == false)
            //         Console.WriteLine(i);
            // }
        }

        public List<int> SerialCheck(int vertex)
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
            var result = new List<int>();
            for (var i = 0; i < visited.Count; i++)
            {
                if (visited[i])
                    result.Add(i);
            }
            return result;
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

            var this_level = new List<int>();
            this_level.Add(initial_vertex);
            var next_level = new List<int>();

            while (this_level.Count != 0)
            {
                Parallel.For(0, this_level.Count, node =>
                {
                    var vert = this_level[node];
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
                                    next_level.Add(neighbor);
                                }
                        }
                    }
                });
                this_level = next_level;
                next_level = new List<int>();
                level++;
            }

            // var result = dist.Count(i => i >= 0);
            // Console.WriteLine(result);
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

            var this_level = new List<int>();
            this_level.Add(initial_vertex);
            var buffer = new List<int>();
            
            
            
            var result = new List<int>();
            result.Add(initial_vertex);
            
            while (this_level.Count != 0)
            {
                Parallel.For(0, this_level.Count, node =>
                {
                    var vert = this_level[node];
                    // если вершина текущего уровня
                    if (dist[vert] == level)
                    {
                        foreach (var neighbor in Nodes[vert])
                        {
                            // всех непосещенных соседей помечаем
                                lock ("neighbor")
                                {
                                    if (dist[neighbor] == -1)
                                    {
                                        dist[neighbor] = level + 1; 
                                        buffer.Add(neighbor);   
                                    }
                                }
                        }
                    }
                });
                this_level = buffer;
                result.AddRange(buffer);
                buffer = new List<int>();
                level++;
            }

            return result;
        }
        
    }
}