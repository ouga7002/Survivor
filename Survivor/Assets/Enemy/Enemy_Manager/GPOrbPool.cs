using System.Collections.Generic;
using UnityEngine;

public class GPOrbPool : MonoBehaviour
{
    public static GPOrbPool Instance { get; private set; }

    [Header("オーブ設定")]
    public GameObject orbPrefab;
    public int initialSize = 50;

    private Queue<GPOrb> pool =
        new Queue<GPOrb>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            CreateOrb();
        }
    }

    void CreateOrb()
    {
        GameObject obj = Instantiate(orbPrefab, transform);

        GPOrb orb = obj.GetComponent<GPOrb>();

        if (orb == null)
        {
            Debug.LogError("GPOrbが見つかりません");
            Destroy(obj);
            return;
        }

        orb.Initialize(this);
        obj.SetActive(false);

        pool.Enqueue(orb);
    }

    public void Spawn(Vector2 position, int amount)
    {
        if (orbPrefab == null) return;

        // プールが空なら追加生成
        if (pool.Count == 0)
        {
            CreateOrb();
        }

        if (pool.Count == 0) return;

        GPOrb orb = pool.Dequeue();

        orb.transform.position = position;
        orb.Setup(amount);
        orb.gameObject.SetActive(true);
    }

    public void Return(GPOrb orb)
    {
        if (orb == null || !orb.gameObject.activeSelf) return;

        orb.gameObject.SetActive(false);
        pool.Enqueue(orb);
    }
}

