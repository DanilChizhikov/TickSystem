# TickSystem
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This repository contains the definitions for the tick system interfaces,
which provide a flexible way to handle different types of ticking in Unity game development for various game elements and systems.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Project Structure](#Project-Structure)
    - [Interfaces](#Interfaces)
- [Basic Usage](#Basic-Usage)
    - [Initialize](#Initialize)
    - [Implementations](#Implementations)
    - [Tickable LifeTime](#Tickable-LifeTime)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/TickSystem/releases/) page.
2. Open TickSystem.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.ticksystem": "https://github.com/DanilChizhikov/TickSystem.git?path=Assets/TickSystem#0.0.1",
      ```
UPM should now install the package.

## Project Structure

### Interfaces

Use these interfaces to implement various ticking behaviors for different game elements and systems within your Unity project.

1. ITickService - interface provides a way to add a tickable element and returns a disposable object that can be used to control the added tickable.
```csharp
public interface ITickService
{
    IDisposable AddTick(IBaseTickable value);
}
```

2. ITickController - interface defines the contract for a tick controller, responsible for managing and processing tickable elements.
```csharp
public interface ITickController
{
    Type ServicedTickType { get; }

    bool Add(IBaseTickable value);
    void Processing();
    void Remove(IBaseTickable value);
}
```

3. IBaseTickable - interface serves as the base for all tickable interfaces, providing a default tick order of 0.
```csharp
public interface IBaseTickable
{
    uint TickOrder => 0;
}
```

4. ITickable - interface extends the base tickable interface and adds a method for regular tick updates.
```csharp
public interface ITickable : IBaseTickable
{
    void Tick(float deltaTime);
}
```

5. IFixTickable - interface extends the base tickable interface and adds a method for fixed tick updates.
```csharp
public interface IFixTickable : IBaseTickable
{
    void FixTick(float deltaTime);
}
```

6. ILateTickable - interface extends the base tickable interface and adds a method for late tick updates.
```csharp
public interface ILateTickable : IBaseTickable
{
    void LateTick(float deltaTime);
}
```

## Basic Usage

### Initialize

First, you need to initialize the TickSystem, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public sealed class TickSystemBootstrap : MonoBehaviour
{
    private static ITickService _service;

    public static ITickService Service => _service;

    private void Awake()
    {
        if (Service != null)
        {
            Destroy(gameObject);
            return;
        }

        _service = new TickService();
    }
}
```

### Implementations

In order to enable your object to be updated, it is enough to make it an implementation of one of the 3 interfaces or all at once.
```csharp
internal sealed class Example : ITickable, IFixTickable, ILateTickable
{
    public void Tick(float deltaTime)
    {
        // some code...
    }

    public void FixTick(float deltaTime)
    {
        // some code...
    }

    public void LateTick(float deltaTime)
    {
        // some code...
    }
}
```

### Tickable LifeTime

After you have implemented the necessary interface, you need to add your object to the system via ITickService.
Which in turn will return you IDisposable, which will allow you to delete the object from the system in the future by calling the Dispose method.
```csharp
internal sealed class ExampleTickableLifeTime : IDisposable
{
    private readonly IDisposable _exampleDispose;
    
    public ExampleTickableLifeTime(ITickService tickService, Example example)
    {
        _exampleDispose = tickService.AddTick(example);
    }

    public void Dispose()
    {
        _exampleDispose?.Dispose();
    }
}
```

## License

MIT