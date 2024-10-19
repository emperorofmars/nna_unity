

# Default Components

## `c-twist` **Twist Constraint**
| parameter | type | description | default |
| --- | --- | --- | --- |
| `w` | float | Weight of the source | `0.5` |
| `tp` | string/path | Path to source node | `../..` |

Creates a `RotationConstraint` component with one source limited to the Y axis.

## `humanoid` **Humanoid Mappings**
| parameter | type | description | default |
| --- | --- | --- | --- |
| `lt` | string | Locomotion type. Supported values are `planti` & `digi`. | `planti` |

Generates a humanoid `Avatar`. Creates an `Animator` component on the root node if not present and sets the avatar.
Currently, only automatic mapping of bones is supported. Explicit mapping may be added in the future.

# Avatar Components from the AVA subproject
Supported target applications:
* VRChat SDK3: `vrchat_avatar3`

## `ava.avatar` **Base Avatar Definition**
Base component for avatars.

## `ava.viewport` **Avatar Viewport**
This nodes position will be set as the avatar's viewport position.

## `ava.eyetracking` **Avatar Eyetracking Information**
| parameter | type | description | default |
| --- | --- | --- | --- |
| `lkt` | string | Eyelook Type (`r`: Rotations, `b`: Blendshapes) | `r` |
| `ldt` | string | Eyelid Type (`r`: Rotations, `b`: Blendshapes) | `b` |
| `m` | string/path | Path to the skinned mesh which contains the target blendshapes | `Body` |
| `u` | float | Up angle | `15` |
| `d` | float | Down angle | `12` |
| `i` | float | Inner angle | `15` |
| `o` | float | Outer angle | `16` |

For now blendshapes are matched automatically based on naming. It should be possible to override that in the future.