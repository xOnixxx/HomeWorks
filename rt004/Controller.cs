using OpenTK.Mathematics;
using System.Drawing;
using Util;

namespace rt004
{
    internal static class Controller
    {

        private static string solidsFile { get; set; } = "SolidsConfig.json";
        private static string lightsFile { get; set; } = "LightsConfig.json";
        private static string camerasFile { get; set; } = "CamerasConfig.json";


        private static Scene scene = new Scene();
        private static Camera[] camera;

        public static Dictionary<string, Bitmap> textures = new Dictionary<string, Bitmap>();

        public static void LoadTextures()
        {
            var TextureFolder = Directory.GetFiles(@"Textures", "*", SearchOption.AllDirectories);
            int fCount = TextureFolder.Length;

            foreach (var texture in TextureFolder)
            {
                textures.Add(texture, new Bitmap(texture));
            }
        }


        public static void LoadFromJson()
        {
            GenerateScene();
            GenerateHierarchy();
            GenerateCamera();
        }

        public static void GenerateScene()
        {
            SolidHierarchy[] solidHierarchies = SetUp.GetComp<SolidHierarchy>(solidsFile);
            scene.solidHierarchy = solidHierarchies[0];
            scene.lights = SetUp.GetComp<ILights>(lightsFile);
        }

        public static void GenerateCamera()
        {
            camera = SetUp.GetComp<Camera>(camerasFile);
        }

        private static void GenerateHierarchy()
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

                    if (scene.solidHierarchy.currNode.solids != null)
                    {
                        foreach (ISolids solid in scene.solidHierarchy.currNode.solids)
                        {
                            solid.Transform = traverseMatrix;
                        }
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

        public static void GenerateCaptures()
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
