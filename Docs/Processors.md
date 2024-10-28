

# Default NNA Processors

## `c-twist` **Twist Constraint**
Creates a `RotationConstraint` component with one source limited to the Y axis.

### Name
| name part | type | description | default |
| --- | --- | --- | --- |
| twist | string | processor name | - |
| source | string | Source node | grandparent of the twist node |
| weight | float | Weight of the sourcee | `0.5` |

**Example**
`LowerArmTwistHand.L0.66.L`

### Json
| parameter | type | description | default |
| --- | --- | --- | --- |
| `w` | float | Weight of the source | `0.5` |
| `s` | string/path | Source node path | grandparent of the twist node |

**Example**
`{"t":"c-twist","w":0.66,"s":"Handl.L"}`

## `humanoid` **Humanoid Mappings**
Generates a humanoid `Avatar`. Creates an `Animator` component on the root node if not present and sets the avatar.
Currently, only automatic mapping of bones is supported. Explicit mapping may be added in the future.

### Name
| name part | type | description | default |
| --- | --- | --- | --- |
| humanoid | string | processor name | - |
| locomotion type | string | Locomotion type. Supported values are `planti` & `digi`. | `planti` |
| no jaw | float | Option to not map the jaw. Supported value is `NoJaw` | - |

**Example**
`ArmatureHumanoidDigiNoJaw`

### Json
| parameter | type | description | default |
| --- | --- | --- | --- |
| `lt` | string | Locomotion type. Supported values are `planti` & `digi`. | `planti` |
| `nj` | boolean | Option to not map the jaw. Supported values are `nj` & `no_jaw`. | `false` |

# Avatar Components from the AVA subproject
Supported target applications:
| Target | NNA Context |
| --- | --- |
| VRChat SDK3 Avatar | `vrchat_avatar3` |
| UniVRM0 | `vrm0` |

**Example**
`{"t":"humanoid","lt":"digi","nj":true}`

## `ava.avatar` **Base Avatar Definition**
Base component for VR & V-Tubing avatars.

It is usually implemented as a global component for its respective implementation. As such an `ava.avatar` component must not be necessarily present. Only if you need to specify any properties.



