using UnityEngine;

public class Enemy3Controller : MonoBehaviour
{
    [Header("突進設定")]
    public float chargeSpeed = 8f;
    public float faceDuration = 0.5f;

    [Header("画面外消滅設定")]
    public float invisibleDestroyTime = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;
    private PooledEnemy pooledEnemy;

    private Vector2 chargeDirection;
    private float faceTimer;
    private float invisibleTimer;

    private bool isCharging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        pooledEnemy = GetComponent<PooledEnemy>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void OnEnable()
    {
        faceTimer = faceDuration;
        invisibleTimer = 0f;
        isCharging = false;

        if (player != null)
        {
            chargeDirection =
                ((Vector2)player.position -
                (Vector2)transform.position).normalized;

            SetFacing(chargeDirection);
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        // 出現直後はプレイヤーの方向を向いて待機
        if (!isCharging)
        {
            rb.linearVelocity = Vector2.zero;

            faceTimer -= Time.fixedDeltaTime;

            if (faceTimer <= 0f)
            {
                isCharging = true;
            }

            return;
        }

        // 決めた方向へ一直線に突進
        rb.linearVelocity = chargeDirection * chargeSpeed;
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (IsVisibleFromCamera())
        {
            invisibleTimer = 0f;
        }
        else
        {
            invisibleTimer += Time.deltaTime;

            if (invisibleTimer >= invisibleDestroyTime)
            {
                ReturnToPool();
            }
        }
    }

    // スプライトのみプレイヤー方向へ回転
    // スプライトの反転と回転を設定
    void SetFacing(Vector2 direction)
    {
        // 左右反転を優先
        bool faceLeft = direction.x < 0;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr == null)
                continue;

            sr.flipX = faceLeft;

            // 上下方向の角度を調整
            float angle = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;

            if (direction.x < 0)
            {
                angle = -angle;
            }

            sr.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    bool IsVisibleFromCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return false;

        Vector3 viewportPosition =
            cam.WorldToViewportPoint(transform.position);

        return viewportPosition.z > 0 &&
               viewportPosition.x >= 0 &&
               viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 &&
               viewportPosition.y <= 1;
    }

    void ReturnToPool()
    {
        if (pooledEnemy != null)
        {
            pooledEnemy.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}