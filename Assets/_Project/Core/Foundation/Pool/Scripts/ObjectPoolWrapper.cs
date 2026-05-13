using System;
using UnityEngine.Pool;

namespace Core.Foundation.Pool
{

    public class ObjectPoolWrapper<T> : IPool<T> where T : class, IPoolable, new()
    {
    	readonly ObjectPool<T> _pool;

    	public ObjectPoolWrapper(int defaultCapacity = 10, int maxSize = 50, bool collectionCheck = false)
    	{
    		_pool = new ObjectPool<T>
    		(
    			createFunc: () => new T(), 
    			actionOnGet: obj => obj.OnSpawn(), 
    			actionOnRelease: obj => obj.OnDespawn(), 
    			actionOnDestroy: obj => obj.OnDestroyItem(),
    			defaultCapacity: defaultCapacity, 
    			maxSize: maxSize, 
    			collectionCheck: collectionCheck
    		);
    	}

    	public int ActiveCount => _pool.CountActive;

    	public int AvailableCount => _pool.CountInactive;

    	public T Get()
    	{
    		return _pool.Get();
    	}

    	public void Release(T item)
    	{
    		_pool.Release(item);
    	}
    }
}
