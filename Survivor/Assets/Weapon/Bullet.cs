using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float lifeTime = 3.0f; // 画面外に出たとき用の消滅タイマー

    private Vector3 moveDir;
    private float damage;
    private bool hasHit = false;

    // 発射時にGunAttackから呼び出される初期化メソッド
    public void Initialize(Vector3 direction, float attackPower)
    {
        moveDir = direction;
        damage = attackPower;
        Destroy(gameObject, lifeTime); // 一定時間で自動消滅
    }

    void Update()
    {
        // まだ何にも当たっていなければ直進する
        if (!hasHit)
        {
            transform.position += moveDir * bulletSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        string objName = collision.name.ToLower();

        // 敵（Enemy）またはボス（Boss）に当たった場合
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") ||
            objName.Contains("enemy") || objName.Contains("boss") || objName.Contains("中ボス"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                // 吸血スキルへの通知（必要に応じて）
                VampireSkill vampire = FindFirstObjectByType<VampireSkill>();
                if (vampire != null)
                {
                    vampire.OnWeaponHit(damage);
                }

                hasHit = true;
                Destroy(gameObject); // 貫通させずに即座に消滅
            }
        }
    }
}