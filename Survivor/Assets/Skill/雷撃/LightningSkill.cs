using UnityEngine;
using System.Collections.Generic;

public class LightningSkill : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float baseLightningAtk = 20f;
    [SerializeField] private float attackInterval = 4.0f;
    [SerializeField] private float displayDuration = 0.5f;
    [SerializeField] private float attackRadius = 2.5f;

    public bool isUnlocked = false;

    private float timer = 0f;
    private PlayerStatus playerStatus;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        timer = 0f;
    }

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

    void StrikeLightning()
    {
        if (lightningPrefab == null) return;

        // ★敵（Enemy）とボス（Boss）の両方からターゲット候補を集める
        List<GameObject> targets = new List<GameObject>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null) targets.AddRange(enemies);

        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        if (bosses != null) targets.AddRange(bosses);

        // もしタグで見つからない場合の保険として名前検索も追加
        foreach (GameObject obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            string objName = obj.name.ToLower();
            if ((objName.Contains("enemy") || objName.Contains("boss") || objName.Contains("中ボス")) && !targets.Contains(obj))
            {
                // プレイヤー自身や弾などは除外する
                if (obj != gameObject && !obj.name.Contains("Player"))
                {
                    targets.Add(obj);
                }
            }
        }

        if (targets.Count == 0) return;

        // 候補の中からランダムに1体選ぶ
        GameObject targetEnemy = targets[Random.Range(0, targets.Count)];

        if (targetEnemy != null)
        {
            Vector3 spawnPos = targetEnemy.transform.position;
            GameObject lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            float totalAtk = baseLightningAtk;
            if (playerStatus != null) totalAtk += playerStatus.CurrentAtk;

            // 雷の爆発範囲（円内）にいる敵やボス全員にダメージを与える
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPos, attackRadius);
            foreach (var hit in hitColliders)
            {
                string hitName = hit.name.ToLower();

                // ★敵またはボス、あるいは名前やタグにそれらを含むものを対象にする
                if (hit.CompareTag("Enemy") || hit.CompareTag("Boss") ||
                    hitName.Contains("enemy") || hitName.Contains("boss") || hitName.Contains("中ボス"))
                {
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(totalAtk);
                    }
                }
            }

            Destroy(lightning, displayDuration);
        }
    }
}