using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentScissors
{

    public static class ShortestPath_Operations
    {
        /// <summary>
        /// generate the shortest path from source node to all other nodes
        /// </summary>
        /// <param name="Graph"></param>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <returns></returns>
        public static List<Point> GenerateShortestPath(int Source, int Dest, RGBPixel[,] ImageMatrix)
        {
            // return sortest path btween src & all other nodes 
            List<int> Previous_list = Dijkstra(Source, Dest, ImageMatrix);
            // Backtracking shortest path to (Dest)node  from previous list 
            return Backtracking(Previous_list, Dest, ImageOperations.GetWidth(ImageMatrix));
        }
        /// <summary>
        /// backtracing the shortestpath btween 2 nodes src, dest
        /// </summary>
        /// <param name="All_Paths"></param>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <returns>shortestpath</returns>
        public static List<Point> Backtracking(List<int> Previous_list, int Dest, int matrix_width)
        {
            List<Point> ShortestPath = new List<Point>(); // shortest path bteewn Source node and destination
            Stack<int> RevPath = new Stack<int>();   //the reversed shortest path bteewn Source node and destination           
            RevPath.Push(Dest); // push the destination node 
            int previous = Previous_list[Dest]; // previous of the destination (current node)
            // backtracking the shortest path from all paths
            while (previous != -1)
            {
                RevPath.Push(previous); // push last node in the path   
                previous = Previous_list[previous]; //previous of current node
            }
            //revrese the reversed path 
            while (RevPath.Count != 0)
            {
                var p = Helper.Unflatten(RevPath.Pop(), matrix_width);
                Point point = new Point((int)p.X, (int)p.Y);
                ShortestPath.Add(point);
            }
            // return shortest path bteewn Source node and destination
            return ShortestPath;
        }
        /// <summary>
        /// Dijkstra algorithm
        /// </summary>
        /// <param name="Graph"></param>
        /// <param name="Source"></param>
        /// <returns>list of all shortest paths btween a source node and all nodes </returns>
        /// 
        #region DIJKSTRA ALGORITHMS
        public static List<int> Dijkstra(int Source, RGBPixel[,] ImageMatrix)
        {
            const double oo = 10000000000000000000; // infity value
            //Distance : the minimum cost between the source node and all the others nodes
            //initialized with infinty value
            int Width = ImageOperations.GetWidth(ImageMatrix);
            int Height = ImageOperations.GetHeight(ImageMatrix);
            int nodes_number = Width * Height;
            List<double> Distance = new List<double>();
            Distance = Enumerable.Repeat(oo, nodes_number).ToList();
            //Previous : saves the previous node that lead to the shortest path from the src node to current node 
            List<int> Previous = new List<int>();
            Previous = Enumerable.Repeat(-1, nodes_number).ToList();
            // SP between src and it self costs 0 
            // PeriorityQueue : always return the shortest bath btween src node and specific node  
            PeriorityQueue<Edge> MinimumDistances = new PeriorityQueue<Edge>();
            MinimumDistances.Push(new Edge(-1, Source, 0));
            while (!MinimumDistances.IsEmpty())
            {
                // get the shortest path so far 
                Edge CurrentEdge = MinimumDistances.Top();
                MinimumDistances.Pop();
                // check if this SP is vaild (i didn't vist this node with a less cost)
                if (CurrentEdge.Weight >= Distance[CurrentEdge.To])
                    continue;
                // save the previous 
                Distance[CurrentEdge.To] = CurrentEdge.Weight;
                Previous[CurrentEdge.To] = CurrentEdge.From;
                // Relaxing 
                List<Edge> neibours = GraphOperations.Get_neighbours(CurrentEdge.To, ImageMatrix);
                for (int i = 0; i < neibours.Count; i++)
                {
                    Edge HeldEdge = neibours[i];
                    // if the relaxed path cost of a neighbour node is less than  it's previous one
                    if (Distance[HeldEdge.To] > Distance[HeldEdge.From] + HeldEdge.Weight)
                    {
                        // set the relaxed cost to Distance  && pash it to the PQ
                        HeldEdge.Weight = Distance[HeldEdge.From] + HeldEdge.Weight;
                        MinimumDistances.Push(HeldEdge);
                    }
                }
            }
            return Previous;  // re turn th shortest paths from src to all nodes
        }
        public static List<int> Dijkstra(int Source, int dest, RGBPixel[,] ImageMatrix)
        {
            const double oo = 10000000000000000000; // infity value
            //Distance : the minimum cost between the source node and all the others nodes
            //initialized with infinty value
            int Width = ImageOperations.GetWidth(ImageMatrix);
            int Height = ImageOperations.GetHeight(ImageMatrix);
            int nodes_number = Width * Height;
            List<double> Distance = new List<double>();
            Distance = Enumerable.Repeat(oo, nodes_number).ToList();

            //Previous : saves the previous node that lead to the shortest path from the src node to current node 
            List<int> Previous = new List<int>();
            Previous = Enumerable.Repeat(-1, nodes_number).ToList();

            // PeriorityQueue : always return the shortest bath btween src node and specific node  
            PeriorityQueue<Edge> MinimumDistances = new PeriorityQueue<Edge>();
            MinimumDistances.Push(new Edge(-1, Source, 0));
            while (!MinimumDistances.IsEmpty())
            {
                // get the shortest path so far 
                Edge CurrentEdge = MinimumDistances.Top();
                MinimumDistances.Pop();
                // check if this SP is vaild (i didn't vist this node with a less cost)
                if (CurrentEdge.Weight >= Distance[CurrentEdge.To])
                    continue;
                // save the previous 
                Previous[CurrentEdge.To] = CurrentEdge.From;
                Distance[CurrentEdge.To] = CurrentEdge.Weight;
                if (CurrentEdge.To == dest) break;
                // Relaxing 
                List<Edge> neibours = GraphOperations.Get_neighbours(CurrentEdge.To, ImageMatrix);
                for (int i = 0; i < neibours.Count; i++)
                {
                    Edge HeldEdge = neibours[i];
                    // if the relaxed path cost of a neighbour node is less than  it's previous one
                    if (Distance[HeldEdge.To] > Distance[HeldEdge.From] + HeldEdge.Weight)
                    {
                        // set the relaxed cost to Distance  && pash it to the PQ
                        HeldEdge.Weight = Distance[HeldEdge.From] + HeldEdge.Weight;
                        MinimumDistances.Push(HeldEdge);
                    }
                }
            }
            return Previous;  // re turn th shortest paths from src to all nodes
        }
        #endregion
        // return bondry limit of dikstra
        public static Boundary Square_Boundary(int Src, int Width, int Height)
        {
            Vector2D Src2d = Helper.Unflatten(Src, Width + 1); //src node  x y
            Boundary bondry = new Boundary();
            int max_dist = 200;
            if (Src2d.X > max_dist)
                bondry.MIN_X = (int)Src2d.X - max_dist;
            else
                bondry.MIN_X = 0;
            if (Width - Src2d.X > max_dist)
                bondry.MAX_X = (int)Src2d.X + max_dist;
            else
                bondry.MAX_X = Width;
            if (Src2d.Y > max_dist)
                bondry.MIN_Y = (int)Src2d.Y - max_dist;
            else
                bondry.MIN_Y = 0;
            if (Height - Src2d.Y > max_dist)
                bondry.MAX_Y = (int)Src2d.Y + max_dist;
            else
                bondry.MAX_Y = Height;
            return bondry;
        }
    }
}
