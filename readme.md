# NNA - Node Name Abuse
Extend any 3d format by adding information to node-names.

This project is an abomination and the sooner it can burn in a fire, the better.

## Purpose
Existing 3d interchange formats are bad. The least horrible one, fbx, is not extensible.

This is a way to add additional information to 3d models in any format, including fbx, into node-names.
This Unity AssetPostprocessor will parse and convert information serialized into node-names into the appropriate Unity constructs.

## Example
In order to define a twist-bone as such, name it the following way:
```
LowerArmTwist.L$nna:twist:weight=0.6
```



