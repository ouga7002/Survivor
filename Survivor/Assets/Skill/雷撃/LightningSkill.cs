using UnityEngine;

public class LightningSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("雷撃として表示するオブジェクトのプレハブ")]
    [SerializeField] private GameObject lightningPrefab;

    [Header("攻撃パラメータ")]
    [Tooltip("雷撃自体の基本攻撃力（ATK: 20）")]
    [SerializeField] private float baseLightningAtk = 20f;

    [Tooltip("発動クールタイム（秒）")]
    [SerializeField] private float attackInterval = 4.0f;

    [Tooltip("雷撃が表示されている時間（秒）")]
    [SerializeField] private float displayDuration = 0.5f;

    [Tooltip("雷撃の攻撃範囲（半径）")]
    [SerializeField] private float attackRadius = 2.5f;

    private float timer = 0f;
    private Transform playerTransform;
    private PlayerStatus playerStatus;

    void Start()
    {
        // プレイヤーの取得とステータス参照の取得（手裏剣と同じ安全な検索処理）
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null)
        {
            playerTransform = pObj.transform;
            playerStatus = pObj.GetComponent<PlayerStatus>();
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                playerTransform = taggedPlayer.transform;
                playerStatus = taggedPlayer.GetComponent<PlayerStatus>();
            }
            else
            {
                Debug.LogError("【LightningSkill】プレイヤーが見つかりませんでした！");
            }
        }
    }

    void Update()
    {
        // プレイヤーが見つからない場合は処理しない
        if (playerTransform == null) return;

        // タイマーを進める
        timer += Time.deltaTime;

        // 4秒経過したら雷撃を発動
        if (timer >= attackInterval)
        {
            timer = 0f;
            StrikeLightning();
        }
    }

    void StrikeLightning()
    {
        if (lightningPrefab == null)
        {
            Debug.LogError("【LightningSkill】Lightning Prefab が設定されていません！");
            return;
        }

        // 1. シーン上にある「Enemy」タグのついたオブジェクトをすべて取得する
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            return;
        }

        // 2. 敵の中からランダムで1体選んで、その位置を雷の発生中心にする
        GameObject targetEnemy = enemies[Random.Range(0, enemies.Length)];

        if (targetEnemy != null)
        {
            Vector3 spawnPos = targetEnemy.transform.position;

            // 3. 雷撃のエフェクトを生成
            GameObject lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            // 4. 最終ダメージの計算（基礎ATK ＋ プレイヤーATK）
            float totalAtk = baseLightningAtk;
            if (playerStatus != null)
            {
                totalAtk += playerStatus.CurrentAtk;
            }

            // 5. 指定した半径（attackRadius）の円形範囲内にいるすべてのコライダーを検知する
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPos, attackRadius);
            int hitCount = 0;

            foreach (Collider2D hit in hitColliders)
            {
                // 敵タグ、または名前に "enemy" が含まれるオブジェクトが対象
                if (hit.CompareTag("Enemy") || hit.name.ToLower().Contains("enemy"))
                {
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(totalAtk);
                        hitCount++;
                    }
                }
            }

            Debug.Log($"[雷撃発動] 半径 {attackRadius} の範囲に雷が落ち、敵 {hitCount} 体に {totalAtk} のダメージを与えた！");

            // 6. 指定時間経過したら自動でエフェクトを消去する
            Destroy(lightning, displayDuration);
        }
    }
}