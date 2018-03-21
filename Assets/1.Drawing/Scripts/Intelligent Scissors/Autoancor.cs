using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IntelligentScissors
{
    public static class Autoancor
    {
        public static List<int> redrawn;
        public static List<double> cool_Time;
        public static List<Vector2> Live_Wire;
        
        public static void reset()
        {
            Live_Wire = new List<Vector2> ();
            cool_Time = new List<double>();
            redrawn = new List<int>(); 
        }

        public static void Update(List<Vector2> Path , double Ctime)
        { 
            int pathsize = Path.Count;
            int I = 0; 

            int Live_wiresize = Live_Wire.Count;
            int J = 0;

            while (I < pathsize &&  J < Live_wiresize)
            {
                if (Path[I] == Live_Wire[J])
                {
                    cool_Time[J] += Ctime;
                    redrawn[J] += 1;
                }
                else
                {
                    Live_Wire[J] = Path[I];
                    cool_Time[J] = 0;
                    redrawn[J]   = 0;
                }
                I++;
                J++;
            }

            while (I < pathsize)
            {
                Live_Wire.Add(Path[I]);
                cool_Time.Add(0);
                redrawn.Add(0);
                I++;
            }

            while (J < Live_wiresize)
            {         
                Live_Wire[J] = new Vector2 ( -1, -1);
                cool_Time[J] = 0;
                redrawn[J] = 0;
                J++;
            }
        }     

        public static List<Vector2> anchor_path()
        {
            List<Vector2> cooledpath = new List<Vector2>();
            int frezed = 0;
            for (int i = 0; i < Live_Wire.Count; i++)
            {
                if (redrawn[i] >= 8 && cool_Time[i] > 1) // freez point condition
                {
                    frezed = i;
                }
            }
            for (int i = 0; i <frezed; i++)
            {
                cooledpath.Add(Live_Wire[i]);
            }
            return cooledpath;
        }

    }
}
