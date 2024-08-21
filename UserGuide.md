

# What is Render#

Render# is photorealistic renderer built using C#. Its supports refractive and reflective surfaces, anti-aliasing. 

## Features
- Json compatible
- Supports solid hierachical scheme
- HDR output

## Usage
The program reads from 3 main config/json files. These are requiered for the run of the program.
### SolidsConfig:
* The scenes objects as well as their materials, represented in tree structure.
* Each node has to contain:
  + Transformation matrix
  + Types of solids on the node
* There are currently 6 supported transformations:  
  + Translate (x,y,z)
  + Rotate x/y/z (in degrees x/y/z)
  + Resize (x,y,z)
  + Shear (xy, xz, yz, yx, zx, zy)
* There are currently 5 supported solids (3 primitive, 2 complex)
  + Primitive: Triangle3D, Sphere3D, Plane3D
  + Complex: Square3D, Prism3D


![test image](./GuideSources/SolidConfig.png)

### LightsConfig:
* All the lights for the current scene

### CameraConfig:
* Position of the camera
* Target vector (direction of camera)
* Upguide vector (what is "up" for camera)
* Resolution and FOV for the camera

## Output
We support multiple cameras, each camera generates 1 output image.
The output images are stored in the /Output directory.


## Download
The dependencies should be installed automatically during the first run of the software. But in the case that it doesn't happen here are the NuGet packages for the Visual Studio.
### Dependencies:
-  Newtonsoft.Json v. 13.0
-  OpenTK 4.7

