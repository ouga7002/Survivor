using UnityEngine;

public class B1_Bullet2 : MonoBehaviour
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private EnemyStatus enemyStatus;

    public Transform BulletTrans;

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

    void Update()
    {
        Vector3 dir = (player.position - BulletTrans.position);

        BulletTrans.rotation = Quaternion.FromToRotation(Vector3.up, dir);
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
                spriteRenderer.flipY = true;           
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