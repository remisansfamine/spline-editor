# Roller Coaspline - **Unity/C#**
### ISART DIGITAL GP3, School Project: *Rémi GINER*  
<br>

<div style="text-align:center">

![RollerCoaster](Annexes/RollerCoaster.gif)

</div>

<!-- ABOUT THE PROJECT -->
# About The Project 
**Built with Unity 2021.3.5f1**

The goal of this project is to develop a tool to create, edit and use Splines curves. This tool must then be used to apply curves in a "Video Game" context of our choice. I made the choice to develop a procedural generator of roller coaster track.

# Table of contents
1. [Features](#features)
2. [Details](#details)
    - [Technical Implementation](#technical-implementation)
    - [Gameplay Implementation](#gameplay-implementation)
3. [In the future](#itf)
4. [References](#references)
5. [Versionning](#versionning)
6. [Autors](#authors)


# Features
- Hermite spline
- Bezier spline
- B-spline
- Catmull-Rom spline
- Point insertion
- Position evalution from polynomial and matrix
- Tangent evalution from polynomial and matrix
- Procedural generation from spline

# Details
## Technical Implementation

### **Spline Controller**

To have a proper control on the Spline, I had to create a Controller that gathers information on the points collects to be able to evaluate the requested information according to the input formula.

<div style="text-align:center">

![SplineController](Annexes/SplineController.png)

</div>

The Spline Controller also allows to give a spatial representation to the Spline to be calculated. When the checkbox "isTransformBounded" is checked, the Spline is calculated in the local frame of the GameObject, otherwise it is calculated in WorldSpace.

<div style="text-align:center">

![ControllerPositionEvaluation](Annexes/ControllerPositionEvaluation.png)

</div>

The Spline Controller also allows to store cached information like CumutativeDistances which allows to make the Spline more regular. This works on the principle of remapping the ``u`` value entered in the evaluation functions from pre-calculated distances.

Irregular spline|Regular spline
:-:|:-:
![](Annexes/IrregularSpline.png)|![](Annexes/RegularSpline.png)

### **Spline Descriptor**

Hermite|Bezier|B-Spline|Catmull-Rom
:-:|:-:|:-:|:-:
![](Annexes/Hermite.png)|![](Annexes/Bezier.png)|![](Annexes/BSpline.png)|![](Annexes/CatmullRom.png)

## Gameplay Implementation

## In the future:
In the future, all this code will surely be ported to C++ on an house-made engine. The calculations to obtain the acceleration, the jerk/jolt at a time t, the patch will surely be implemented as well

## References:
General references:
- https://en.wikipedia.org/wiki/Spline_(mathematics)
- https://www.youtube.com/watch?v=jvPPXbo87ds

## Versionning
Git Lab for the versioning.

# Authors
* **Rémi GINER**