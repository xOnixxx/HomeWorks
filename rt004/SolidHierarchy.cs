using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{        
    public class Node{
        //Pointer for the BFS
        private int pointer = -1;
        public void resetPointer() { pointer = -1; }
        //Node Transformation
        //public Matrix4d transformation;

        public ITransformations transformations { get; set; }
        public ISolids[] solids { get; set; }

        /*public double[] Transformation { get { return new double[] { }; } set { this.transformation = new Matrix4d(
            value[0], value[1], value[2], value[3],
            value[4], value[5], value[6], value[7],
            value[8], value[9], value[10], value[11],
            value[12], value[13], value[14], value[15]
            ); } }
        */


        public Node[]? sons { get; set; }
        public Node activeNode { get { return sons[pointer]; } }
        public Node(int key, Node[]? sons)
        {
            this.sons = sons;
        }

        public bool nextNode()
        {
            if (this.sons == null || pointer == sons.Count() - 1) {
                pointer = -1;
                return false; }
            else { pointer++; }

            return true;
        }

        public Matrix4d[] transformStack { get; set; }
        public ISolids activeSolid;
    }

    internal class SolidHierarchy
    {
        public Node root;
        public bool Down = true;
        private List<Node> activeNodes { get; set; } = new List<Node>();
        public Node currNode;


        public bool AssertTransforms()
        {
            if (currNode == null) { currNode = root; }
            if (currNode.nextNode())
            {
                Down = true;
                activeNodes.Add(currNode);
                currNode = currNode.activeNode;
            }
            else
            {
                if (activeNodes.Count == 0)
                {
                    return false;
                }
                currNode = activeNodes.Last();
                Down = false;
                activeNodes.RemoveAt(activeNodes.Count() - 1);
            }
            //BFS backtracing
            return true;
        }

    }

    //The root node is never read in the foreach statement
    //It only serves as the first node
    internal class SolidHierarchyContainer : IEnumerable
    {
        public Node root;

        public SolidHierarchyContainer(Node root)
        {
            this.root = root;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public NodeEnumerator GetEnumerator()
        {
            return new NodeEnumerator(root);
        }
    }


    internal class NodeEnumerator : IEnumerator<Node>
    {
        private Node root;
        private bool Down = true;
        private List<Node> activeNodes { get; set; } = new List<Node>();
        private Node currNode;

        public NodeEnumerator(Node root)
        {
            this.root = root;
            this.currNode = root;
        }
        public Node Current { get { return currNode; } }
        object IEnumerator.Current { get { return Current; } }

        public void Reset() { currNode = root; }

        public bool MoveNext()
        {
            if (currNode.nextNode())
            {
                Down = true;
                activeNodes.Add(currNode);
                currNode = currNode.activeNode;
            }
            else
            {
                if (activeNodes.Count == 0)
                {
                    return false;
                }
                currNode = activeNodes.Last();
                Down = false;
                activeNodes.RemoveAt(activeNodes.Count() - 1);
            }
            //BFS backtracing
            if (!Down) { if (!MoveNext()) { return false; } ; }
            return true;
        }
        void IDisposable.Dispose()
        {
            currNode.resetPointer();
            foreach (Node node in activeNodes)
            {
                node.resetPointer();
            }
            activeNodes.Clear();
            Reset();
        }
    }
}
