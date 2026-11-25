using Assets.Enemy.Enemy_Manager;
using UnityEngine;

public class TreasureEnemyController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 3.5f;
    [Tooltip("プレイヤーから逃げる際に少しふらつく強さ（単調な直線逃げを防ぐ）")]
    [SerializeField] private float wanderStrength = 0.4f;
    [SerializeField] private float wanderChangeInterval = 0.8f;

    [Header("体力・GPドロップ設定")]
    [SerializeField] private int maxHp = 100;
    [Tooltip("攻撃を受けた際にドロップするGPオーブの個数")]
    [SerializeField] private int dropOrbCountOnHit = 2;
    [Tooltip("オーブが飛び散る半径")]
    [SerializeField] private float dropSpreadRadius = 1.0f;

    private int currentHp;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 wanderOffset;
    private float wanderTimer;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ランダムなふらつき方向の更新
        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            wanderOffset = Random.insideUnitCircle * wanderStrength;
            wanderTimer = wanderChangeInterval;
        }

        // プレイヤーから「離れる」方向を計算 ＋ ふらつきを加える
        Vector2 awayDirection = (rb.position - (Vector2)player.position).normalized;
        Vector2 finalDirection = (awayDirection + wanderOffset).normalized;

        rb.linearVelocity = finalDirection * moveSpeed;

        // 進行方向に合わせてスプライトを反転
        if (spriteRenderer != null)
        {
            if (finalDirection.x > 0.01f)
            {
                spriteRenderer.flipX = true;
            }
            else if (finalDirection.x < -0.01f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    // プレイヤーから呼ばれるダメージ処理
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        // ダメージを受けるたびにGPオーブをポロポロ落とす
        SpawnGPOrbs();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    [Header("GPオーブのプレハブ")]
    [SerializeField] private GameObject gpOrbPrefab; // インスペクターでオーブのプレハブを割り当てる

    private void SpawnGPOrbs()
    {
        if (GPOrbPool.Instance == null || gpOrbPrefab == null) return;

        for (int i = 0; i < dropOrbCountOnHit; i++)
        {
            // 敵の周囲に少し散らばる位置を計算
            Vector2 randomOffset = Random.insideUnitCircle * dropSpreadRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            // 既存のGPOrbPoolの Spawn メソッドを呼び出す
            GPOrbPool.Instance.Spawn(gpOrbPrefab, spawnPos);
        }
    }

    private void Die()
    {
        // 撃破時にもボーナスで少し多めにオーブを散らす
        dropOrbCountOnHit *= 3;
        SpawnGPOrbs();

        Destroy(gameObject);
    }
}