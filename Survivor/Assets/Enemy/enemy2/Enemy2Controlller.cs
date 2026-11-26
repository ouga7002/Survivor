using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    public float moveSpeed;

    public int Spd;

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

        int Spd = Random.Range(1, 4);

        moveSpeed = Spd;
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