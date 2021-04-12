<h3 align="center">
  <br>
  <img src="https://i.imgur.com/F3iiMvI.gif" alt="PathfinderMG in action" width="800">
  <br>
</h3>

<h1 align="center">PathfinderMG - Visualizing Tool [C# MonoGame]</h1>

## Table of Contents

- [Description](#description)
- [A* Pathfinding](#a-pathfinding)
- [Download](#download)
    - [Windows/Linux](#windowslinux)
    - [macOS](#macOS)
- [Building the project](#building-the-project)
- [License](#license)

## Description

<b>PathfinderMG</b> is a cross-platform pathfinding visualizer written in *C#* (.NET Core 3.1) using *MonoGame* framework. The project uses OpenGL for rendering, which enables easy porting.

The pathfinder uses **A\* algorithm** to find the shortest path possible, and visualizes the progress by running the algorithm asynchronously, while pausing it for short amounts of time at certain points. The amount of time is customizable from the UI. <br>
The project might include other pathfinding algorithms in the future (i.e. general Dijkstra or AI assisted)

As of now (*v0.1*), it's bundled with 7 preloaded maps (called scenarios) to play around with. Scenario editor is planned to release in the coming days.

Second most important feature of the application is **Scenario editor**, which enables the user to easily create custom scenarios, by adjusting the grid size and quickly placing obstacles for the pathfinder. I marked it as an absolute prerequisite for the project to leave the pre-release status.

## A* Pathfinding

For the time being, *PathfinderMG* implements an A* algorithm to find the fastest route between two points.

A* Pathfinding (A Star) is an algorithm that tries to find the fastest possible route between a starting node and a target node. It is commonly used in video games, as well as other fields.

The principle by which the algorithm works can be described as follows:
* it operates on two data structures, let's call them *open list* and *closed list*
    * the lists are used to mark nodes for evaluation
* starting with the Start Node, it finds its neighbours
* it evaluates nodes based on their distance: 
    * *from the starting node* - G Cost
    * *from the target node* - H Cost
* it moves towards the nodes with the lowest F Cost (=gCost+hCost)
* it tracks the parenting nodes that are part of the shortest path
* upon reaching the target, it moves backwards using parent references to find its way back to the start

Complexity of the algorithm depends on the scenario.

<img src="https://i.imgur.com/qDXP28M.gif" alt="node costs" width="400">

## Download

Compiled binaries are currently provided for Linux (x64) and Windows (x86/x64).

### Windows/Linux

The latest pre-release version can be downloaded from [here](https://github.com/sskorka/PathfinderMG/releases), or simply check GitHub releases for the newest version and changelog.

You need *.NET Core 3.1 Runtime* (or SDK) installed on your machine in order to run this. <br>
If you don't have it installed yet, there are **2 ways** to proceed:
* When attempting to run the executable, an error dialog should pop up that leads to the download of the necessary libraries:
<a><img src="https://i.imgur.com/TVURt9P.png" alt="install dialog" width="300"></a>
* Download the runtime manually from Microsoft:
    * [.NET Runtime 3.1.13](https://dotnet.microsoft.com/download/dotnet/3.1)

### macOS

The macOS binaries are not provided for the time being. You can build them by yourself using one of the following methods:
* Using .NET Core CLI:
    * Run ``` dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained ```
* Using Visual Studio:
    * Publish while targetting osx-x64

You can find more information regarding building for macOS in [MonoGame Documentation](https://docs.monogame.net/articles/packaging_games.html#build-and-packaging-for-macos).

***Important!*** <br>
The project has not been tested on OSX systems and might behave unpredictably or even crash.

## Building the project

You need **Visual Studio** or **Visual Studio Code** in order to build the project. There are several more steps involved in order to build a MonoGame project - please follow the instructions from the official [MonoGame docs](https://docs.monogame.net/articles/getting_started/0_getting_started.html) to set up your environment properly.

## License

[MIT](https://github.com/sskorka/PathfinderMG/blob/master/LICENSE.MD)
