using UnityEngine;
using TMPro; // TextMeshProを使用する場合

public class PlayerHPUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーのPlayerStatusをアサイン
    [SerializeField] private TextMeshProUGUI hpText;  // HPを表示するTextMeshPro

    void Update()
    {
        // プレイヤーとテキストが正しく設定されているか確認
        if (playerStatus != null && hpText != null)
        {
            // 現在のHPと最大HPを取得してテキストに反映
            // 例: "HP: 100 / 100" のように表示されます
            hpText.text = $"HP: {playerStatus.CurrentHp} / {playerStatus.MaxHp}";
        }
    }
}