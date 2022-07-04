using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUL
{
    /* Graph Implementation
        Strongly Connected Components
        */
    public class Graph<T> 
    {
        internal List<DLList<dynamic>> graph;

        public struct Vertex
        {
            public Colors Color;
            public int Distance;
            public T Predeccessor;
            public T Data; /* dynamic > T */
            public int[] time;
        }

        public enum Colors { White = 0, Grey = 1, Black = 2 }

        public Graph() { graph = new List<DLList<dynamic>>(); }

        public bool AddVertex(T data) /* dynamic > T */
        {
            for (int i = 0; i < graph.Count; i++)
            {
                if (graph.Count != 0)
                {
                    if (((Vertex)graph[i][0]).Data != null)
                    {
                        if (String.Compare(((Vertex)graph[i][0]).Data.ToString(), data.ToString()) == 0) { return false; }
                    }
                }
            }
            graph.Add(new DLList<dynamic>());
            graph[graph.Count - 1].Append(NewVertex(data)); /* new Vertex() { Color = Colors.White, Data = data, Distance = -1 } */
            return true;
        }

        public bool AddVertex(T data, T[] edges) /* dynamic > T */
            {
            for (int i = 0; i < graph.Count; i++)
            {
                if (graph.Count != 0)
                {
                    if (((Vertex)graph[i][0]).Data != null)
                    {
                        if (String.Compare(((Vertex)graph[i][0]).Data.ToString(), data.ToString()) == 0) { return false; }
                    }
                }
            }
            graph.Add(new DLList<dynamic>());
            graph[graph.Count - 1].Append(NewVertex(data));
            for (int i = 0; i < edges.Length; i++)
            {
                if (FindVertexIndex(edges[i]) != -1) { graph[graph.Count - 1].Append(new dynamic[2]{ edges[i], null }); }
                else { Console.WriteLine("Vertex {0} Edge {1} Skipped", RetrieveVertexData(graph.Count - 1) ,edges[i]); }
            }
            return true;
        }

        public bool AddEdge(T data, T[] edge)
        {
            int vertexindex = FindVertexIndex(data);
            if (vertexindex != -1)
            {
                foreach (var VARIABLE in edge)
                {
                    if (FindVertexIndex(VARIABLE) != -1)
                    {
                        if (vertexindex != -1)
                        {
                            if (FindEdgeIndex(data, VARIABLE) == -1)
                            {
                                graph[vertexindex].Append(new dynamic[2]{ VARIABLE, null });

                            }
                        }
                    }
                }
                return true;
            }
            else { return false; }
        }

        public void BFS(T startvertex)
        {
            var vertexindex = FindVertexIndex(startvertex);
            var vertex = (Vertex) graph[vertexindex][0];
            vertex.Color = Colors.Grey;
            vertex.Distance = 0;
            graph[vertexindex][0] = vertex;
            LLQueue<T> newLLQueue = new LLQueue<T>();
            newLLQueue.EnQueue(vertex.Data);
            while (newLLQueue.Count() != 0)
            {
                var tempDeQueue = newLLQueue.DeQueue();
                var u = ((Vertex) graph[FindVertexIndex(tempDeQueue)][0]);
                for (int i = 1; i < graph[FindVertexIndex(tempDeQueue)].Size; i++)
                {
                    var tempedge = ((dynamic)graph[FindVertexIndex(tempDeQueue)][i])[0];
                    var v = ((Vertex) graph[FindVertexIndex(tempedge)][0]);
                    if (v.Color == Colors.White)
                    {
                        v.Color = Colors.Grey;
                        v.Distance = v.Distance + 1;
                        v.Predeccessor = u.Data;
                        newLLQueue.EnQueue(v.Data);
                        graph[FindVertexIndex(tempedge)][0] = v;
                    }
                }
                u.Color = Colors.Black;
                graph[FindVertexIndex(tempDeQueue)][0] = u;
            }
        }

        public void PrintBFSPath(T start, T end)
        {
            var s = ((Vertex)graph[FindVertexIndex(start)][0]);
            var v = ((Vertex)graph[FindVertexIndex(end)][0]);

            if (start.ToString() == end.ToString()) { Console.WriteLine(start); }
            else if (v.Predeccessor.ToString() == "0") { Console.WriteLine("No Path Exists!"); }
            else { PrintBFSPath(start, v.Predeccessor); Console.WriteLine(v.Data); }
        }

        public void DFS()
        {
            int time = 0;
            for (int i = 0; i < graph.Count; i++)
            {
                var u = (Vertex)graph[i][0];
                if (u.Color == Colors.White) { time = DFS_Visit(u.Data, time); }
            }
        }

        internal int DFS_Visit(T vertex, int time)
        {
            time++;
            var vertexindex = FindVertexIndex(vertex);
            var u = ((Vertex) graph[vertexindex][0]);
            u.time[0] = time;
            u.Color = Colors.Grey;
            graph[vertexindex][0] = u;
            for (int i = 1; i < graph[vertexindex].Size; i++)
            {
                var tempedge = ((dynamic) graph[vertexindex][i])[0];
                var v = ((Vertex) graph[FindVertexIndex(tempedge)][0]);
                if (v.Color == Colors.White)
                {
                    v.Predeccessor = u.Data;
                    graph[FindVertexIndex(tempedge)][0] = v;
                    time = DFS_Visit(v.Data, time);
                }
            }
            u.Color = Colors.Black;
            time++;
            u.time[1] = time;
            graph[FindVertexIndex(u.Data)][0] = u;
            return time;
        }

        public int FindVertexIndex(T data)
        {
            for (int i = 0; i < graph.Count; i++)
            {
                if (((Vertex) graph[i][0]).Data != null)
                {
                    if (String.Compare(((Vertex)graph[i][0]).Data.ToString(), data.ToString()) == 0) { return i; }
                }
            }
            return -1;
        }

        public int FindEdgeIndex(T data, T edge)
        {
            var vertexindex = FindVertexIndex(data);
            if (vertexindex != -1)
            {
                for (int i = 1; i < graph[vertexindex].Size/*Size()*/; i++)
                {
                    if (String.Compare(((dynamic)graph[vertexindex][i])[0].ToString(), edge.ToString()) == 0) /* ((Vertex) graph[vertexindex][i]).Data.ToString() */
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private Vertex NewVertex(T data)
        {
            return new Vertex()
            {
                Color = Colors.White,
                Data = data,
                Distance = 0 /* changed distance from -1 -> 0 */,
                Predeccessor = default(T),
                time = new int[2]{ -1, -1 }
            };
        }

        public T RetrieveVertexData(int index)
        {
            return ((Vertex) graph[index][0]).Data;
        }

        public bool RemoveVertex(T data)
        {
            for (int i = 0; i < graph.Count; i++)
            {
                if (graph.Count != 0)
                {
                    if (((Vertex)graph[i][0]).Data != null)
                    {
                        if (String.Compare(((Vertex) graph[i][0]).Data.ToString(), data.ToString()) == 0)
                        {
                            graph[i].Clear();
                            graph[i] = null;
                            for (int j = 0; j < graph.Count; j++)
                            {
                                if (graph[j] != null)
                                {
                                    int tempindex = FindEdgeIndex(RetrieveVertexData(j), data);
                                    if (tempindex != -1) { graph[tempindex][i] = null; }
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool RemoveEdge(T data,T edge)
        {
            int vertexindex = FindVertexIndex(data);
            if (vertexindex != -1)
            {
                for (int i = 1; i < graph[vertexindex].Size/*Size()*/; i++)
                {
                    if (String.Compare(((dynamic)graph[vertexindex][i])[0].ToString(), edge.ToString()) == 0) /* ((Vertex) graph[vertexindex][i]).Data.ToString() */
                    {
                        graph[vertexindex][i] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        public DLList<Vertex> TopologicalSort()
        {
            DFS();
            DLList<Vertex> newDLinkedList = new DLList<Vertex>();
            for (int i = 0; i < graph.Count; i++)
            {
                int j = 0;
                Vertex u = (Vertex)graph[i][0];
                while (j < newDLinkedList.Size)
                {
                    if (((Vertex)newDLinkedList[j]).time[1] < u.time[1]) { j++; }
                    else { break; }
                }
                newDLinkedList.Append(u);
            }
            return newDLinkedList;
        }

        public void VertexRevert()
        {
            for (int i = 0; i < graph.Count; i++)
            {
                var tempvertex = ((Vertex) graph[i][0]);
                tempvertex.Color = Colors.White;
                tempvertex.Predeccessor = default(T);
                tempvertex.time[0] = -1;
                tempvertex.time[1] = -1;
                tempvertex.Distance = 0;
                graph[i][0] = tempvertex;
            }
        }
    }
}
