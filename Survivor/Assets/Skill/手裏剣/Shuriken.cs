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

        // 手裏剣の生成と当たり判定のセットアップ
        if (spawnedShuriken == null && shurikenPrefab != null)
        {
            spawnedShuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            spawnedShuriken.transform.SetParent(transform);

            // 生成した手裏剣プレハブに当たり判定コンポーネントを自動追加して攻撃力を渡す
            ShurikenHitbox hitbox = spawnedShuriken.GetComponent<ShurikenHitbox>();
            if (hitbox == null)
            {
                hitbox = spawnedShuriken.AddComponent<ShurikenHitbox>();
            }
            hitbox.damage = attackPower;
        }

        // アクティブとクールダウンの切り替え
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

        // 手裏剣の回転処理
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

// ★生成された手裏剣プレハブ側で敵との接触を検知するためのクラス
public class ShurikenHitbox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}