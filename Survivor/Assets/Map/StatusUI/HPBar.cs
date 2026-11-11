using UnityEngine;
using UnityEngine.UI; // Imageを使うために必要

public class PlayerHPBarLayered : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーのPlayerStatus
    [SerializeField] private Image fillImage;         // 現在のHPを示すImage（Fillタイプ）

    void Start()
    {
        // アサイン忘れ防止の自動取得
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    void Update()
    {
        if (playerStatus != null && fillImage != null)
        {
            // 1. HPの割合（0.0 〜 1.0）を計算
            float hpRate = (float)playerStatus.CurrentHp / playerStatus.MaxHp;

            // 2. 現在のHPバーの長さを更新
            fillImage.fillAmount = hpRate;
        }
    }
}