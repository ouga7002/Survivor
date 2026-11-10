using System.Collections.Generic;
using UnityEngine;

// 敵の攻撃スクリプトが実装する共通機能
public interface IEnemyAttack
{
    void StopAttack();
}

public class EnemyAttackManager : MonoBehaviour
{
    // この敵が生成した弾
    private List<GameObject> activeBullets = new List<GameObject>();

    // 弾を登録
    public void RegisterBullet(GameObject bullet)
    {
        if (bullet != null)
        {
            activeBullets.Add(bullet);
        }
    }

    // すべての攻撃を停止
    public void StopAllAttacks()
    {
        // 敵に付いている攻撃スクリプトを停止
        MonoBehaviour[] components =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IEnemyAttack attack)
            {
                attack.StopAttack();
            }
        }

        // 生成済みの弾をすべて削除
        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }

        activeBullets.Clear();

        // 敵の移動を停止
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
