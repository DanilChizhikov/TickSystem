# TickSystem
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This repository contains the definitions for the tick system interfaces,
which provide a flexible way to handle different types of ticking in Unity game development for various game elements and systems.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Features](#Features)
- [API Reference](#api-reference)
    - [Core Interfaces](#core-interfaces)
    - [Base Classes](#base-classes)
    - [Concrete Implementations](#concrete-implementations)
    - [Usage Example](#usage-example)
    - [Performance Considerations](#performance-considerations)
    - [Best Practices](#best-practices)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/TickSystem/releases/) page.
2. Open com.dtech.ticksystem.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add the following line to the dependencies section:
    - ```json
      "com.dtech.ticksystem": "https://github.com/DanilChizhikov/TickSystem.git",
      ```
3. Unity will automatically import the package.

If you want to set a target version, TickSystem uses the `v*.*.*` release tag so you can specify a version like #v2.0.0.

For example `https://github.com/DanilChizhikov/TickSystem.git#v2.0.0`.

## Features

- **Multiple Update Types**: Supports standard Update, FixedUpdate, and LateUpdate callbacks
- **Ordered Execution**: Tick methods can be ordered using the `[TickOrder]` attribute
- **Efficient Management**: Optimized for high-frequency updates with minimal overhead
- **Memory Safe**: Automatic cleanup of tick handlers when components are destroyed
- **Profiler Integration**: Built-in profiler markers for performance monitoring

## API Reference

### Core Interfaces

- **ITickable**: Interface for standard Update tick callbacks
  ```csharp
  public interface ITickable
  {
      void Tick(float deltaTime);
  }
  ```

- **IFixTickable**: Interface for FixedUpdate tick callbacks
  ```csharp
  public interface IFixTickable
  {
      void FixTick(float deltaTime);
  }
  ```

- **ILateTickable**: Interface for LateUpdate tick callbacks
  ```csharp
  public interface ILateTickable
  {
      void LateTick(float deltaTime);
  }
  ```

- **ITickService**: Main service interface for managing tick subscriptions
  ```csharp
  public interface ITickService
  {
      IDisposable AddFixTick(IFixTickable value);
      IDisposable AddTick(ITickable value);
      IDisposable AddLateTick(ILateTickable value);
  }
  ```

### Base Classes

- **TickController< TUpdate >**: Abstract base class for managing tick execution
    - Manages tick registration and execution order
    - Handles add/remove operations in a thread-safe manner
    - Integrates with Unity's PlayerLoop system

- **TickItem**: Internal class representing a single tick registration
    - Contains order, owner reference, and tick action
    - Used for sorting and execution

### Concrete Implementations

- **TickService**: Default implementation of ITickService
    - Manages all tick types (Update, FixedUpdate, LateUpdate)
    - Handles tick handler lifecycle

- **UpdateTickController**: Handles standard Update ticks
- **FixedUpdateTickController**: Handles physics FixedUpdate ticks
- **LateUpdateTickController**: Handles LateUpdate ticks

- **TickOrderAttribute**: Attribute to specify execution order of tick methods
  ```csharp
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class TickOrderAttribute : Attribute
  {
      public int Value { get; }
      public TickOrderAttribute(int order) => Value = order;
  }
  ```

## Usage Example

1. **Implement the appropriate interface(s) on your MonoBehaviour**

```csharp
using DTech.TickSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour, ITickable, IFixTickable
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 100f;
    
    private Rigidbody _rigidbody;
    private float _rotation;
    
    private IDisposable _tickSubscription;
    private IDisposable _fixTickSubscription;
    
    public void Initialize(ITickService tickService)
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        // Register with the tick system
        _tickSubscription = tickService.AddTick(this);
        _fixTickSubscription = tickService.AddFixTick(this);
    }
    
    // Called every frame
    public void Tick(float deltaTime)
    {
        // Handle input and other game logic
        float moveInput = Input.GetAxis("Vertical");
        _rotation = Input.GetAxis("Horizontal");
        
        // Move forward/backward
        transform.Translate(Vector3.forward * (moveInput * _moveSpeed * deltaTime));
    }
    
    // Called every physics frame
    public void FixTick(float deltaTime)
    {
        // Apply physics-based rotation
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * (_rotation * _rotationSpeed * deltaTime));
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
    
    private void OnDestroy()
    {
        _tickSubscription?.Dispose();
        _fixTickSubscription?.Dispose();
    }
}
```

2. **Using TickOrder to control execution order**

```csharp
[TickOrder(100)] // Will execute after default (int.MaxValue) and lower values
public class LateUpdater : MonoBehaviour, ITickable
{
    public void Tick(float deltaTime)
    {
        // This will execute after other tick methods with higher (or default) order
    }
}

[TickOrder(-100)] // Will execute before default order
public class EarlyUpdater : MonoBehaviour, ITickable
{
    public void Tick(float deltaTime)
    {
        // This will execute before other tick methods with lower (or default) order
    }
}
```

3. **Manually managing tick subscriptions**

```csharp
public class AdvancedController : MonoBehaviour
{
    private IDisposable _tickSubscription;
    private IDisposable _lateTickSubscription;
    
    private void OnEnable()
    {
        var tickService = new TickService();
        _tickSubscription = tickService.AddTick(new TickHandler(TickUpdate));
        _lateTickSubscription = tickService.AddLateTick(new LateTickHandler(LateTickUpdate));
    }
    
    private void OnDisable()
    {
        _tickSubscription?.Dispose();
        _lateTickSubscription?.Dispose();
    }
    
    private void TickUpdate(float deltaTime)
    {
        // Update logic here
    }
    
    private void LateTickUpdate(float deltaTime)
    {
        // Late update logic here
    }
    
    // Handler classes for manual subscription
    private class TickHandler : ITickable
    {
        private readonly Action _tickAction;
        
        public TickHandler(Action tickAction) => _tickAction = tickAction;
        public void Tick(float deltaTime) => _tickAction?.Invoke(deltaTime);
    }
    
    private class LateTickHandler : ILateTickable
    {
        private readonly Action _tickAction;
        
        public LateTickHandler(Action tickAction) => _tickAction = tickAction;
        public void LateTick(float deltaTime) => _tickAction?.Invoke(deltaTime);
    }
}
```

## Performance Considerations

- The system batches add/remove operations to minimize performance impact during frame execution
- Tick methods are sorted only when necessary (when tick collection changes)
- Profiler markers are included in development builds to help identify performance bottlenecks
- Consider using the `[TickOrder]` attribute to optimize execution order and reduce cache misses

## Best Practices

1. **Use the appropriate tick type**
    - Use `ITickable` for general game logic and input handling
    - Use `IFixTickable` for physics-related updates
    - Use `ILateTickable` for camera follow and other post-update logic

2. **Manage subscriptions carefully**
    - Always dispose of tick subscriptions when they're no longer needed
    - Consider using a dependency injection framework to manage the `ITickService` lifetime

3. **Optimize tick methods**
    - Keep tick methods lean and fast
    - Consider using a coroutine or async/await for infrequent operations
    - Use the profiler to identify and optimize performance hotspots

4. **Error handling**
    - Implement proper error handling in your tick methods to prevent one error from breaking the entire tick system

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.