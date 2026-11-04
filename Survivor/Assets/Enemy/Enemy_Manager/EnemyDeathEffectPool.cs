using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffectPool : MonoBehaviour
{
    public static EnemyDeathEffectPool Instance { get; private set; }

    [Header("撃破エフェクト")]
    public GameObject effectPrefab;

    [Header("初期生成数")]
    public int initialSize = 20;

    private Queue<EnemyDeathEffect> pool =
        new Queue<EnemyDeathEffect>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 最初にエフェクトをまとめて生成
        for (int i = 0; i < initialSize; i++)
        {
            CreateEffect();
        }
    }

    void CreateEffect()
    {
        if (effectPrefab == null) return;

        GameObject obj = Instantiate(effectPrefab, transform);

        EnemyDeathEffect effect =
            obj.GetComponent<EnemyDeathEffect>();

        if (effect == null)
        {
            effect = obj.AddComponent<EnemyDeathEffect>();
        }

        effect.Initialize(this);

        obj.SetActive(false);

        pool.Enqueue(effect);
    }

    public void Spawn(Vector3 position)
    {
        if (effectPrefab == null) return;

        // 使用可能なエフェクトがなければ追加生成
        if (pool.Count == 0)
        {
            CreateEffect();
        }

        if (pool.Count == 0) return;

        EnemyDeathEffect effect = pool.Dequeue();

        effect.PlayEffect(position);
    }

    public void ReturnEffect(EnemyDeathEffect effect)
    {
        if (effect == null) return;

        effect.ReturnToPool();

        // 再利用できるようにキューへ戻す
        pool.Enqueue(effect);
    }
}