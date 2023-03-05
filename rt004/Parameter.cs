using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    //Baseline for any picture parameters for JSON
    internal interface IParameter
    {
        public float width { get; set; }
        public float height { get; set; }
        public ImageFormat format { get; set; }
        public string outputFile { get; set; }
        public CameraMode cameraMode { get; set; }
    }

    internal class SimpleIMG : IParameter
    {
        public float width { get; set; }
        public float height { get; set; }
        public ImageFormat format { get; set; }
        public string outputFile { get; set; }
        public CameraMode cameraMode { get; set;}
    }

    internal class SceneIMG : IParameter
    {
        public float width { get; set; }
        public float height { get; set; }
        public ImageFormat format { get; set; }
        public string outputFile { get; set; }
        public CameraMode cameraMode { get; set;}
        public Scene scene { get; set; }

    }
}
