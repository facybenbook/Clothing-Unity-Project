using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Text;
//using System.Windows.Forms;
namespace IntelligentScissors
{

    public static class GraphOperations
    {
        #region Neighbours
        public static List<Edge> Get_neighbours(int Node_Index, RGBPixel[,] ImageMatrix)
        {
            List<Edge> neighbours = new List<Edge>();
            int Height = ImageOperations.GetHeight(ImageMatrix);
            int Width = ImageOperations.GetWidth(ImageMatrix);
            //get x , y indices of the node
            var unflat = Helper.Unflatten(Node_Index, Width);
            int X = (int)unflat.X;
            int Y = (int)unflat.Y;
            // calculate the gradient with right and bottom neighbour
            var Gradient = ImageOperations.CalculatePixelEnergies(X, Y, ImageMatrix);
            if (X < Width - 1) // have a right neighbour ?  
            {
                //add to neighbours list with cost 1/G
                if (Gradient.X == 0)
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X + 1, Y, Width), 10000000000000000));
                else
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X + 1, Y, Width), 1 / (Gradient.X)));
            }
            if (Y < Height - 1) // have a Bottom neighbour ?
            {
                //add to neighbours list with cost 1/G
                if (Gradient.Y == 0)
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X, Y + 1, Width), 10000000000000000));
                else
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X, Y + 1, Width), 1 / (Gradient.Y)));
            }
            if (Y > 0) // have a Top neighbour ?
            {
                // calculate the gradient with top neighbour
                Gradient = ImageOperations.CalculatePixelEnergies(X, Y - 1, ImageMatrix);
                //add to neighbours list with cost 1/G
                if (Gradient.Y == 0)
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X, Y - 1, Width), 10000000000000000));
                else
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X, Y - 1, Width), 1 / (Gradient.Y)));
            }
            if (X > 0) // have a Left neighbour ?
            {
                // calculate the gradient with left neighbour
                Gradient = ImageOperations.CalculatePixelEnergies(X - 1, Y, ImageMatrix);

                //add to neighbours list with cost 1/G 
                if (Gradient.X == 0)
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X - 1, Y, Width), 10000000000000000));
                else
                    neighbours.Add(new Edge(Node_Index, Helper.Flatten(X - 1, Y, Width), 1 / (Gradient.X)));
            }
            return neighbours; // return neighbours
        }
        #endregion
        #region UN-USED CODE, WHERE WE GOT THE GRAPH FROM THE PICTURE
        public static List<List<Edge>> Graph_Constraction(RGBPixel[,] ImageMatrix)
        {
            int Height = ImageOperations.GetHeight(ImageMatrix);
            int Width = ImageOperations.GetWidth(ImageMatrix);
            // constract empty adjacency List 
            List<List<Edge>> adj_list = new List<List<Edge>>();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int node_index = Helper.Flatten(j, i, Width); // get flat pixel x,y to 1d number 
                    //constract neighbours list of current pixel(node_index) and add it in  the adj list
                    adj_list.Add(Get_neighbours(node_index, ImageMatrix));
                }
            }
            return adj_list; // return graph adj list
        }
        #endregion
    }
}
