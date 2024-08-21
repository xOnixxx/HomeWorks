# Technical Documentation
The architecture of the program is straightforward. The camera generates a ray (or multiple in case of A-A) for each pixel of the future image. The ray is then transformed by the various transform matrices for each solid and calculates the color depending on the shading and other enviromental settings.

## JsonParser
### GetComp\<T>(string fileName)
Used to read and parse the Config data into array of \<T> objects from the file.

## SolidHierarchy
Used to contain the solids in the scene in tree structure. Contains enumerator for ease of access to traversing the structure. 
### AssertTransforms()
Traverses the tree and creates transformation matrix for each of the solids

## Controller
This should be the bridge between the hard backend of the application and possible future frontend. Methods in this class should be simple and straightforward.

## Transformations
The transformation matrices are implemented using interface ITransformations for general purpose. BasicTransforms abstract class servers for my specific purpose of transforming the rays and calculating inverse. I have decided to make this split for if other users would implement their own math they can create their own abstract class with specific calculations. 
For the purpose of Json parser all instances of ITransformations should have 1 constructor tagged with \[JsonConstructor] and one empty constructor.

## Solids
The core of the project are the solids. The ISolids interface provides a general blueprint for adding more geometry. In implementing new solid the GetIntersection method should be prioritized with optimalization as it is one of the bottlenecks for calculating the image. 
The Solids are currently divided into 2 categories, the simple ones which directly inherent the ISolids interface and the ComplexSolid3D.
### Simple solid
In adding simple solid the user has to implement all the methods from ISolids.
### Complex solid 
The ComplexSolid3D consists of multiple simple solids. The main thing to take care of is correctly defining the positions of the primitives. Complex solid can also contain other complex solids as part of itself, for that it is mandatory to create custom constructor.

As in the case of Transformations the ISolids need to have one empty constructor and onle tagged as \[JsonConstructor] for Json support.

## Light
The ILight contains jsonORIGIN for Json compatibility because Json doesnt support Vector3d as an storable object. The interface exists for future extensions which use directional light etc.  

## Camera
### FloatImage RenderScene(Scene scene, Func\<Vector2d, int, list\<Vector2d>> sampler, int spp = 4)
This method renders the scene and uses sampling method <b>sampler</b> for sampling in the anti-aliasing. The size of the sampling is defined with <b>spp</b> (samples per pixel). This has grand impact on the performance of the program. If the Camera doesn't hit any solid the background colour should be returned.

## RayTracer
### RayTracing
Gets the color for the ray generated either by the Camera or another ray from RayTracer (reflection/refraction). It has recursive call for the <i>Ray Tracing</i>. If in any of the steps during RayTracing the Ray missed all solids it should return the background colour.
In future the shading method can be passed to the shader for more extensibility. 
### CheckShadow 
Returns a multiplier for the shadow. In the case of translucent materials the shadow is not "100%" strong.
### GetRefraction
Gets the color for refracted ray using the fresnel formula.

## Scene
Servers as the container for the scene.

## MathHelp
This is where all the math calculations are implemented. Contains the different sampling methods.