using UnityEngine;
using TMPro; // TextMeshProを使用する場合

public class CountdownTimer : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI timerText; // 画面に表示するテキスト

    [Header("タイマー設定")]
    [SerializeField] private float totalTime = 300f; // 5分 = 300秒

    private float currentTime;
    private bool isRunning = true;

    private void Start()
    {
        // タイマーの残り時間を初期化（5分に設定）
        currentTime = totalTime;
    }

    private void Update()
    {
        if (!isRunning) return;

        // 時間を減らす
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // 0未満にならないように制限
            if (currentTime < 0)
            {
                currentTime = 0;
                TimerFinished();
            }
        }

        // 画面のテキストを更新
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        // 分と秒を計算
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // 「05:00」のような形式にフォーマットしてテキストに代入
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TimerFinished()
    {
        isRunning = false;
        Debug.Log("タイムアップ！");
        // ここに時間切れになったときの処理（ゲームオーバー等）を書けます
    }
}