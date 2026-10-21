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

    // 現在のステータス（外部の攻撃処理や移動処理から参照するためのプロパティ）
    public float CurrentAtk { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float MoveSpeed { get; private set; }

    void Awake()
    {
        // 初期値のセット
        CurrentAtk = baseAtk;
        MaxHp = baseHp;
        CurrentHp = baseHp; // 開始時はHP全回復状態
        MoveSpeed = baseSpeed;
    }

    // デバッグ確認用（インスペクターのコンテキストメニューから確認可能）
    [ContextMenu("Debug Print Status")]
    public void PrintStatus()
    {
        Debug.Log($"[PlayerStatus] HP: {CurrentHp}/{MaxHp} | ATK: {CurrentAtk} | SPEED: {MoveSpeed}");
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

    void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
    }
}