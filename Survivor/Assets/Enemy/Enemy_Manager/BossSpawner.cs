using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnData
    {
        public GameObject bossPrefab;

        [Tooltip("ゲーム開始から何秒後に出現するか")]
        public float spawnTime = 120f;

        [HideInInspector]
        public bool hasSpawned = false;
    }

    [Header("ボスの出現設定")]
    public BossSpawnData[] bosses;

    [Header("プレイヤーからの出現距離")]
    public float minSpawnDistance = 7f;
    public float maxSpawnDistance = 10f;

    [Header("敵プール")]
    public EnemyPool enemyPool;

    // ボスが生存しているかどうか
    public static bool IsBossActive { get; private set; } = false;

    private Transform player;
    private GameObject currentBoss;
    private float elapsedTime;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Playerタグのオブジェクトが見つかりません");
        }
    }

    void Update()
    {
        if (player == null || enemyPool == null) return;

        // ボスが出現中なら撃破されたか確認
        if (IsBossActive)
        {
            if (currentBoss == null || !currentBoss.activeInHierarchy)
            {
                IsBossActive = false;
                currentBoss = null;

                Debug.Log("ボス撃破！ゲームを再開します");
            }
            else
            {
                return;
            }
        }

        // ボスがいない間だけ時間を進める
        elapsedTime += Time.deltaTime;

        foreach (BossSpawnData data in bosses)
        {
            if (data.hasSpawned) continue;
            if (data.bossPrefab == null) continue;

            if (elapsedTime < data.spawnTime) continue;

            SpawnBoss(data);
            data.hasSpawned = true;

            // 1体出現したら、このフレームの処理を終了
            break;
        }
    }

    void SpawnBoss(BossSpawnData data)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float distance =
            Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector2 spawnPosition =
            (Vector2)player.position + direction * distance;

        currentBoss = enemyPool.Spawn(
            data.bossPrefab,
            spawnPosition
        );

        if (currentBoss != null)
        {
            IsBossActive = true;
            Debug.Log(data.bossPrefab.name + " が出現！");
        }
    }
}
