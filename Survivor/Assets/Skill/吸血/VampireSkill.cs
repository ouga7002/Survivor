using UnityEngine;

public class VampireSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("回復時にプレイヤーの頭上などに表示するエフェクトのプレハブ（任意）")]
    [SerializeField] private GameObject healEffectPrefab;

    [Header("吸血パラメータ")]
    [Tooltip("通常武器のヒット1回あたりの固定回復量")]
    [SerializeField] private float healAmount = 1f;

    private PlayerStatus playerStatus;

    void Start()
    {
        // プレイヤーのステータス取得
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null)
        {
            playerStatus = pObj.GetComponent<PlayerStatus>();
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                playerStatus = taggedPlayer.GetComponent<PlayerStatus>();
            }
        }
    }

    // 各武器のヒット時に呼び出される吸血処理
    public void OnWeaponHit(float dealtDamage)
    {
        if (playerStatus == null) return;

        // プレイヤーのHPを固定値（1）で回復
        playerStatus.Heal(healAmount);

        // 回復エフェクトの生成
        if (healEffectPrefab != null)
        {
            // 1. プレイヤーの現在位置にエフェクトを生成
            GameObject effect = Instantiate(healEffectPrefab, playerStatus.transform.position, Quaternion.identity);

            // 2. ★プレイヤーの子要素に設定して、移動に追従させる
            effect.transform.SetParent(playerStatus.transform);

            // 3. 1秒後に自動で消去
            Destroy(effect, 1.0f);
        }
    }
}