using UnityEngine;

public class TreasureHuntMission : BaseEventMission
{
    [Header("レアモンスター討伐固有の設定")]
    [SerializeField] private GameObject treasureEnemyPrefab;
    [SerializeField] private float spawnDistance = 6f;

    private GameObject spawnedTreasureEnemy;

    protected override void OnMissionStart()
    {
        Vector3 spawnPos = Vector3.zero;
        if (playerTransform != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized * spawnDistance;
            spawnPos = playerTransform.position + new Vector3(randomDir.x, randomDir.y, 0f);
        }

        if (treasureEnemyPrefab != null)
        {
            spawnedTreasureEnemy = Instantiate(treasureEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    protected override void OnMissionUpdate()
    {
        // 逃げ回るレアモンスターを倒したらクリア
        if (spawnedTreasureEnemy == null)
        {
            CompleteMission();
        }
    }

    protected override void OnMissionEnd()
    {
        // 時間切れで失敗した場合は逃走（消滅）させる
        if (spawnedTreasureEnemy != null)
        {
            Destroy(spawnedTreasureEnemy);
        }
    }
}