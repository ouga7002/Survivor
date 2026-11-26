using UnityEngine;

public class TreasureHuntMission : BaseEventMission
{
    [Header("レアモンスター討伐固有の設定")]
    [SerializeField] private GameObject treasureEnemyPrefab;
    [SerializeField] private GameObject arrowUIPrefab; // 矢印のUIプレハブをここに設定
    [SerializeField] private float spawnDistance = 6f;

    private GameObject spawnedTreasureEnemy;
    private GameObject arrowIndicatorObj;

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

            // 逃げる敵が生成されたら、矢印インジケーターをアタッチして追跡を開始する
            if (arrowUIPrefab != null)
            {
                arrowIndicatorObj = new GameObject("ArrowIndicator_Controller");
                MissionArrowIndicator indicator = arrowIndicatorObj.AddComponent<MissionArrowIndicator>();
                // ※インスペクターからプレハブを渡せない場合の簡易的なセットアップ用拡張も可能ですが、
                // 上記スクリプトの [SerializeField] private GameObject arrowUIPrefab にプレハブをセットしておく形が一番簡単です。
            }
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
        // 矢印インジケーターを削除
        if (arrowIndicatorObj != null)
        {
            Destroy(arrowIndicatorObj);
        }

        // 時間切れで失敗した場合は逃走（消滅）させる
        if (spawnedTreasureEnemy != null)
        {
            Destroy(spawnedTreasureEnemy);
        }
    }
}