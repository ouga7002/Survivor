using UnityEngine;

public class B1_Bullet2 : MonoBehaviour
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private EnemyStatus enemyStatus;

    void Awake()
    {
        enemyStatus = GetComponent<EnemyStatus>();
    }

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
                spriteRenderer.flipX = true;
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStatus playerStatus =
            collision.gameObject.GetComponent<PlayerStatus>();

        if (collision.tag == "Player")
        {
            playerStatus.TakeDamage(enemyStatus.Attack);
            Destroy(gameObject);
        }
    }
}