using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // どこからでも呼び出せるようにする（シングルトン）
    public static ScoreManager Instance { get; private set; }

    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI scoreText; // スコアを表示するTextMeshPro

    [Header("スコアデータ")]
    [SerializeField] private int currentScore = 0;

    public int CurrentScore => currentScore;

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

    // スコアを加算するメソッド
    public void AddScore(int amount)
    {
        currentScore += amount;
    }
}