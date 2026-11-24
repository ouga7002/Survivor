using UnityEngine;

public class EliteHuntMission : BaseEventMission
{
    [Header("精鋭討伐固有の設定")]
    [SerializeField] private GameObject eliteEnemyPrefab;
    [SerializeField] private float spawnDistance = 7f;

    private GameObject spawnedElite;

    protected override void OnMissionStart()
    {
        Vector3 spawnPos = Vector3.zero;
        if (playerTransform != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized * spawnDistance;
            spawnPos = playerTransform.position + new Vector3(randomDir.x, randomDir.y, 0f);
        }

        if (eliteEnemyPrefab != null)
        {
            spawnedElite = Instantiate(eliteEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    protected override void OnMissionUpdate()
    {
        // 精鋭が倒されたらクリア
        if (spawnedElite == null)
        {
            CompleteMission();
        }
    }

    protected override void OnMissionEnd()
    {
        // 失敗時など、まだ精鋭が残っていれば消す
        if (spawnedElite != null)
        {
            Destroy(spawnedElite);
        }
    }
}