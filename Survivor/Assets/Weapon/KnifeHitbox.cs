using UnityEngine;

public class KnifeHitbox : MonoBehaviour
{
    [Header("ナイフの基本攻撃力（KnifeAttack側で管理する場合はそのままでOK）")]
    public float baseWeaponAtk = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string objName = collision.name.ToLower();

        // 敵（Enemy）またはボス（Boss）に当たったときの処理
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") ||
            objName.Contains("enemy") || objName.Contains("boss") || objName.Contains("中ボス"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // プレイヤーのステータスを取得してダメージ計算
                PlayerStatus playerStatus = FindFirstObjectByType<PlayerStatus>();
                float totalAtk = baseWeaponAtk;
                if (playerStatus != null)
                {
                    totalAtk += playerStatus.CurrentAtk;
                }

                // ダメージを与える
                enemyHealth.TakeDamage(totalAtk);

                // 吸血スキルへの通知
                VampireSkill vampire = FindFirstObjectByType<VampireSkill>();
                if (vampire != null)
                {
                    vampire.OnWeaponHit(totalAtk);
                }

                Debug.Log($"[攻撃成功] ナイフが {collision.name} にヒット！ 総ダメージ: {totalAtk}");
            }
        }
    }
}