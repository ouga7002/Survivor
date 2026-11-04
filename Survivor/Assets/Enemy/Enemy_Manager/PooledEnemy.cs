
using UnityEngine;

public class PooledEnemy : MonoBehaviour
{
    private EnemyPool pool;
    private GameObject originalPrefab;

    public void Initialize(
        EnemyPool enemyPool,
        GameObject prefab)
    {
        pool = enemyPool;
        originalPrefab = prefab;
    }

    public void ReturnToPool()
    {
        if (pool == null) return;

        pool.Return(this, originalPrefab);
    }
}

