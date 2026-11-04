using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("タイマー設定")]
    [SerializeField] private float totalTime = 300f;

    private float currentTime;
    private bool isRunning = true;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        // ボス出現中はタイマーを停止
        if (BossSpawner.IsBossActive) return;

        // 時間を減らす
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // 0未満にならないように制限
            if (currentTime <= 0)
            {
                currentTime = 0;
                TimerFinished();
            }
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        if (timerText != null)
        {
            timerText.text =
                string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TimerFinished()
    {
        isRunning = false;
        Debug.Log("タイムアップ！");
        // ここに時間切れになったときの処理を書く
    }
}
