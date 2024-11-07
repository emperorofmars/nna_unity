
## Unity Implementation Documentation
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
public class RegisterMyFancyProcessor {
	static RegisterMyFancyProcessor() {
		NNARegistry.RegisterJsonProcessor(new MyFancyProcessor(), "vrchat_avatar3");
	}
}
```

### Import Context
All processors for NNA types are registered in a context.

Processors in the default context are always applied, unless another processor for the specified import context is registered.

For example, the `c-twist` type in the default context will always create a Unity `RotationConstraint` component.
If a Processor for `c-twist` was also registered with a `vrchat_avatar3` context, and the models import context is set to that, then the VRChat specific Processor will be chosen. It would create a VRChat Constraint component instead.
