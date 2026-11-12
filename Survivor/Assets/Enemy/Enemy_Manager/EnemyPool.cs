using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolData
    {
        public GameObject enemyPrefab;
        public int initialSize = 10;
    }

    [Header("敵ごとの初期プール数")]
    public PoolData[] pools;

    private Dictionary<GameObject, Queue<PooledEnemy>> poolDictionary
        = new Dictionary<GameObject, Queue<PooledEnemy>>();

    public int ActiveCount { get; private set; }

    void Awake()
    {
        foreach (PoolData data in pools)
        {
            if (data.enemyPrefab == null) continue;

            if (!poolDictionary.ContainsKey(data.enemyPrefab))
            {
                poolDictionary.Add(
                    data.enemyPrefab,
                    new Queue<PooledEnemy>()
                );
            }

            for (int i = 0; i < data.initialSize; i++)
            {
                CreateEnemy(data.enemyPrefab);
            }
        }
    }

    PooledEnemy CreateEnemy(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);

        PooledEnemy pooledEnemy = obj.GetComponent<PooledEnemy>();

        if (pooledEnemy == null)
        {
            pooledEnemy = obj.AddComponent<PooledEnemy>();
        }

        pooledEnemy.Initialize(this, prefab);

        poolDictionary[prefab].Enqueue(pooledEnemy);

        return pooledEnemy;
    }

    public GameObject Spawn(
    GameObject prefab,
    Vector2 position)
    {
        if (prefab == null) return null;

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(
                prefab,
                new Queue<PooledEnemy>()
            );
        }

        Queue<PooledEnemy> pool = poolDictionary[prefab];

        // 待機中の敵がいなければ追加生成
        if (pool.Count == 0)
        {
            CreateEnemy(prefab);
        }

        PooledEnemy enemy = pool.Dequeue();

        enemy.transform.SetPositionAndRotation(
            position,
            Quaternion.identity
        );

        enemy.gameObject.SetActive(true);
        ActiveCount++;

        return enemy.gameObject;
    }

    public void Return(PooledEnemy enemy, GameObject prefab)
    {
        if (enemy == null || !enemy.gameObject.activeSelf) return;

        enemy.gameObject.SetActive(false);

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(
                prefab,
                new Queue<PooledEnemy>()
            );
        }

        poolDictionary[prefab].Enqueue(enemy);

        ActiveCount--;
    }
}
