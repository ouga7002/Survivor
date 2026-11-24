using System.Collections;
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

    [Header("攻撃時の画像")]
    public Sprite attackSprite;
    public float attackSpriteDuration = 0.3f;

    [Header("向き・回転設定")]
    public float rotationSpeed = 360f;
    public float maxRotationAngle = 35f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector3 firePointDefaultPosition;

    private Sprite normalSprite;
    private Coroutine spriteCoroutine;

    private float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            normalSprite = spriteRenderer.sprite;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        nextAttackTime = Time.time + attackInterval;

        if (firePoint != null)
        {
            firePointDefaultPosition = firePoint.localPosition;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(transform.position, player.position);

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

        // プレイヤーとの距離を保つ
        if (distance > preferredDistance + distanceTolerance)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            rb.linearVelocity = -direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // プレイヤーの左右位置に応じて向きを変更
        bool facingLeft = direction.x < 0f;

        spriteRenderer.flipX = facingLeft;

        // FirePointの位置も反転
        if (firePoint != null)
        {
            firePoint.localPosition = new Vector3(
                facingLeft
                    ? -firePointDefaultPosition.x
                    : firePointDefaultPosition.x,
                firePointDefaultPosition.y,
                firePointDefaultPosition.z
            );
        }

        // 上下方向に合わせて体を回転
        float targetAngle = Mathf.Atan2(
            direction.y,
            Mathf.Abs(direction.x)
        ) * Mathf.Rad2Deg;

        // 左向きの場合は回転方向を反転
        if (facingLeft)
        {
            targetAngle = -targetAngle;
        }

        // 回転角度を制限
        targetAngle = Mathf.Clamp(
            targetAngle,
            -maxRotationAngle,
            maxRotationAngle
        );

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || player == null)
            return;

        // 攻撃時の画像に切り替える
        PlayAttackSprite();

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
            Debug.LogError(
                "矢のPrefabにE_Arrowが設定されていません！"
            );

            Destroy(arrow);
        }
    }

    // 攻撃画像に切り替える
    void PlayAttackSprite()
    {
        if (spriteRenderer == null || attackSprite == null)
            return;

        if (spriteCoroutine != null)
            StopCoroutine(spriteCoroutine);

        spriteCoroutine = StartCoroutine(AttackSpriteCoroutine());
    }

    IEnumerator AttackSpriteCoroutine()
    {
        spriteRenderer.sprite = attackSprite;

        yield return new WaitForSeconds(attackSpriteDuration);

        spriteRenderer.sprite = normalSprite;

        spriteCoroutine = null;
    }
}
