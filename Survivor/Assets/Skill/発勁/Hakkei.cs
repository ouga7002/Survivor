
using UnityEngine;

public class Hakkei : MonoBehaviour, ISkillUnlockable
{
    [Header("参照設定")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform aimCursor;

    [Header("発勁の設定")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackAngle = 180f;
    [SerializeField] private int damage = 1;

    [Header("ノックバック設定")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("クールタイム")]
    [SerializeField] private float cooldown = 5f;

    private bool isUnlocked = false;
    private float nextUseTime = 0f;

    // ガチャ管理用のスキルID
    public string SkillId => "hakkei";

    private void Start()
    {
        // プレイヤーを自動取得
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // 照準カーソルを自動取得
        if (aimCursor == null)
        {
            GameObject cursorObject =
                GameObject.Find("AimCursor");

            if (cursorObject != null)
            {
                aimCursor = cursorObject.transform;
            }
        }
    }

    private void Update()
    {
        if (!isUnlocked)
            return;

        if (Time.time < nextUseTime)
            return;

        if (player == null || aimCursor == null)
            return;

        UseHakkei();

        nextUseTime = Time.time + cooldown;
    }

    // ガチャによる解放・未解放の切り替え
    public void SetSkillUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        {
            // 解放直後に発動できるようにする
            nextUseTime = Time.time;
        }
    }

    private void UseHakkei()
    {
        // 照準方向を取得
        Vector2 aimDirection =
            ((Vector2)aimCursor.position -
             (Vector2)player.position).normalized;

        if (aimDirection == Vector2.zero)
            return;

        // ノックバック対象を検索
        KnockbackReceiver[] enemies =
            FindObjectsOfType<KnockbackReceiver>();

        foreach (KnockbackReceiver enemy in enemies)
        {
            if (enemy == null)
                continue;

            // 石化中の敵は対象外
            Petrifiable petrifiable =
                enemy.GetComponent<Petrifiable>();

            if (petrifiable != null &&
                petrifiable.IsPetrified())
            {
                continue;
            }

            Vector2 enemyPosition = enemy.transform.position;

            // プレイヤーから敵までの方向と距離
            Vector2 toEnemy =
                enemyPosition - (Vector2)player.position;

            float distance = toEnemy.magnitude;

            // 攻撃範囲外なら対象外
            if (distance > attackRange)
                continue;

            // 前方180度の範囲を判定
            float angle =
                Vector2.Angle(aimDirection, toEnemy);

            if (angle > attackAngle / 2f)
                continue;

            // ダメージを与える
            enemy.SendMessageUpwards(
                "TakeDamage",
                damage,
                SendMessageOptions.DontRequireReceiver
            );

            // ノックバック方向を計算
            Vector2 knockbackDirection =
                toEnemy.normalized;

            // 敵を吹き飛ばす
            enemy.ApplyKnockback(
                knockbackDirection,
                knockbackForce,
                knockbackDuration
            );
        }
    }
}
