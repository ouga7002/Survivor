using UnityEngine;

public class E_Enemy2Controller : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 1.5f; // 少しゆっくり離れる速度

    [Header("接触ダメージ設定")]
    [SerializeField] private int contactDamage = 1; // プレイヤーに与えるダメージ

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
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
        // ノックバック中なら処理を中断
        KnockbackReceiver knockback = GetComponent<KnockbackReceiver>();
        if (knockback != null && knockback.IsKnockedBack())
        {
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // プレイヤーとは「逆の方向」を計算して離れていく
        Vector2 awayDirection = ((Vector2)rb.position - (Vector2)player.position).normalized;
        rb.linearVelocity = awayDirection * moveSpeed;

        // 逃げる方向（awayDirection）にスプライトの向きを合わせる
        if (spriteRenderer != null)
        {
            if (awayDirection.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (awayDirection.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    // ★ プレイヤーに触れたときの接触ダメージ判定
    // （※敵の Collider2D の "Is Trigger" にチェックが入っている前提の書き方です）
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                // プレイヤー側にダメージを受けるメソッド（例: TakeDamage）があればここで呼び出します
                // playerStatus.TakeDamage(contactDamage);
            }
        }
    }
}