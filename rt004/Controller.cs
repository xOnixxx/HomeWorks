using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace rt004
{
    internal class Controller
    {

        public Controller()
        {

        }

        public string solidsFile { get; set; } = "SolidsConfig.json";
        public string lightsFile { get; set; } = "LightsConfig.json";
        public string camerasFile { get; set; } = "CamerasConfig.json";


        private Scene scene = new Scene();
        private Camera[] camera;


        public void LoadFromJson()
        {
            GenerateScene();
            GenerateHierarchy();
            GenerateCamera();
        }

        public void GenerateScene()
        {
            SolidHierarchy[] solidHierarchies = SetUp.GetComp<SolidHierarchy>(solidsFile);
            scene.solidHierarchy = solidHierarchies[0];
            scene.lights = SetUp.GetComp<ILights>(lightsFile);
        }

        public void GenerateCamera()
        {
            camera = SetUp.GetComp<Camera>(camerasFile);
        }

        private void GenerateHierarchy()
        {
            List<Matrix4d> reverseTransformations = new List<Matrix4d>();

            //Matrix which we transform
            Matrix4d traverseMatrix = Matrix4d.Identity;
            Matrix4d activeMatrix = Matrix4d.Identity;

            while (scene.solidHierarchy.AssertTransforms())
            {
                //Traversing to the leaf nodes
                //Apply inverse matrix of the saved matrices
                //Save matrix for backtracking into array of reverse transformations

                if (scene.solidHierarchy.Down)
                {
                    activeMatrix = scene.solidHierarchy.currNode.transformations.tM;
                    reverseTransformations.Add(activeMatrix.Inverted());
                    traverseMatrix = Matrix4d.Mult(activeMatrix, traverseMatrix);

                    foreach (ISolids solid in scene.solidHierarchy.currNode.solids)
                    {
                        solid.Transform = traverseMatrix;
                    }
                }
                else if (!scene.solidHierarchy.Down)
                {
                    activeMatrix = reverseTransformations.Last();
                    reverseTransformations.RemoveAt(reverseTransformations.Count - 1);
                    traverseMatrix = Matrix4d.Mult(activeMatrix, traverseMatrix);
                }
            }

            while (scene.solidHierarchy.AssertTransforms())
            {

            }

        }

        public void GenerateCaptures()
        {
            string fileName;
            int i = 0;
            foreach (Camera c in camera)
            {
                
                fileName = $"Output/demo{i++}.pfm";
                FloatImage img = c.RenderScene(scene, MathHelp.Jittering);
                img.SavePFM(fileName);
            }
        }

    }
}
