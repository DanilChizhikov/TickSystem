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
      "com.danilchizhikov.ticksystem": "https://github.com/DanilChizhikov/TickSystem.git?path=Assets/TickSystem#0.2.0",
      ```
UPM should now install the package.

## Project Structure

### Interfaces

Use these interfaces to implement various ticking behaviors for different game elements and systems within your Unity project.

1. ITickService - interface provides a way to add a tickable element and returns a disposable object that can be used to control the added tickable.
```csharp
public interface ITickService
{
    IDisposable AddFixTick(IFixTickable value, int order = int.MaxValue);
    IDisposable AddTick(ITickable value, int order = int.MaxValue);
    IDisposable AddLateTick(ILateTickable value, int order = int.MaxValue);
}
```

2. ITickController - interface defines the contract for a tick controller, responsible for managing and processing tickable elements.
```csharp
public interface ITickController : IDisposable
{
    bool TryAdd(object owner, Action<float> tick, int order);
    bool TryRemove(Action<float> tick);
}
```

3. ITickable - interface extends the base tickable interface and adds a method for regular tick updates.
```csharp
public interface ITickable
{
    void Tick(float deltaTime);
}
```

4. IFixTickable - interface extends the base tickable interface and adds a method for fixed tick updates.
```csharp
public interface IFixTickable
{
    void FixTick(float deltaTime);
}
```

5. ILateTickable - interface extends the base tickable interface and adds a method for late tick updates.
```csharp
public interface ILateTickable
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
    private static ITickService _tickService;

    public static ITickService TickService => _tickService;

    private void Awake()
    {
        if (TickService != null)
        {
            Destroy(gameObject);
            return;
        }

        _tickService = new TickService();
        DontDestroyOnLoad(gameObject);
    }
}
```

### Implementations

In order to enable your object to be updated, it is enough to make it an implementation of one of the 3 interfaces or all at once.
```csharp
public sealed class TickableEntity : IFixTickable, ITickable, ILateTickable, IDisposable
{
    private readonly IDisposable _fixTickDisposable;
    private readonly IDisposable _tickDisposable;
    private readonly IDisposable _lateTickDisposable;
    
    public TickableEntity(ITickService tickService)
    {
        _fixTickDisposable = tickService.AddFixTick(this);
        _tickDisposable = tickService.AddTick(this);
        _lateTickDisposable = tickService.AddLateTick(this);
    }
    
    public void FixTick(float deltaTime)
    {
        Debug.Log($"{nameof(FixTick)}: {deltaTime}");
    }

    public void Tick(float deltaTime)
    {
        Debug.Log($"{nameof(Tick)}: {deltaTime}");
    }

    public void LateTick(float deltaTime)
    {
        Debug.Log($"{nameof(LateTick)}: {deltaTime}");
    }

    public void Dispose()
    {
        _fixTickDisposable?.Dispose();
        _tickDisposable?.Dispose();
        _lateTickDisposable?.Dispose();
    }
}
```

## License

MIT