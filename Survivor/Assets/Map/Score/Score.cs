using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    // どこからでも呼び出せるようにする（シングルトン）
    public static ScoreManager Instance { get; private set; }

    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI scoreText; // スコアを表示するTextMeshPro

    [Header("スコアデータ")]
    [SerializeField] private int currentScore = 0;

    public int CurrentScore => currentScore;

    // ── 敵ごとのスコアを管理するためのデータ構造 ──
    [System.Serializable]
    public struct EnemyScoreData
    {
        [Tooltip("敵の名前（例: enemy1, enemy2, Boss など。`(Clone)` が付いていても自動で無視されます）")]
        public string enemyName;
        [Tooltip("その敵を倒したときのスコア")]
        public int score;
    }

    [Header("敵ごとのスコア設定リスト（ここで一括管理）")]
    [SerializeField] private List<EnemyScoreData> enemyScoreList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // テキストがアサインされていれば自動で更新する
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    // スコアを直接加算するメソッド
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    // ── 敵の名前からリストを引いて自動でスコアを加算するメソッド ──
    public void AddScoreByEnemyName(string rawEnemyName)
    {
        // 生成時に付く "(Clone)" や前後の空白を自動で削除して綺麗にする
        string cleanName = rawEnemyName.Replace("(Clone)", "").Trim();

        // リストから一致する敵の名前を探す
        foreach (var data in enemyScoreList)
        {
            if (data.enemyName == cleanName)
            {
                currentScore += data.score;
                Debug.Log($"{cleanName} を倒した！ +{data.score}点 (現在のスコア: {currentScore})");
                return;
            }
        }

        // リストに登録されていない場合の警告
        Debug.LogWarning($"未登録の敵ネームです: 「{cleanName}」（元の名前: 「{rawEnemyName}」）。ScoreManagerのリストを確認してください。");
    }
}