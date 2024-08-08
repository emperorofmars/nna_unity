# ⛧ NNA - Node Name Abuse ⛧
Extend any 3d format by adding information to node-names.

This project is an abomination and the sooner it can burn in a fire, the better.

## Purpose
Existing 3d interchange formats are bad. The least horrible one, fbx, is not extensible.

This is a way to add additional information to 3d models in any format, including fbx, into node-names.
This Unity AssetPostprocessor will parse and convert information serialized into node-names into the appropriate Unity constructs.

Since to my knowledge Blender allows only a maximum node-name length 61 bytes, NNA definitions can be split up into multiple node-names of child nodes.

## Current status
I just started making this, but generally this is how it will work.

More NNA types will be added, like humanoid definitions, perhaps comprehensive material mappings and components for VR avatars.

Support for additional types can be hot loaded.

## How it works
In order to define a `twist-bone` as such, name it the following way:
```
LowerArmTwist.L$nna:twist-bone:weight:0.6
```
The actual node name is `LowerArmTwist.L` and the NNA component type is `twist-bone`.
The NNA definition, starting with `$nna` will be removed from the node-name. If the node-name consists only of the NNA definition, the entire node will be deleted after processing.

The Parser for the type `twist-bone` will create a `RotationConstraint` and set the source weight to 0.6.
Since no explicit source is specified, it will take the parent of the parent as the source.

If the source was to be specified, the character limit for one node would be very likely reached.
In that case the node could be named the following way:
```
LowerArmTwist.L$nna:$multinode
```

In that case the node has to have one or more childnodes whose names have to consist purely of NNA definitions, each starting with a line number.
The child-node(s) could be named the following way:
```
$$01$nna:twist-bone:weight:0.66,target:$ref:../Hand.L
```
I have no clue if the node-order is guaranteed to be preserved across various file-formats and their various implementations. The default line-number length is 2. Should the definition exceed 99 lines/child-nodes, then the NNA node can be defined as follows:
```
LowerArmTwist.L$nna:$multinode:3
```
All child-nodes of a `$multinode` will be removed after processing.

## Example
An example of a model-hirachy using NNA In Blender could look like this:
![](./Docs/img/blender_armature_hirarchy.png)

The `$nna:twist-bone` definition will be converted in Unity into a RotationConstraint:
![](./Docs/img/unity_twist-bone_component.png)

If you wanted to define a `multinode` component in Blender, you would have to create one or more `Empty` object and parent them to the appropriate bone. This is clunky, perhaps I will create a Blender counterpart to this eventually.
