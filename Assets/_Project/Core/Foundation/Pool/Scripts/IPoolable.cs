namespace Core.Foundation.Pool
{

    public interface IPoolable
    {
    	void OnSpawn();
    	void OnDespawn();
    	void OnDestroyItem();
    }
}
