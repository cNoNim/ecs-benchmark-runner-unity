# Entity-Component-System Benchmarks for Unity

[![License](https://img.shields.io/github/license/cNoNim/ecs-benchmark-runner-unity)](https://github.com/cNoNim/ecs-benchmark-runner-unity?tab=MIT-1-ov-file#readme)
[![Stars](https://img.shields.io/github/stars/cNoNim/ecs-benchmark-runner-unity?color=brightgreen)](https://github.com/cNoNim/ecs-benchmark-runner-unity/stargazers)

This repository contains a collection of benchmarks for Unity Entity-Component-System (ECS) frameworks.
Benchmarks perform a complex performance comparison of ECS frameworks on a near-real-world scenario.

[**.NET Version**](https://github.com/cNoNim/ecs-benchmark-runner-dotnet)

### Frameworks:

|                                                               ECS | Version / Source                                                                                 | Implemented |
|------------------------------------------------------------------:|:-------------------------------------------------------------------------------------------------|:-----------:|
|              [DragonECS](https://github.com/DCFApixels/DragonECS) | [0.9.21](https://github.com/DCFApixels/DragonECS/releases/tag/0.9.21)                            |      ✅      |
| [Friflo Engine ECS](https://github.com/friflo/Friflo.Engine.ECS) | local precompiled assemblies in `Assets/lib/netstandard2.1` (`Friflo.Engine.ECS` `3.5.0`)       |      ✅      |
|             [LeoECSLite](https://github.com/Leopotam/ecslite)     | [2025.4.22](https://github.com/Leopotam/ecslite/releases/tag/2025.4.22)                          |      ✅      |
|        [Massive ECS](https://github.com/nilpunch/massive-ecs)     | [v20.1.1](https://github.com/nilpunch/massive-ecs/tree/v20.1.1)                                  |      ✅      |
|                      [Morpeh](https://github.com/scellecs/morpeh) | [2024.1.1](https://github.com/scellecs/morpeh/releases/tag/2024.1.1)                             |      ✅      |
| [StaticEcs](https://github.com/Felid-Force-Studios/StaticEcs)     | [v2.0.3](https://github.com/Felid-Force-Studios/StaticEcs/releases/tag/v2.0.3)                   |      ✅      |
| [Unity Entities](https://docs.unity3d.com/Packages/com.unity.entities@6.4/manual/index.html) | `com.unity.entities` `6.4.0` |      ✅      |

## Scenario

All benchmark implementations run the same deterministic simulation.

At startup, the benchmark creates `N` units. During spawn, each unit receives a type (`NPC`, `Hero`, or `Monster`), combat stats, position, and movement state. Each tick then runs the same gameplay pipeline: spawning and respawning, target selection, attack creation, damage application, movement, velocity updates, and rendering into a framebuffer.

This makes the benchmark closer to a small RTS-like combat loop than to a synthetic ECS microbenchmark.

## Tick Pipeline

Each benchmark context executes the same logical steps:

1. Spawn / Respawn / Kill
2. Render current state into framebuffer
3. Update unit sprite state
4. Apply damage from pending attacks
5. Create new attacks
6. Move units
7. Update velocity
8. Advance simulation data

## Validation

Benchmark implementations are validated by shared tests.
For the same entity count and tick count, all contexts must produce the same framebuffer hash. This ensures the comparison is based on equivalent simulation results, not on different behavior.

## Running

0. Install [Unity Hub](https://unity.com/download) and Unity `6000.4.1f1`
1. Clone repository
   ```sh
   git clone https://github.com/cNoNim/ecs-benchmark-runner-unity.git
   ```
2. Open the project in Unity
3. Run tests from Unity Test Runner
4. Run benchmark scene in Play Mode

   Scene
   ```text
   Assets/Benchmark.unity
   ```

   Results
   ```text
   Application.persistentDataPath/Benchmark.md
   ```

5. Build from batch mode

   ```sh
   /path/to/Unity/Editor/Unity \
     -batchmode \
     -quit \
     -projectPath /path/to/ecs-benchmark-runner-unity \
     -buildTarget Android \
     -executeMethod Benchmark.Editor.BenchmarkBuild.BuildBatchMode
   ```

## Structure

The benchmark is divided into repositories.
Current repository integrates benchmark packages into runner scene and Unity test assemblies.

### [Benchmark.Core Package](https://github.com/cNoNim/ecs-benchmark-core)

A separate repository contains common assemblies that are used by benchmarks or by infrastructure.
Integration is done through the `ecs.benchmark.core` package.

### Benchmark Packages

Each benchmark is a separate repository, integration is done through packages referenced in [Packages/manifest.json](Packages/manifest.json).

|                                                                  Repository | Package / Assembly        |
|----------------------------------------------------------------------------:|:--------------------------|
| [Dragon ECS](https://github.com/cNoNim/ecs-benchmark-dragonecs)             | `ecs.benchmark.dragonecs` |
| [FriFlo ECS](https://github.com/cNoNim/ecs-benchmark-frifloecs)             | `ecs.benchmark.frifloecs` |
| [LeoEcsLite](https://github.com/cNoNim/ecs-benchmark-ecslite)               | `ecs.benchmark.ecslite`   |
| [Massive ECS](https://github.com/cNoNim/ecs-benchmark-massiveecs)           | `ecs.benchmark.massiveecs` |
| [Morpeh](https://github.com/cNoNim/ecs-benchmark-morpeh)                    | `ecs.benchmark.morpeh`    |
| [StaticEcs](https://github.com/cNoNim/ecs-benchmark-staticecs)              | `ecs.benchmark.staticecs` |
| [Unity Entities](https://github.com/cNoNim/ecs-benchmark-entities)          | `ecs.benchmark.entities`  |

#### Dependencies

Frameworks can be referenced:
* as Unity packages in [Packages/manifest.json](Packages/manifest.json);
* as local precompiled assemblies in [Assets/lib](Assets/lib).

### [Benchmark.Runner](Assets/Benchmark.cs)

Runner scene includes all benchmarks from referenced runtime assemblies.
Active runtime references are declared in [Assets/Benchmark.Runner.asmdef](Assets/Benchmark.Runner.asmdef).

### [Performance Tests](Assets/PerformanceTests/Benchmark.cs)

Contains Unity performance test runner project for benchmark contexts.

### [Hash Tests](Assets/Tests/HashTests.cs)

Contains test project that validates that each benchmark produces the same state.
