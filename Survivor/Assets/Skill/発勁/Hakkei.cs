
using UnityEngine;

public class Hakkei : MonoBehaviour, ISkillUnlockable
{
    [Header("参照設定")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform aimCursor;

    [Header("衝撃波設定")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float projectileScale = 2f;

    [Header("攻撃設定")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("クールタイム")]
    [SerializeField] private float cooldown = 5f;

    private bool isUnlocked = false;
    private float nextUseTime = 0f;

    public string SkillId => "hakkei";

    private void Start()
    {
        // プレイヤーを取得
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        // 照準カーソルを取得
        if (aimCursor == null)
        {
            GameObject cursorObject =
                GameObject.Find("AimCursor");

            if (cursorObject != null)
                aimCursor = cursorObject.transform;
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

        if (shockwavePrefab == null)
            return;

        UseHakkei();
        nextUseTime = Time.time + cooldown;
    }

    // ガチャによる解放
    public void SetSkillUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        {
            nextUseTime = Time.time;
        }
    }

    // 発勁を発射
    private void UseHakkei()
    {
        Vector2 direction =
            ((Vector2)aimCursor.position -
             (Vector2)player.position).normalized;

        if (direction == Vector2.zero)
            return;

        // プレイヤーの前方に生成
        Vector3 spawnPosition =
            player.position +
            (Vector3)(direction * spawnDistance);

        // 衝撃波の向きを照準方向に合わせる
        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        Quaternion rotation =
            Quaternion.Euler(0f, 0f, angle);

        GameObject projectile = Instantiate(
            shockwavePrefab,
            spawnPosition,
            rotation
        );

        projectile.transform.localScale =
            Vector3.one * projectileScale;

        // 衝撃波の初期設定
        HakkeiProjectile projectileScript =
            projectile.GetComponent<HakkeiProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(
                direction,
                projectileSpeed,
                projectileLifetime,
                damage,
                knockbackForce,
                knockbackDuration
            );
        }
        else
        {
            Debug.LogError(
                "衝撃波PrefabにHakkeiProjectileがありません。"
            );

            Destroy(projectile);
        }
    }
}
