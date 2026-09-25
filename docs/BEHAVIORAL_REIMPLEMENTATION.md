# Behavioral Reimplementation Policy

GK2 Tweaks is developed as an independent, behavior-oriented reimplementation.

## What may be used as input

Development may use:

- observed runtime behavior
- player-facing configuration and defaults
- public mod metadata and documentation
- GK2 Mod Framework public APIs
- game assembly type/member signatures required to integrate with Graveyard Keeper 2
- runtime logs and test results

Reverse engineering may be used to determine interoperability facts and behavioral requirements, but decompiled source is not treated as implementation source.

## What must not be copied into this repository

Do not copy or mechanically translate:

- decompiled method bodies
- original source comments
- private implementation-specific identifiers when they are not required game/API names
- original class/file organization
- original control-flow structure merely because it was observed
- embedded text or data that is not necessary for compatibility

Game and Framework type/member names are used where required because they are interoperability contracts.

## Implementation approach

For each feature:

1. Write down the behavior the feature must provide.
2. Identify the smallest verified game/Framework API contract needed for that behavior.
3. Design the GK2 Tweaks module around this repository's architecture.
4. Implement from the behavior specification rather than translating an observed implementation.
5. Add structural compatibility checks for game members the feature depends on.
6. Runtime-test the feature against the current game build.
7. Keep feature-specific logic isolated so it can be removed or replaced without affecting unrelated modules.

Shared behavior should be factored into GK2 Tweaks-owned infrastructure rather than duplicated to match another mod's organization.

## Provenance notes

A pull request that reimplements an existing mod should describe:

- the behavior being reproduced
- the game APIs/contracts verified
- which behavior was confirmed by runtime/decompilation investigation
- any assumptions still requiring runtime testing

The repository should not contain third-party mod binaries, decompiled game source, or decompiled third-party source.
