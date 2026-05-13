using Core.Foundation.Pool;
using UnityEngine;
using UnityEngine.Pool;
namespace Core.Foundation.Pool
{


    public class MonoPool<T> : IPool<T> where T : MonoBehaviour, IPoolable
    {
    	readonly T _prefab;
    	readonly Transform _parent;
    	readonly ObjectPool<T> _pool;
    	
    	public MonoPool(T prefab, int defaultCapacity = 10, int maxSize = 50, Transform parent = null, bool collectionCheck = false)
    	{
    		_prefab = prefab;
    		_parent = parent;

    		_pool = new ObjectPool<T>(
    			createFunc: CreateFunc, 
    			actionOnGet: OnGet, 
    			actionOnRelease: OnRelease, 
    			actionOnDestroy: OnDestroyItem, 
    			collectionCheck: collectionCheck, 
    			defaultCapacity: defaultCapacity, 
    			maxSize: maxSize
    		);
    	}

    	private T CreateFunc ()
    	{
    		T obj = Object.Instantiate(_prefab, _parent);
    		obj.gameObject.SetActive(false);
    		return obj;
    	}
    	
    	private void OnGet(T obj)
    	{
    		obj.gameObject.SetActive(true);
    		obj.OnSpawn();
    	}
    	
    	private void OnRelease(T obj)
    	{
    		obj.OnDespawn();
    		obj.gameObject.SetActive(false);
    	}
    	
    	private void OnDestroyItem(T obj)
    	{
    		obj.OnDestroyItem();
    		if(obj != null)
    		{
    			Object.Destroy(obj.gameObject);
    		}
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
