using Assets.Enemy.Enemy_Manager;
using System.Collections.Generic;
using UnityEngine;

public class GPOrbPool : MonoBehaviour
{
    public static GPOrbPool Instance { get; private set; }

    [System.Serializable]
    public class OrbPoolData
    {
        public GameObject orbPrefab;
        public int initialSize = 20;
    }

    [Header("オーブのプール設定")]
    public OrbPoolData[] orbPools;

    private Dictionary<GameObject, Queue<GPOrb>> pools =
        new Dictionary<GameObject, Queue<GPOrb>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (OrbPoolData data in orbPools)
        {
            if (data.orbPrefab == null) continue;

            if (pools.ContainsKey(data.orbPrefab)) continue;

            pools.Add(
                data.orbPrefab,
                new Queue<GPOrb>()
            );

            for (int i = 0; i < data.initialSize; i++)
            {
                CreateOrb(data.orbPrefab);
            }
        }
    }

    void CreateOrb(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);

        GPOrb orb = obj.GetComponent<GPOrb>();

        if (orb == null)
        {
            Debug.LogError(
                prefab.name + " にGPOrbがありません"
            );

            Destroy(obj);
            return;
        }

        orb.Initialize(this);
        orb.SetOriginalPrefab(prefab);

        obj.SetActive(false);

        pools[prefab].Enqueue(orb);
    }

    public void Spawn(GameObject prefab, Vector2 position)
    {
        if (prefab == null) return;

        if (!pools.ContainsKey(prefab))
        {
            pools.Add(
                prefab,
                new Queue<GPOrb>()
            );
        }

        Queue<GPOrb> pool = pools[prefab];

        // 使用可能なオーブがなければ追加生成
        if (pool.Count == 0)
        {
            CreateOrb(prefab);
        }

        if (pool.Count == 0) return;

        GPOrb orb = pool.Dequeue();

        orb.transform.position = position;
        orb.Setup();
        orb.gameObject.SetActive(true);
    }

    public void Return(GPOrb orb)
    {
        if (orb == null || !orb.gameObject.activeSelf) return;

        GameObject prefab = orb.OriginalPrefab;

        if (prefab == null || !pools.ContainsKey(prefab))
            return;

        orb.gameObject.SetActive(false);

        pools[prefab].Enqueue(orb);
    }
}