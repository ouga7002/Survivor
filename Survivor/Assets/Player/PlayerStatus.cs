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

    [Header("スコア")]
    public int CurrentScore { get; private set; } = 0;

    // 現在のステータス
    public float CurrentAtk { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float MoveSpeed { get; private set; }

    // 攻撃力強化
    private float attackUpMultiplier = 1f;
    private float attackUpEndTime = 0f;

    void Awake()
    {
        CurrentAtk = baseAtk;
        MaxHp = baseHp;
        CurrentHp = baseHp;
        MoveSpeed = baseSpeed;
    }

    void Update()
    {
        // 攻撃力強化の終了
        if (attackUpMultiplier > 1f &&
            Time.time >= attackUpEndTime)
        {
            attackUpMultiplier = 1f;
            CurrentAtk = baseAtk;

            Debug.Log("攻撃力アップ終了");
        }
    }

    // GPを獲得
    public void AddGatyaPoint(int amount)
    {
        if (amount <= 0) return;

        CurrentGP += amount;

        Debug.Log(
            $"ガチャポイントを{amount}獲得！ 現在のガチャポイント：{CurrentGP}"
        );
    }

    // HPを回復
    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHp <= 0) return;

        CurrentHp += amount;

        if (CurrentHp > MaxHp)
        {
            CurrentHp = MaxHp;
        }

        Debug.Log(
            $"HPが{amount}回復！ 現在のHP：{CurrentHp}/{MaxHp}"
        );
    }

    // 攻撃力を一定時間上昇
    public void ApplyAttackUp(float percent, float duration)
    {
        if (percent <= 0 || duration <= 0) return;

        attackUpMultiplier = 1f + percent / 100f;

        CurrentAtk = baseAtk * attackUpMultiplier;

        attackUpEndTime = Time.time + duration;

        Debug.Log(
            $"攻撃力が{percent}%アップ！ 現在の攻撃力：{CurrentAtk}"
        );
    }

    // スコアを獲得
    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        CurrentScore += amount;

        Debug.Log($"スコアを{amount}獲得！ 現在のスコア：{CurrentScore}");
    }

    // ダメージを受ける
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

    // デバッグ確認用
    [ContextMenu("Debug Print Status")]
    public void PrintStatus()
    {
        Debug.Log(
            $"[PlayerStatus] HP: {CurrentHp}/{MaxHp} | ATK: {CurrentAtk} | SPEED: {MoveSpeed} | GP: {CurrentGP}"
        );
    }

    void Die()
    {
        Debug.Log("プレイヤーが死亡しました");
        Destroy(gameObject);
    }
}