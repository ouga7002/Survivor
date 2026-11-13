using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("手裏剣のプレハブをここにアタッチする")]
    [SerializeField] private GameObject shurikenPrefab;

    [Header("手裏剣のパラメータ")]
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float orbitRadius = 1.8f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float selfRotationSpeed = 360f;
    [SerializeField] private float activeDuration = 5.0f;
    [SerializeField] private float coolDownDuration = 3.0f;

    public bool isUnlocked = false;

    private GameObject spawnedShuriken;
    private float currentAngle = 0f;
    private float stateTimer = 0f;
    private bool isActiveState = true;

    void Update()
    {
        if (!isUnlocked)
        {
            if (spawnedShuriken != null)
            {
                Destroy(spawnedShuriken);
            }
            return;
        }

        if (spawnedShuriken == null && shurikenPrefab != null)
        {
            spawnedShuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            spawnedShuriken.transform.SetParent(transform);

            ShurikenHitbox hitbox = spawnedShuriken.GetComponent<ShurikenHitbox>();
            if (hitbox == null)
            {
                hitbox = spawnedShuriken.AddComponent<ShurikenHitbox>();
            }
            hitbox.damage = attackPower;
        }

        if (isActiveState)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= activeDuration)
            {
                isActiveState = false;
                stateTimer = 0f;
                if (spawnedShuriken != null) spawnedShuriken.SetActive(false);
            }
        }
        else
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= coolDownDuration)
            {
                isActiveState = true;
                stateTimer = 0f;
                if (spawnedShuriken != null) spawnedShuriken.SetActive(true);
            }
            return;
        }

        if (spawnedShuriken != null && spawnedShuriken.activeSelf)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            float rad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * orbitRadius;
            float y = Mathf.Sin(rad) * orbitRadius;

            spawnedShuriken.transform.localPosition = new Vector3(x, y, 0f);
            spawnedShuriken.transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
        }
    }
}

// ★ボスや通常の敵どちらにもヒットするように条件を拡張したクラス
public class ShurikenHitbox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string objName = collision.name.ToLower();

        // 敵（Enemy）またはボス（Boss）、あるいは名前やタグにそれらを含むものを対象にする
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") ||
            objName.Contains("enemy") || objName.Contains("boss") || objName.Contains("中ボス"))
        {
            // ボス側が「EnemyHealth」を持っているか、あるいは共通のダメージ処理コンポーネントを持っているか
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                // もしボス専用の別コンポーネント（例: BossHealth など）があればここで取得してダメージを与えることも可能です
                // BossHealth bossHealth = collision.GetComponent<BossHealth>();
                // if (bossHealth != null) bossHealth.TakeDamage(damage);
            }
        }
    }
}