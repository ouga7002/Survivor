
using UnityEngine;

public class Shuriken : MonoBehaviour, ISkillUnlockable
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

    // =====================================
    // 共通スキル管理
    // =====================================

    public string SkillId => "shuriken";

    public void SetSkillUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (!isUnlocked)
        {
            if (spawnedShuriken != null)
            {
                Destroy(spawnedShuriken);
                spawnedShuriken = null;
            }
        }
    }

    // =====================================
    // 更新処理
    // =====================================

    void Update()
    {
        if (!isUnlocked)
        {
            if (spawnedShuriken != null)
            {
                Destroy(spawnedShuriken);
                spawnedShuriken = null;
            }

            return;
        }

        // 手裏剣生成
        if (spawnedShuriken == null && shurikenPrefab != null)
        {
            spawnedShuriken = Instantiate(
                shurikenPrefab,
                transform.position,
                Quaternion.identity
            );

            spawnedShuriken.transform.SetParent(transform);

            ShurikenHitbox hitbox =
                spawnedShuriken.GetComponent<ShurikenHitbox>();

            if (hitbox == null)
            {
                hitbox = spawnedShuriken.AddComponent<ShurikenHitbox>();
            }

            hitbox.damage = attackPower;
        }

        // 発動中
        if (isActiveState)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= activeDuration)
            {
                isActiveState = false;
                stateTimer = 0f;

                if (spawnedShuriken != null)
                {
                    spawnedShuriken.SetActive(false);
                }
            }
        }
        // クールタイム中
        else
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= coolDownDuration)
            {
                isActiveState = true;
                stateTimer = 0f;

                if (spawnedShuriken != null)
                {
                    spawnedShuriken.SetActive(true);
                }
            }

            return;
        }

        // 手裏剣の回転
        if (spawnedShuriken != null && spawnedShuriken.activeSelf)
        {
            currentAngle += rotationSpeed * Time.deltaTime;

            if (currentAngle >= 360f)
            {
                currentAngle -= 360f;
            }

            float rad = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * orbitRadius;
            float y = Mathf.Sin(rad) * orbitRadius;

            spawnedShuriken.transform.localPosition =
                new Vector3(x, y, 0f);

            spawnedShuriken.transform.Rotate(
                0f,
                0f,
                selfRotationSpeed * Time.deltaTime
            );
        }
    }
}

// =====================================
// 手裏剣の当たり判定
// =====================================

public class ShurikenHitbox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string objName = collision.name.ToLower();

        if (collision.CompareTag("Enemy") ||
            collision.CompareTag("Boss") ||
            objName.Contains("enemy") ||
            objName.Contains("boss") ||
            objName.Contains("中ボス"))
        {
            EnemyHealth enemyHealth =
                collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}
