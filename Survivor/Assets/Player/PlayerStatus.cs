using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("初期ステータス")]
    [Tooltip("攻撃力（初期値: 100）")]
    [SerializeField] private float baseAtk = 100f;

    [Tooltip("最大体力（初期値: 100）")]
    [SerializeField] private float baseHp = 100f;

    [Tooltip("移動速度（初期値: 5）")]
    [SerializeField] private float baseSpeed = 5f;

    [Header("ガチャポイント")]
    public int CurrentGP { get; private set; } = 0;

    // 現在のステータス
    public float CurrentAtk { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float MoveSpeed { get; private set; }

    void Awake()
    {
        CurrentAtk = baseAtk;
        MaxHp = baseHp;
        CurrentHp = baseHp;
        MoveSpeed = baseSpeed;
    }

    // 経験値を獲得
    public void AddGatyaPoint(int amount)
    {
        if (amount <= 0) return;

        CurrentGP += amount;

        Debug.Log($"ガチャポイントを{amount}獲得！ 現在のガチャポイント：{CurrentGP}");
    }

    // デバッグ確認用
    [ContextMenu("Debug Print Status")]
    public void PrintStatus()
    {
        Debug.Log($"[PlayerStatus] HP: {CurrentHp}/{MaxHp} | ATK: {CurrentAtk} | SPEED: {MoveSpeed} | GP: {CurrentGP}");
    }

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;

        if (CurrentHp < 0)
        {
            CurrentHp = 0;
        }

        Debug.Log("プレイヤーHP：" + CurrentHp);

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    // ★【追加】HPを回復するメソッド（吸血スキル等から呼び出されます）
    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHp <= 0) return;

        CurrentHp += amount;

        // 最大HPを超えないように制限する
        if (CurrentHp > MaxHp)
        {
            CurrentHp = MaxHp;
        }

        Debug.Log($"[PlayerStatus] HPが {amount} 回復した！ 現在のHP：{CurrentHp}/{MaxHp}");
    }

    void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        Destroy(gameObject);
    }
}