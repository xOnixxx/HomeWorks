using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Scene
    {
        public ISolids[] scene;
        public ILight[] lights;
        public double diffuseC;
        public double specularC;
        private void Normalize(Matrix4d transMatrix)
        {

        }

    }
}
