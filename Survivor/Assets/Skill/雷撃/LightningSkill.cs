
using UnityEngine;
using System.Collections.Generic;

public class LightningSkill : MonoBehaviour, ISkillUnlockable
{
    [Header("雷撃設定")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float baseLightningAtk = 20f;
    [SerializeField] private float attackInterval = 4.0f;
    [SerializeField] private float displayDuration = 0.5f;
    [SerializeField] private float attackRadius = 2.5f;

    public bool isUnlocked = false;

    private float timer = 0f;
    private PlayerStatus playerStatus;

    // =====================================
    // 共通スキル管理
    // =====================================

    public string SkillId => "lightning";

    public void SetSkillUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
    }

    // =====================================
    // 初期設定
    // =====================================

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        timer = 0f;
    }

    // =====================================
    // 更新処理
    // =====================================

    void Update()
    {
        if (!isUnlocked) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            StrikeLightning();
        }
    }

    // =====================================
    // 雷撃攻撃
    // =====================================

    void StrikeLightning()
    {
        if (lightningPrefab == null) return;

        List<GameObject> targets = new List<GameObject>();

        // 通常の敵
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies != null)
        {
            targets.AddRange(enemies);
        }

        // ボス
        GameObject[] bosses =
            GameObject.FindGameObjectsWithTag("Boss");

        if (bosses != null)
        {
            targets.AddRange(bosses);
        }

        // 名前による検索
        foreach (GameObject obj in
            GameObject.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None))
        {
            string objName = obj.name.ToLower();

            if ((objName.Contains("enemy") ||
                 objName.Contains("boss") ||
                 objName.Contains("中ボス")) &&
                !targets.Contains(obj))
            {
                if (obj != gameObject &&
                    !obj.name.Contains("Player"))
                {
                    targets.Add(obj);
                }
            }
        }

        if (targets.Count == 0) return;

        // ランダムにターゲットを選択
        GameObject targetEnemy =
            targets[Random.Range(0, targets.Count)];

        if (targetEnemy == null) return;

        Vector3 spawnPos = targetEnemy.transform.position;

        // 雷エフェクト生成
        GameObject lightning = Instantiate(
            lightningPrefab,
            spawnPos,
            Quaternion.identity
        );

        // 攻撃力計算
        float totalAtk = baseLightningAtk;

        if (playerStatus != null)
        {
            totalAtk += playerStatus.CurrentAtk;
        }

        // 雷撃範囲内の敵を検索
        Collider2D[] hitColliders =
            Physics2D.OverlapCircleAll(
                spawnPos,
                attackRadius
            );

        foreach (var hit in hitColliders)
        {
            string hitName = hit.name.ToLower();

            if (hit.CompareTag("Enemy") ||
                hit.CompareTag("Boss") ||
                hitName.Contains("enemy") ||
                hitName.Contains("boss") ||
                hitName.Contains("中ボス"))
            {
                EnemyHealth enemyHealth =
                    hit.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(totalAtk);
                }
            }
        }

        Destroy(lightning, displayDuration);
    }
}
