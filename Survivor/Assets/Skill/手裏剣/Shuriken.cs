using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("手裏剣のパラメータ")]
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float orbitRadius = 1.8f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float selfRotationSpeed = 360f;
    [SerializeField] private float activeDuration = 5.0f;
    [SerializeField] private float coolDownDuration = 3.0f;

    private float currentAngle = 0f;
    private Transform playerTransform;
    private float stateTimer = 0f;
    private bool isActiveState = true;
    private SpriteRenderer spriteRenderer;
    private Collider2D col2d;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 正しい Collider2D を取得するように修正しました
        col2d = GetComponent<Collider2D>();
    }

    // ガチャで有効化された瞬間に呼ばれる
    void OnEnable()
    {
        FindPlayer();
        stateTimer = 0f;
        isActiveState = true;
        SetShurikenActive(true);
    }

    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null) playerTransform = pObj.transform;
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null) playerTransform = taggedPlayer.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        stateTimer += Time.deltaTime;

        if (isActiveState)
        {
            if (stateTimer >= activeDuration)
            {
                isActiveState = false;
                stateTimer = 0f;
                SetShurikenActive(false);
            }
        }
        else
        {
            if (stateTimer >= coolDownDuration)
            {
                isActiveState = true;
                stateTimer = 0f;
                SetShurikenActive(true);
            }
            return;
        }

        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        float rad = currentAngle * Mathf.Deg2Rad;
        float x = playerTransform.position.x + Mathf.Cos(rad) * orbitRadius;
        float y = playerTransform.position.y + Mathf.Sin(rad) * orbitRadius;

        transform.position = new Vector3(x, y, 0f);
        transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
    }

    void SetShurikenActive(bool active)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = active;
        if (col2d != null) col2d.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActiveState) return;

        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackPower);
            }
        }
    }
}