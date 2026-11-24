using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("プレイヤーの参照")]
    [Tooltip("シーン内のPlayerStatusがついているオブジェクトをアタッチ")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("UI設定（TextMeshPro）")]
    [Tooltip("ガチャポイントを表示するテキスト")]
    [SerializeField] private TextMeshProUGUI gpText;

    [Tooltip("スコアを表示するテキスト")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("表示フォーマット")]
    [SerializeField] private string gpPrefix = "GP: ";
    [SerializeField] private string scorePrefix = "Score: ";

    void Update()
    {
        // プレイヤーが設定されていない場合は自動で探す
        if (playerStatus == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerStatus = playerObj.GetComponent<PlayerStatus>();
            }
            return;
        }

        // ガチャポイントの表示更新
        if (gpText != null)
        {
            gpText.text = gpPrefix + playerStatus.CurrentGP.ToString("N0");
        }

        // スコアの表示更新
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + playerStatus.CurrentScore.ToString("N0");
        }
    }
}