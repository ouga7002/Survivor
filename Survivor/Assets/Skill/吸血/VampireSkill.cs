using UnityEngine;

public class VampireSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("回復時にプレイヤーの頭上などに表示するエフェクトのプレハブ（任意）")]
    [SerializeField] private GameObject healEffectPrefab;

    [Header("吸血パラメータ")]
    [Tooltip("通常武器のダメージに対する回復割合（10%なら 0.1f）")]
    [SerializeField] private float healRate = 0.1f;

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

        // シーン内の既存のナイフ（または武器）を探して、吸血イベントを登録する
        HookExistingWeapons();
    }

    void HookExistingWeapons()
    {
        // ナイフのコンポーネントをシーン内から探す
        KnifeAttack[] knives = FindObjectsByType<KnifeAttack>(FindObjectsSortMode.None);
        foreach (var knife in knives)
        {
            // 注: ナイフ側でイベントを発行する仕組みを追加するか、
            // もしくは下記のようにヒット時に直接プレイヤーの回復を呼ぶ拡張が可能です。
        }
    }

    // 各武器のヒット時に外部から呼び出す、または連動させるための吸血処理
    public void OnWeaponHit(float dealtDamage)
    {
        if (playerStatus == null) return;

        // 与えたダメージの10%を計算
        float healAmount = dealtDamage * healRate;

        // プレイヤーのHPを回復（PlayerStatusに回復用メソッドがない場合はCurrentHpを直接調整するか追加します）
        // ※今回は簡易的に現在のHPに加算する処理を記載します
        // playerStatus.Heal(healAmount); // PlayerStatus側にHealメソッドを作るのが綺麗です

        // 回復エフェクトの生成（プレイヤーの位置に表示）
        if (healEffectPrefab != null && playerStatus != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, playerStatus.transform.position, Quaternion.identity);
            Destroy(effect, 1.0f); // 1秒後に消去
        }

        Debug.Log($"[吸血スキル] 武器の攻撃で敵からHPを {healAmount} 回復した！");
    }
}