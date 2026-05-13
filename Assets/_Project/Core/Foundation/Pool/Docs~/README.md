# Pool System - User Guide

## Table of Contents
1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Core Concepts](#core-concepts)
4. [Usage Examples](#usage-examples)
5. [Advanced Features](#advanced-features)
6. [Best Practices](#best-practices)
7. [Troubleshooting](#troubleshooting)

---

## Overview

The Pool System provides efficient object reuse to minimize garbage collection and improve performance in Unity projects. It supports both MonoBehaviour pooling (for GameObjects) and plain C# class pooling.

### Key Features
- ✅ MonoBehaviour pooling for GameObjects
- ✅ C# class pooling for non-Unity objects
- ✅ Centralized pool management
- ✅ Statistics tracking
- ✅ Debug mode with collection checking
- ✅ Lifecycle callbacks (OnSpawn, OnDespawn, OnDestroyItem)

---

## Quick Start

### Step 1: Make Your Class Poolable

```csharp
using Core.Foundation.Pool;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawn()
    {
        // Called when object is taken from pool
        // Reset your object state here
        transform.position = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        // Called when object is returned to pool
        // Clean up state here
        gameObject.SetActive(false);
    }

    public void OnDestroyItem()
    {
        // Called when pool is destroyed or item is removed
        // Final cleanup here
    }
}
```

### Step 2: Create a Pool

```csharp
using Core.Foundation.Pool;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private IPool<Bullet> bulletPool;

    void Start()
    {
        // Create a pool for bullets
        bulletPool = new MonoPool<Bullet>(
            prefab: bulletPrefab,
            defaultCapacity: 20,
            maxSize: 100
        );

        // Register with PoolManager (optional but recommended)
        PoolManager.Instance.RegisterPool(bulletPool, "BulletPool");
    }

    void FireBullet()
    {
        // Get bullet from pool
        Bullet bullet = bulletPool.Get();
        
        // Use it...
        bullet.transform.position = transform.position;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10f;
    }

    void ReturnBullet(Bullet bullet)
    {
        // Return bullet to pool
        bulletPool.Release(bullet);
    }
}
```

---

## Core Concepts

### 1. IPoolable Interface

Every pooled object must implement `IPoolable`:

```csharp
public interface IPoolable
{
    void OnSpawn();      // Called when getting from pool
    void OnDespawn();    // Called when returning to pool
    void OnDestroyItem(); // Called when destroying the item
}
```

### 2. Pool Types

#### MonoPool<T>
For pooling Unity GameObjects with MonoBehaviour components.

```csharp
IPool<MyMonoBehaviour> pool = new MonoPool<MyMonoBehaviour>(
    prefab: myPrefab,
    defaultCapacity: 10,  // Initial pool size
    maxSize: 50,          // Maximum pool size
    parent: transform,    // Optional: parent transform
    collectionCheck: false // Optional: enable double-release detection
);
```

#### ObjectPoolWrapper<T>
For pooling plain C# classes (non-MonoBehaviour).

```csharp
IPool<MyClass> pool = new ObjectPoolWrapper<MyClass>(
    defaultCapacity: 10,
    maxSize: 50,
    collectionCheck: false
);
```

### 3. PoolManager

Centralized manager for all pools with statistics tracking.

```csharp
// Register a pool
PoolManager.Instance.RegisterPool(myPool, "MyPoolName");

// Get object from registered pool
MyObject obj = PoolManager.Instance.Get<MyObject>("MyPoolName");

// Return object to registered pool
PoolManager.Instance.Release("MyPoolName", obj);

// Get statistics
var stats = PoolManager.Instance.GetStats("MyPoolName");
Debug.Log($"Active: {stats.CurrentActive}, Available: {stats.CurrentAvailable}");
```

---

## Usage Examples

### Example 1: Bullet Pool (MonoBehaviour)

```csharp
using Core.Foundation.Pool;
using UnityEngine;

// 1. Create the poolable bullet class
public class Bullet : MonoBehaviour, IPoolable
{
    private Rigidbody rb;
    private float lifetime = 5f;
    private float spawnTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnSpawn()
    {
        spawnTime = Time.time;
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void OnDestroyItem()
    {
        // Cleanup if needed
    }

    void Update()
    {
        // Auto-return to pool after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            PoolManager.Instance.Release("BulletPool", this);
        }
    }
}

// 2. Setup the pool in a manager
public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletParent;

    void Start()
    {
        var bulletPool = new MonoPool<Bullet>(
            prefab: bulletPrefab,
            defaultCapacity: 50,
            maxSize: 200,
            parent: bulletParent
        );

        PoolManager.Instance.RegisterPool(bulletPool, "BulletPool");
    }

    public void Shoot(Vector3 position, Vector3 direction)
    {
        Bullet bullet = PoolManager.Instance.Get<Bullet>("BulletPool");
        bullet.transform.position = position;
        bullet.transform.forward = direction;
        bullet.GetComponent<Rigidbody>().velocity = direction * 20f;
    }
}
```

### Example 2: Particle Effect Pool

```csharp
using Core.Foundation.Pool;
using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPoolable
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        ps.Play();
        
        // Auto-return after particle finishes
        StartCoroutine(ReturnToPoolAfterDuration());
    }

    public void OnDespawn()
    {
        ps.Stop();
        ps.Clear();
        gameObject.SetActive(false);
    }

    public void OnDestroyItem()
    {
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDuration()
    {
        yield return new WaitForSeconds(ps.main.duration);
        PoolManager.Instance.Release("EffectPool", this);
    }
}

// Setup
public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleEffect explosionPrefab;

    void Start()
    {
        var effectPool = new MonoPool<ParticleEffect>(
            prefab: explosionPrefab,
            defaultCapacity: 10,
            maxSize: 30
        );

        PoolManager.Instance.RegisterPool(effectPool, "EffectPool");
    }

    public void PlayExplosion(Vector3 position)
    {
        var effect = PoolManager.Instance.Get<ParticleEffect>("EffectPool");
        effect.transform.position = position;
    }
}
```

### Example 3: Plain C# Class Pool

```csharp
using Core.Foundation.Pool;

// 1. Create poolable class (must have parameterless constructor)
public class DamageNumber : IPoolable
{
    public float Value { get; set; }
    public Vector3 Position { get; set; }
    public float Duration { get; set; }

    public void OnSpawn()
    {
        // Reset state
        Value = 0f;
        Duration = 1f;
    }

    public void OnDespawn()
    {
        // Clean up
        Value = 0f;
    }

    public void OnDestroyItem()
    {
        // Final cleanup
    }
}

// 2. Setup and use
public class DamageSystem
{
    private IPool<DamageNumber> damageNumberPool;

    public void Initialize()
    {
        damageNumberPool = new ObjectPoolWrapper<DamageNumber>(
            defaultCapacity: 20,
            maxSize: 100
        );

        PoolManager.Instance.RegisterPool(damageNumberPool, "DamageNumbers");
    }

    public void ShowDamage(float damage, Vector3 position)
    {
        var dmgNum = PoolManager.Instance.Get<DamageNumber>("DamageNumbers");
        dmgNum.Value = damage;
        dmgNum.Position = position;
        
        // ... display logic ...
        
        // Return after use
        PoolManager.Instance.Release("DamageNumbers", dmgNum);
    }
}
```

### Example 4: Enemy Pool with Multiple Types

```csharp
using Core.Foundation.Pool;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IPoolable
{
    protected int health;
    protected float speed;

    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
        ResetStats();
    }

    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDestroyItem()
    {
        // Cleanup
    }

    protected abstract void ResetStats();
}

public class Zombie : Enemy
{
    protected override void ResetStats()
    {
        health = 100;
        speed = 2f;
    }
}

public class FastZombie : Enemy
{
    protected override void ResetStats()
    {
        health = 50;
        speed = 5f;
    }
}

// Manager
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Zombie zombiePrefab;
    [SerializeField] private FastZombie fastZombiePrefab;

    void Start()
    {
        // Create separate pools for each enemy type
        var zombiePool = new MonoPool<Zombie>(
            prefab: zombiePrefab,
            defaultCapacity: 20,
            maxSize: 50
        );

        var fastZombiePool = new MonoPool<FastZombie>(
            prefab: fastZombiePrefab,
            defaultCapacity: 10,
            maxSize: 30
        );

        PoolManager.Instance.RegisterPool(zombiePool, "ZombiePool");
        PoolManager.Instance.RegisterPool(fastZombiePool, "FastZombiePool");
    }

    public void SpawnZombie(Vector3 position)
    {
        var zombie = PoolManager.Instance.Get<Zombie>("ZombiePool");
        zombie.transform.position = position;
    }

    public void SpawnFastZombie(Vector3 position)
    {
        var zombie = PoolManager.Instance.Get<FastZombie>("FastZombiePool");
        zombie.transform.position = position;
    }

    public void DespawnEnemy(Enemy enemy)
    {
        if (enemy is Zombie)
            PoolManager.Instance.Release("ZombiePool", enemy as Zombie);
        else if (enemy is FastZombie)
            PoolManager.Instance.Release("FastZombiePool", enemy as FastZombie);
    }
}
```

---

## Advanced Features

### 1. Pool Statistics Tracking

```csharp
// Get detailed statistics for a pool
var stats = PoolManager.Instance.GetStats("BulletPool");

Debug.Log($"Pool: {stats.PoolName}");
Debug.Log($"Currently Active: {stats.CurrentActive}");
Debug.Log($"Currently Available: {stats.CurrentAvailable}");
Debug.Log($"Max Active (Peak): {stats.MaxActive}");
Debug.Log($"Total Gets: {stats.TotalGets}");
Debug.Log($"Total Releases: {stats.TotalReleases}");

// Get all pool statistics
var allStats = PoolManager.Instance.GetAllStats();
foreach (var kvp in allStats)
{
    Debug.Log($"{kvp.Key}: Active={kvp.Value.CurrentActive}, Available={kvp.Value.CurrentAvailable}");
}
```

### 2. Debug Mode (Collection Checking)

Enable collection checking during development to catch bugs:

```csharp
// Enable in debug/editor builds
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    var pool = new MonoPool<Bullet>(
        prefab: bulletPrefab,
        defaultCapacity: 10,
        maxSize: 50,
        collectionCheck: true  // Will throw exception on double-release
    );
#else
    var pool = new MonoPool<Bullet>(
        prefab: bulletPrefab,
        defaultCapacity: 10,
        maxSize: 50,
        collectionCheck: false  // Better performance
    );
#endif
```

### 3. Custom Parent Transform

Organize pooled objects in hierarchy:

```csharp
public class PoolContainer : MonoBehaviour
{
    [SerializeField] private Transform bulletContainer;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private Transform effectContainer;

    void Start()
    {
        var bulletPool = new MonoPool<Bullet>(
            prefab: bulletPrefab,
            defaultCapacity: 50,
            maxSize: 200,
            parent: bulletContainer  // All bullets will be children of this
        );

        var enemyPool = new MonoPool<Enemy>(
            prefab: enemyPrefab,
            defaultCapacity: 20,
            maxSize: 100,
            parent: enemyContainer  // All enemies will be children of this
        );
    }
}
```

### 4. Direct Pool Usage (Without PoolManager)

You can use pools directly without registering them:

```csharp
public class DirectPoolExample : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private IPool<Bullet> bulletPool;

    void Start()
    {
        bulletPool = new MonoPool<Bullet>(bulletPrefab, 20, 100);
    }

    void Shoot()
    {
        Bullet bullet = bulletPool.Get();
        // Use bullet...
    }

    void ReturnBullet(Bullet bullet)
    {
        bulletPool.Release(bullet);
    }

    void CheckPoolStatus()
    {
        Debug.Log($"Active: {bulletPool.ActiveCount}");
        Debug.Log($"Available: {bulletPool.AvailableCount}");
    }
}
```

---

## Best Practices

### 1. Pool Sizing
```csharp
// ✅ GOOD: Size based on expected usage
var bulletPool = new MonoPool<Bullet>(
    prefab: bulletPrefab,
    defaultCapacity: 50,   // Average bullets on screen
    maxSize: 200          // Maximum possible bullets
);

// ❌ BAD: Undersized pool
var bulletPool = new MonoPool<Bullet>(
    prefab: bulletPrefab,
    defaultCapacity: 5,    // Too small
    maxSize: 10           // Will hit limit quickly
);
```

### 2. Always Implement All IPoolable Methods
```csharp
// ✅ GOOD: Complete implementation
public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawn()
    {
        // Reset velocity, position, etc.
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }

    public void OnDespawn()
    {
        // Clear state
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void OnDestroyItem()
    {
        // Stop coroutines, unsubscribe events
        StopAllCoroutines();
    }
}

// ❌ BAD: Empty implementations
public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawn() { }
    public void OnDespawn() { }
    public void OnDestroyItem() { }
}
```

### 3. Use Consistent Pool Names
```csharp
// ✅ GOOD: Use constants
public static class PoolNames
{
    public const string BULLET = "BulletPool";
    public const string ENEMY = "EnemyPool";
    public const string EFFECT = "EffectPool";
}

PoolManager.Instance.RegisterPool(bulletPool, PoolNames.BULLET);
var bullet = PoolManager.Instance.Get<Bullet>(PoolNames.BULLET);

// ❌ BAD: Magic strings
PoolManager.Instance.RegisterPool(bulletPool, "bullets");
var bullet = PoolManager.Instance.Get<Bullet>("Bullets");  // Typo!
```

### 4. Auto-Return Pattern
```csharp
// ✅ GOOD: Object returns itself
public class Bullet : MonoBehaviour, IPoolable
{
    private float lifetime = 5f;
    private float spawnTime;

    public void OnSpawn()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime > lifetime)
        {
            PoolManager.Instance.Release("BulletPool", this);
        }
    }
    
    // ... other methods ...
}

// ❌ BAD: External tracking required
// Requires manager to track all active bullets and their lifetimes
```

### 5. Pool Pre-Warming
```csharp
// ✅ GOOD: Pre-warm pool at startup
void Start()
{
    var bulletPool = new MonoPool<Bullet>(bulletPrefab, 50, 200);
    PoolManager.Instance.RegisterPool(bulletPool, "BulletPool");

    // Pre-warm: Get and immediately release to create instances
    var tempBullets = new List<Bullet>();
    for (int i = 0; i < 50; i++)
    {
        tempBullets.Add(bulletPool.Get());
    }
    foreach (var bullet in tempBullets)
    {
        bulletPool.Release(bullet);
    }
}
```

### 6. Monitor Pool Usage
```csharp
// ✅ GOOD: Monitor and adjust
void Update()
{
    var stats = PoolManager.Instance.GetStats("BulletPool");
    
    if (stats.MaxActive > stats.CurrentActive * 0.9f)
    {
        Debug.LogWarning($"Bullet pool is almost at capacity! Consider increasing maxSize.");
    }
}
```

---

## Troubleshooting

### Issue 1: NullReferenceException when Getting from Pool

**Symptom:**
```csharp
var bullet = PoolManager.Instance.Get<Bullet>("BulletPool");
// bullet is null
```

**Solution:**
- Ensure the pool is registered before trying to get objects
- Check pool name spelling (use constants!)
- Verify PoolManager.Instance exists

```csharp
// ✅ Fix
void Start()
{
    // Register first
    PoolManager.Instance.RegisterPool(bulletPool, "BulletPool");
}

void Shoot()
{
    // Then get
    var bullet = PoolManager.Instance.Get<Bullet>("BulletPool");
}
```

### Issue 2: Objects Not Resetting Properly

**Symptom:**
Objects behave strangely when reused from pool.

**Solution:**
Properly implement OnSpawn() and OnDespawn():

```csharp
public void OnSpawn()
{
    // Reset ALL state
    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;
    GetComponent<Rigidbody>().velocity = Vector3.zero;
    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    health = maxHealth;
    // etc...
}
```

### Issue 3: Memory Leaks

**Symptom:**
Memory usage keeps increasing.

**Solution:**
- Ensure objects are properly returned to pool
- Implement OnDestroyItem() to clean up resources
- Don't hold references to pooled objects after release

```csharp
public void OnDestroyItem()
{
    StopAllCoroutines();
    // Unsubscribe from events
    OnHit -= HandleHit;
    // Clear references
    target = null;
}
```

### Issue 4: Type Mismatch Error

**Symptom:**
```
[PoolManager] Pool 'EnemyPool' exists but is not of type IPool<FastZombie>!
```

**Solution:**
Use correct type when getting from pool:

```csharp
// ❌ Wrong
var zombie = PoolManager.Instance.Get<FastZombie>("ZombiePool");  // Pool is for Zombie, not FastZombie

// ✅ Correct
var zombie = PoolManager.Instance.Get<Zombie>("ZombiePool");
```

### Issue 5: Double Release Exception (with collectionCheck enabled)

**Symptom:**
Exception thrown when releasing an object.

**Solution:**
Don't release the same object twice:

```csharp
// ❌ Bad
void OnDestroy()
{
    PoolManager.Instance.Release("BulletPool", this);
}

void OnCollision()
{
    PoolManager.Instance.Release("BulletPool", this);  // Might release twice!
}

// ✅ Good
private bool isInPool = false;

void ReturnToPool()
{
    if (!isInPool)
    {
        isInPool = true;
        PoolManager.Instance.Release("BulletPool", this);
    }
}

public void OnSpawn()
{
    isInPool = false;
}
```

---

## Performance Tips

### 1. Disable GameObject on Release
MonoPool already does this, but if using custom pools:
```csharp
public void OnDespawn()
{
    gameObject.SetActive(false);  // Prevents Update() calls
}
```

### 2. Use Object Pooling for Frequent Allocations
Pool anything instantiated more than 10 times:
- ✅ Bullets, enemies, particles, UI elements
- ❌ Level geometry, player character, bosses

### 3. Avoid LINQ in Hot Paths
```csharp
// ❌ Avoid
var activeBullets = allBullets.Where(b => b.gameObject.activeSelf).ToList();

// ✅ Better
var activeBullets = new List<Bullet>();
foreach (var bullet in allBullets)
{
    if (bullet.gameObject.activeSelf)
        activeBullets.Add(bullet);
}
```

### 4. Profile Your Pools
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.P))
    {
        var allStats = PoolManager.Instance.GetAllStats();
        foreach (var kvp in allStats)
        {
            Debug.Log($"{kvp.Key}: Peak Usage = {kvp.Value.MaxActive}/{kvp.Value.MaxActive + kvp.Value.CurrentAvailable}");
        }
    }
}
```

---

## Complete Working Example

```csharp
using Core.Foundation.Pool;
using UnityEngine;
using System.Collections.Generic;

// 1. Poolable Bullet
public class Bullet : MonoBehaviour, IPoolable
{
    private Rigidbody rb;
    private float spawnTime;
    private const float LIFETIME = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnSpawn()
    {
        spawnTime = Time.time;
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void OnDestroyItem()
    {
        // Cleanup
    }

    void Update()
    {
        if (Time.time - spawnTime > LIFETIME)
        {
            PoolManager.Instance.Release(PoolNames.BULLET, this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Hit something, return to pool
        PoolManager.Instance.Release(PoolNames.BULLET, this);
    }
}

// 2. Pool Names Constants
public static class PoolNames
{
    public const string BULLET = "BulletPool";
}

// 3. Weapon System
public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    void Start()
    {
        // Initialize pool
        var bulletPool = new MonoPool<Bullet>(
            prefab: bulletPrefab,
            defaultCapacity: 50,
            maxSize: 200,
            parent: bulletParent,
            collectionCheck: Application.isEditor
        );

        // Register with manager
        PoolManager.Instance.RegisterPool(bulletPool, PoolNames.BULLET);

        // Pre-warm
        PreWarmPool(bulletPool, 50);
    }

    void PreWarmPool(IPool<Bullet> pool, int count)
    {
        var temp = new List<Bullet>();
        for (int i = 0; i < count; i++)
        {
            temp.Add(pool.Get());
        }
        foreach (var bullet in temp)
        {
            pool.Release(bullet);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintPoolStats();
        }
    }

    void Fire()
    {
        Bullet bullet = PoolManager.Instance.Get<Bullet>(PoolNames.BULLET);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.GetComponent<Rigidbody>().velocity = firePoint.forward * bulletSpeed;
    }

    void PrintPoolStats()
    {
        var stats = PoolManager.Instance.GetStats(PoolNames.BULLET);
        Debug.Log($"Bullet Pool Stats:");
        Debug.Log($"  Active: {stats.CurrentActive}");
        Debug.Log($"  Available: {stats.CurrentAvailable}");
        Debug.Log($"  Peak Usage: {stats.MaxActive}");
        Debug.Log($"  Total Gets: {stats.TotalGets}");
        Debug.Log($"  Total Releases: {stats.TotalReleases}");
    }
}
```

---

## Conclusion

The Pool System provides a robust, efficient way to manage object reuse in Unity. By following this guide and best practices, you can significantly improve your game's performance and reduce garbage collection overhead.

### Key Takeaways:
1. Always implement IPoolable methods properly
2. Use PoolManager for centralized management
3. Enable collectionCheck during development
4. Monitor pool statistics to optimize sizing
5. Pre-warm pools at startup for consistent performance

For additional support or questions, refer to the code comments or contact your team lead.