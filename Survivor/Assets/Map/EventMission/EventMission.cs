using UnityEngine;

public class EliteEventController : MonoBehaviour
{
    [Header("イベント発生設定")]
    [Tooltip("ゲーム開始から何秒後に精鋭を出現させるか")]
    [SerializeField] private float triggerTime = 60f;

    [Tooltip("精鋭が出現してから討伐までの制限時間（秒）")]
    [SerializeField] private float timeLimit = 30f;

    [Header("精鋭モンスターのプレハブ")]
    [SerializeField] private GameObject eliteEnemyPrefab;

    [Header("プレイヤーの参照")]
    [SerializeField] private Transform playerTransform;

    private bool isEventTriggered = false;
    private bool isEventActive = false;
    private float elapsedGameTime = 0f;
    private float currentLimitTimer = 0f;
    private GameObject spawnedElite;

    void Update()
    {
        // 1. まだイベントが発生していない場合、時間をカウント
        if (!isEventTriggered)
        {
            // ボス戦中などはタイマーを止めたい場合はここに条件を追加できます
            elapsedGameTime += Time.deltaTime;

            if (elapsedGameTime >= triggerTime)
            {
                StartEliteEvent();
            }
            return;
        }

        // 2. イベント発生中の処理（制限時間カウント）
        if (isEventActive)
        {
            currentLimitTimer -= Time.deltaTime;

            // 成功判定：精鋭が倒されて消滅した場合
            if (spawnedElite == null)
            {
                CompleteEliteEvent();
                return;
            }

            // 失敗判定：制限時間がゼロになった場合
            if (currentLimitTimer <= 0f)
            {
                FailEliteEvent();
            }
        }
    }

    void StartEliteEvent()
    {
        isEventTriggered = true;
        isEventActive = true;
        currentLimitTimer = timeLimit;

        Debug.Log("【イベントミッション】制限時間内に精鋭モンスターを撃破せよ！");

        // プレイヤーの近くに出現させる位置を計算
        Vector3 spawnPos = Vector3.zero;
        if (playerTransform != null)
        {
            // プレイヤーの周囲（ランダムな方向の少し離れた位置）
            Vector2 randomDir = Random.insideUnitCircle.normalized * 7f;
            spawnPos = playerTransform.position + new Vector3(randomDir.x, randomDir.y, 0f);
        }

        // 精鋭モンスターをスポーン
        if (eliteEnemyPrefab != null)
        {
            spawnedElite = Instantiate(eliteEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    void CompleteEliteEvent()
    {
        isEventActive = false;
        Debug.Log("【ミッションクリア！】精鋭討伐成功！ガチャポイントや報酬を獲得！");

        // TODO: ここに報酬を付与する処理を書く（例: ガチャポイント加算など）
    }

    void FailEliteEvent()
    {
        isEventActive = false;
        Debug.Log("【ミッション失敗...】時間切れで精鋭が逃げ出した！");

        // 時間切れになったら精鋭を消す（残したい場合は消さなくてもOKです）
        if (spawnedElite != null)
        {
            Destroy(spawnedElite);
        }
    }
}