using UnityEngine;
using System.Collections;

public class B1_Bullet3 : MonoBehaviour
{
    [Header("出現設定")]
    public float warningDuration = 0.6f;
    public float activeDuration = 1.0f;

    [Header("出現予兆の赤い円")]
    public SpriteRenderer warningCircle;
    public float warningCircleAlpha = 0.7f;

    private Collider2D spikeCollider;
    private SpriteRenderer spriteRenderer;
    private EnemyStatus enemyStatus;

    private bool hasHit;

    void Awake()
    {
        spikeCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyStatus = GetComponent<EnemyStatus>();

        // 最初は岩を非表示にする
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (spikeCollider != null)
            spikeCollider.enabled = false;

        if (warningCircle != null)
            warningCircle.enabled = false;
    }

    void Start()
    {
        StartCoroutine(SpikeRoutine());
    }

    IEnumerator SpikeRoutine()
    {
        // 赤い円を表示
        if (warningCircle != null)
        {
            warningCircle.enabled = true;
            warningCircle.color = new Color(
                1f, 0f, 0f, warningCircleAlpha
            );
        }

        // 予兆時間待機
        yield return new WaitForSeconds(warningDuration);

        // 赤い円を消す
        if (warningCircle != null)
            warningCircle.enabled = false;

        // 岩を表示
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // 攻撃判定を有効化
        if (spikeCollider != null)
            spikeCollider.enabled = true;

        // 1秒間攻撃判定を維持
        yield return new WaitForSeconds(activeDuration);

        // 攻撃判定を無効化
        if (spikeCollider != null)
            spikeCollider.enabled = false;

        // 岩を消す
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || !collision.CompareTag("Player"))
            return;

        PlayerStatus playerStatus =
            collision.GetComponent<PlayerStatus>();

        if (playerStatus != null && enemyStatus != null)
        {
            playerStatus.TakeDamage(enemyStatus.Attack);
            hasHit = true;
        }
    }
}

