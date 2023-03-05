using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    internal struct Scene
    {
        private ISolids[] scene;
        private ImageFormat format;
        public readonly int width;
        public readonly int height;
    }
}
