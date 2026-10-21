
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵のステータス")]
    public float maxHP = 10f;

    public float CurrentHP { get; private set; }

    private bool isDead = false;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    // ダメージを受ける処理
    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        CurrentHP -= damage;

        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }

        Debug.Log($"{gameObject.name} HP: {CurrentHP}/{maxHP}");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log($"{gameObject.name} が倒された！");

        Destroy(gameObject);
    }
}
