using Util;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;
using System.Numerics;
using OpenTK.Windowing.Common.Input;
using System.Xml.Linq;
//using System.Numerics;

namespace rt004
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Predetermined names for json files
            string solidsFile = "SolidsConfig.json";
            string lightsFile = "LightsConfig.json";
            string camerasFile = "CamerasConfig.json";


            string fileName;

            //Reading Json and setting up the scene
            string json = File.ReadAllText(solidsFile);
            Console.WriteLine(json);

            Scene scene = new Scene();
            SolidHierarchy[] solidHierarchies = SetUp.GetComp<SolidHierarchy>(solidsFile);
            scene.solidHierarchy = solidHierarchies[0];
            scene.lights = SetUp.GetComp<ILights>(lightsFile);
            Camera[] camera = SetUp.GetComp<Camera>(camerasFile);

            int i = 0;


            //TranslateTest
            Ray testR = new Ray(new Vector3d(4,-5, 2), new Vector3d(4, 3, 2));
            //ITransformations testT = new Translate(4, 5, -2);

            //Ray newR = testT.MultiplyL(testR);
            //Console.WriteLine(newR.origin + "  " + newR.direction);


            //solidHierarchies[0].TEST();
            Node temp = solidHierarchies[0].root;
            List<Matrix4d> stack = new List<Matrix4d>();
            Matrix4d tempM = Matrix4d.Identity;
            Matrix4d tempMInverse = Matrix4d.Identity;

            SolidHierarchy solidHierarchy = scene.solidHierarchy;
            List<Matrix4d> reverseTransformations = new List<Matrix4d>();  
            //Matrix which we transform
            Matrix4d traverseMatrix = Matrix4d.Identity;
            //Matrix with which we transform traverseMatrix
            Matrix4d activeMatrix = Matrix4d.Identity;

            while (solidHierarchy.AssertTransforms())
            {
                //Traversing to the leaf nodes
                //Apply inverse matrix of the saved matrices
                //Save matrix for backtracking into array of reverse transformations

                if (solidHierarchy.Down)
                {

                    activeMatrix = solidHierarchy.currNode.transformations.tM;
                    reverseTransformations.Add(activeMatrix.Inverted());
                    traverseMatrix = Matrix4d.Mult(activeMatrix, traverseMatrix);

                    foreach (ISolids solid in solidHierarchy.currNode.solids)
                    {
                        solid.Transform = traverseMatrix;
                    }
                }
                else if (!solidHierarchy.Down)
                {
                    activeMatrix = reverseTransformations.Last();
                    reverseTransformations.RemoveAt(reverseTransformations.Count - 1);
                    traverseMatrix = Matrix4d.Mult(activeMatrix, traverseMatrix);
                }
            }

            /*
            while (solidHierarchies[0].AssertTransforms())
            {
                if (!solidHierarchies[0].Down) { 

                    //Add inverse here
                    temp.transformations.tM = tempM;
                    temp.transformations.tmInverse = tempMInverse;

                    if (stack.Count() >= 1) {

                        //Add inverse here
                        tempM =  stack[stack.Count() - 1] * tempM;
                        tempMInverse = stack[stack.Count() - 1].Inverted() * tempMInverse;
                    }
                    stack.RemoveAt(stack.Count() - 1);
                    foreach (ISolids solid in temp.solids)
                    {
                        solid.Transform = temp.transformations;
                    }
                }
                if (solidHierarchies[0].Down) { 
                    
                    stack.Add(solidHierarchies[0].currNode.transformations.Inverse());
                    //Add inverse here
                    tempM = solidHierarchies[0].currNode.transformations.tM * tempM;
                    tempMInverse = solidHierarchies[0].currNode.transformations.tM.Inverted() * tempMInverse ;


                }

                temp = solidHierarchies[0].currNode;
            }
            */

            SolidHierarchyContainer root = new SolidHierarchyContainer(solidHierarchies[0].root);
            foreach(Node node in root)
            {
                //Console.WriteLine(node.transformations.tM);
                //Console.WriteLine();
            }

            while (solidHierarchies[0].AssertTransforms())
            {

            }
            Console.WriteLine(i);


            foreach (Camera c in camera)
            {
                fileName = $"Output/demo{i++}.pfm";
                FloatImage img = c.RenderScene(scene);
                img.SavePFM(fileName);
            }
            
            Console.WriteLine("Finnished");
        }
    }
}