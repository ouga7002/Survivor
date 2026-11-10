using UnityEngine;

public class Boss1Controller : MonoBehaviour
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public GameObject bullet1;
    public GameObject bullet2;

    // 弾の生成位置を頭上からどれだけ離すか
    public float spawnHeight = 0.3f;

    // 攻撃間隔
    public float attackInterval = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 2秒後から攻撃開始
        InvokeRepeating(nameof(Attack), 2f, attackInterval);
    }

    void Attack()
    {
        if (bullet2 == null)
        {
            Debug.LogError("Bullet2が設定されていません！");
            return;
        }

        Vector3 createPosition;

        if (spriteRenderer != null)
        {
            // ボスのスプライトの一番上を取得
            createPosition = new Vector3(
                spriteRenderer.bounds.center.x,
                spriteRenderer.bounds.max.y + spawnHeight,
                transform.position.z
            );
        }
        else
        {
            createPosition = transform.position;
            createPosition.y += spawnHeight;
        }

        // 弾を生成
        Instantiate(bullet2, createPosition, Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        // 左右の移動方向に合わせて反転
        if (spriteRenderer != null)
        {
            if (direction.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
