using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private ParticleSystem particle;
    private EnemyDeathEffectPool pool;

    private bool isPlaying;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();

        if (particle == null)
        {
            Debug.LogError("ParticleSystemが見つかりません");
        }
    }

    public void Initialize(EnemyDeathEffectPool effectPool)
    {
        pool = effectPool;
    }

    public void PlayEffect(Vector3 position)
    {
        if (particle == null) return;

        transform.position = position;

        gameObject.SetActive(true);

        // 前回のパーティクルを消去
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 再生開始
        isPlaying = true;
        particle.Play(true);
    }

    void Update()
    {
        if (!isPlaying || particle == null) return;

        // パーティクルがすべて消えたらプールに返却
        if (!particle.IsAlive(true))
        {
            isPlaying = false;

            pool.ReturnEffect(this);
        }
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}