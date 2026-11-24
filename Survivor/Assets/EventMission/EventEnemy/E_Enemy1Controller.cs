
using UnityEngine;

public class E_Enemy1Controller : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f;
    public float preferredDistance = 5f;
    public float distanceTolerance = 0.5f;

    [Header("攻撃設定")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackInterval = 2f;
    public float attackRange = 8f;
    public float arrowSpeed = 7f;
    public float arrowDamage = 10f;
    public float arrowLifeTime = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        nextAttackTime = Time.time + attackInterval;
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        // 攻撃可能な距離なら弓矢を撃つ
        if (distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            ShootArrow();
            nextAttackTime = Time.time + attackInterval;
        }
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

        float distance =
            Vector2.Distance(transform.position, player.position);

        // プレイヤーとの距離に応じて移動
        if (distance > preferredDistance + distanceTolerance)
        {
            // 遠すぎる場合は近づく
            rb.linearVelocity = direction * moveSpeed;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            // 近すぎる場合は後退する
            rb.linearVelocity = -direction * moveSpeed;
        }
        else
        {
            // 適切な距離なら停止
            rb.linearVelocity = Vector2.zero;
        }

        // プレイヤーの方向を向く
        if (spriteRenderer != null)
        {
            if (direction.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (direction.x < -0.01f)
                spriteRenderer.flipX = true;
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || player == null)
            return;

        Vector2 spawnPosition;

        if (firePoint != null)
            spawnPosition = firePoint.position;
        else
            spawnPosition = transform.position;

        Vector2 direction =
            ((Vector2)player.position - spawnPosition).normalized;

        GameObject arrow = Instantiate(
            arrowPrefab,
            spawnPosition,
            Quaternion.identity
        );

        E_Arrow arrowScript = arrow.GetComponent<E_Arrow>();

        if (arrowScript != null)
        {
            arrowScript.Initialize(
                direction,
                arrowSpeed,
                arrowDamage,
                arrowLifeTime
            );
        }
        else
        {
            Debug.LogError("矢のPrefabにE_Arrowが設定されていません！");
            Destroy(arrow);
        }
    }
}
