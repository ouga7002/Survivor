using UnityEngine;

public class Boss1Controller : MonoBehaviour
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public GameObject bullet1;
    public GameObject bullet2;
    Vector3 createPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 2秒後から、5秒おきに DoSomething() を実行
        InvokeRepeating(nameof(Attack), 2f, 5f);
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            createPosition = transform.position;
            createPosition.y += 1.0f; // プレイヤーのy座標 + 1の位置に弾を生成する

            Instantiate(bullet2, createPosition, Quaternion.identity);
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // 左右の移動方向に合わせてスプライトを反転
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