# ⛧ NNA - Node Name Abuse ⛧
Extend any 3d format by abusing node-names.\
This works by naming nodes in a specific way, and by serializing JSON into node-names.

This project is an abomination and the sooner it can burn in a fire, the better.

⭐ Star this repo if you love heresy! ⭐

![](./Docs/img/nna-example.png)

Issues, discussions & PRs Welcome!

## Why
Existing 3d interchange formats are bad. The least horrible one, FBX, is not extensible.

This is a way to add additional information to 3d models in any format, primarily FBX.
This Unity AssetPostprocessor will parse and convert information serialized into node-names into the appropriate Unity constructs.

The goal is for a 3d file to be the single source of truth for all its functionality, and work across different game engines.

## How
For simpler definitions, information can be encoded into a node name directly.\
For more complex components, you can serialize JSON into an array of child-nodes.

On import into Unity, these definitions will be parsed by NNA's hot loadable processors.

**[Documantation on all provided NNA processors!](Docs/Processors.md)**

You can easily implement additional processors yourself!

### Name Processors
Implementations of [INameProcessor](./NNA/Runtime/Processors/IJsonProcessor.cs) try to match information contained in a regular node name.

**Syntax:** `Actual Node Name` `NNA Processor Name` `Optional Parameters` `Optional Symmetry Suffix`

*Example:* `UpperLegTwistHips0.5.R`

`UpperLeg` is the actual node name.
`Twist` is the NNA processor name.
`Hips` is a parameter for the processor.
`0.5` is a parameter for the processor.
`.R` signifies the right side of the model.

Assume the following hierarchy: `Hips` → `UpperLeg.R` → `UpperLegTwist.R`.\
`UpperLegTwist.R` is supposed to be a twist-bone, as in it has to copy part of the Y-axis rotation from another node.

On import into Unity, the `Twist` node-name processor will recognize the `UpperLegTwist.R` node, and create a `RotationConstraint` with the `Hips` as the source, since it is the grandparent, and a weight of 0.5.

In order to specify a different source-node and weight, name the node the following way: `LowerArmTwistHand.L0.66.L`.
This will find the node called `Hand.L` and assign a weight of 0.66.

### Json Processors
Implementations of [IJsonProcessor](./NNA/Runtime/Processors/IJsonProcessor.cs) parse Json from node names contained in a NNA specific subtree in the model's hierarchy.

In order to specify a component with JSON, create a node named `$nna` as the child of the root.
To this you parent targeting nodes.

These follow the following syntax: `$target:NodeName`

'NodeName' is the node in the hierarchy outside of `$nna` to which the components are to be applied.\
A special targeting node named `$root` can be specified to target the hierarchies true root node, whose name may not be known before export.

In order to specify a component with JSON, add one or more numbered child nodes to your targeting node.

Since to Blender allows only a maximum node-name length 61 bytes, NNA JSON definitions are split up into multiple node-names of child nodes.

Start each line/child-node with `$`, the line number, and another `$`. The remaining characters get filled with the raw JSON text. On import these lines will be combined.

The root of the JSON definition must be an array of objects. Each object has a `t` (meaning type) property, which is a string. Based on the objects `t` property, a processor will be matched.

Example hierarchy:\
`$nna`\
→ `$target:Hair01`\
→ → `$0$[{"t":"ava.secondary_motion","id":"0","intensity":0.4},{"t`\
→ → `$1$":"vrc.physbone", "overrides":["0"],"pull":0.15,"spring":0`\
→ → `$2$.3,"limit_type":"angle","max_angle":60}]`

Json Components can have optional `id` and `overrides` properties.\
An `id` is a string that must be unique within the model and can be used to reference other components.\
`overrides` is an array of ID's. The overridden components will not be processed.

### Global Processors
Implementations of [IGlobalProcessor](./NNA/Runtime/Processors/IGlobalProcessor.cs) will always run for a given context.

### Hot Loading
Register your own processors like this:

```
[InitializeOnLoad]
public class RegisterMyFancyrocessor {
	static RegisterMyFancyProcessor() {
		NNARegistry.RegisterJsonProcessor(new MyFancyJsonProcessor(), "vrchat_avatar3");
	}
}
```

### Import Context
All processors for NNA types are registered in a context.

Processors in the default context are always applied, unless another processor for the specified import context is registered.

For example, the `c-twist` type in the default context will always create a Unity `RotationConstraint` component.
If a Processor for `c-twist` was also registered with a `vrchat_avatar3` context, and the models import context is set to that, then the VRChat specific Processor will be chosen. It would create a VRChat Constraint component instead.

## Current Status
The structure is pretty much there, and specific functionality is being worked on.

### Stage 1: MVP
Parse decently featured VR avatars from `*.nna.fbx` files in Unity. Some resources like animator controllers or materials will be mapped by name from the Unity project.

The 'export from Blender, drag into Unity and get a ready to upload VR avatar' experience is generally there, but the file depends on resources in the Unity project.

#### TODO
* Material mappings by name from within the Unity project.
* More constraint types.
* More VR & V-tubing avatar components & features.
	* Bone based eyelid animation.
	* Bone physics libraries: VRC Physbones & VRM Springbones.
* A basic Blender addon to make defining these components easier.

### Stage 2: All In One
An `*.nna.fbx` file no longer needs external dependencies, other than NNA itself, in order to be parsed into a fully featured VR avatar.

A lot of these features are blocked by Blender's (lack of an) animation system. Will be fixed once Blender 4.4 is released with 'slotted actions'.

#### TODO
* More constraint types.
* Animations, including being able to target NNA properties. (Blocked until Blender 4.4)
* Implement [MTF](https://github.com/emperorofmars/stf-unity/tree/master/MTF) to fully encode shader & game engine agnostic materials.
* More VR & V-tubing avatar components & features.
	* Bone physics (ava.secondary_motion as universal fallback, VRC Physbones & colliders & contacts, VRM spring bones & colliders, maybe DynamicBones & MagickaCloth)
	* Automatic animator controller generation (Partially blocked until Blender 4.4)
		* Face Tracking
		* Hand gestures (additionally: fallback VRM blendshape pose nonsense)
		* Toggles
		* Joystick puppets
		* Custom application & game engine agnostic animation logic
* A fully featured Blender addon.

### Stage 3: Overlord
Go beyond what VR & V-Tubing applications support out of the box.

#### TODO
* Template system. (To apply user modifications. This would be the basis of a 'character editor' system, so end users could adapt their avatars easier.)
	* Gesture rebinding!
* Addon system. (To apply a piece of clothing from a separate file to a base body for example. This should be deeply integrated with the avatar components and the template system.)
* IDK, suggest me more!

---

I am disgusted by myself for making this.

Cheers!

Or something.
