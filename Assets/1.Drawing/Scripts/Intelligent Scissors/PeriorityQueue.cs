using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IntelligentScissors
{
    #region PERIORITY QUEUE
    /// <summary>
    /// the periority queue heap based implementation
    /// </summary>
    public class PeriorityQueue<T> where T : IWeightable
    {
        private List<T> Heap = new List<T>(); //array of edges
        private void swap ( int x, int y )
        {
            int temp = x;
            x = y;
            y = temp;
        }
        private int Left ( int Node )
        {
            //private function returns the index of the expected left child of the node at index Node 
            return Node * 2 + 1;
            //
        }
        private int Right ( int Node )
        {
            //private function returns the index of the expected right child of the node at index Node 
            return Node * 2 + 2;
        }
        private int Parent ( int Node )
        {
            //private function returns the index of the expected parent of the node at index Node 
            return ( Node - 1 ) / 2;
        }
        /// <summary>
        /// the function that modify our tree(Heap) after adding elements
        /// </summary>
        /// <param name="Node"></param>
        private void SiftUp ( int Node )
        {
            //in the case of you are a root (position 0) 
            //or your value is bigger than the value of your parent
            // you have no thing to do here, just return 
            if ( Node == 0 || Heap [ Node ].Weight >= Heap [ Parent ( Node ) ].Weight )
                return;
            //swap(Heap[Parent(Node)], Heap[Node]);
            T temp = Heap[Parent(Node)];
            Heap [ Parent ( Node ) ] = Heap [ Node ];
            Heap [ Node ] = temp;
            //
            //now let's continue positioning-fitting process of the position of the given node ( which became a parent ) 
            SiftUp ( Parent ( Node ) );
        }
        /// <summary>
        /// the function that modify our tree(Heap) after deleting(poping) elements
        /// </summary>
        /// <param name="Node"></param>
        private void SiftDown ( int Node )
        {
            //in case of: you have no left child ( absolutely you will not have even right one 
            //or you have a only left child (no right one) which is in its accurate position(it is smaller than his child on left)
            //or you have 2 children which are both greater than you
            // therefore, you have no thing to do, just return home :D
            if ( Left ( Node ) >= Heap.Count
                || ( Left ( Node ) < Heap.Count && Right ( Node ) >= Heap.Count && Heap [ Left ( Node ) ].Weight >= Heap [ Node ].Weight )
                || ( Left ( Node ) < Heap.Count && Right ( Node ) < Heap.Count && Heap [ Left ( Node ) ].Weight >= Heap [ Node ].Weight &&
                Heap [ Right ( Node ) ].Weight >= Heap [ Node ].Weight ) )
                return;
            //in case of you have a right child, and this child is smaller than his brother on left
            //just rise this cute small up :D
            if ( Right ( Node ) < Heap.Count && Heap [ Right ( Node ) ].Weight <= Heap [ Left ( Node ) ].Weight )
            {
                T temp = Heap[Right(Node)];
                Heap [ Right ( Node ) ] = Heap [ Node ];
                Heap [ Node ] = temp;
                SiftDown ( Right ( Node ) );
            }
            //in case of you have not a right child
            //just rise your only left child up he is cute too :D
            else
            {
                T temp = Heap[Left(Node)];
                Heap [ Left ( Node ) ] = Heap [ Node ];
                Heap [ Node ] = temp;
                SiftDown ( Left ( Node ) );
            }
        }
        public PeriorityQueue ( )
        {
            //
        }
        /// <summary>
        /// Add element to the periority queue
        /// </summary>
        /// <param name="Node"></param>
        public void Push ( T Node )
        {
            Heap.Add ( Node );//add him at the end
            SiftUp ( Heap.Count - 1 );//now modify the heap, as the last position made the heap not well modified
        }
        /// <summary>
        /// Remove the smallest element from your heap, and then modify it to have the next smallest element in the front
        /// </summary>
        /// <returns></returns>
        public T Pop ( )
        {
            T temp = Heap[0];//hold the element before killing him :D
            Heap [ 0 ] = Heap [ Heap.Count - 1 ];//remove the smallest one,overwrite it the last element in the heap
            Heap.RemoveAt ( Heap.Count - 1 );//remove this guy at the back, he came in the front of the heap :D
            SiftDown ( 0 );//NOW, we have a non-accurate heap, let's put this guy at the frontm in his acctual(expected position) :D
            return temp;//whom did you delete now :D ?
        }
        /// <summary>
        /// returns if the heap still has elements or not
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty ( )
        {
            if ( Heap.Count == 0 )//if you had run out of them , say true :D
                return true;
            return false;
        }
        /// <summary>
        /// What is your size ? 
        /// </summary>
        /// <returns></returns>
        public int Size ( )
        {
            return Heap.Count;
        }
        /// <summary>
        /// returns the first and smallest element in the heap
        /// </summary>
        /// <returns></returns>
        public T Top ( )
        {
            return Heap [ 0 ];
        }
    }
    #endregion
}
