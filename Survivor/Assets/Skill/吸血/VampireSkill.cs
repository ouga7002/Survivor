using UnityEngine;

public class VampireSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("回復時にプレイヤーの頭上などに表示するエフェクトのプレハブ（任意）")]
    [SerializeField] private GameObject healEffectPrefab;

    [Header("吸血パラメータ")]
    [Tooltip("通常武器のヒット1回あたりの固定回復量")]
    [SerializeField] private float healAmount = 1f;

    [Tooltip("エフェクトの表示サイズ")]
    [SerializeField] private Vector3 effectScale = new Vector3(1f, 1f, 1f);

    [Header("表示順（描画順）設定")]
    [Tooltip("プレイヤーよりどれくらい手前に表示するか（Order in Layerのプラス分）")]
    [SerializeField] private int sortingOrderOffset = 10;

    private Transform playerTransform;
    private PlayerStatus playerStatus;

    // 同一フレーム内でのエフェクト重複生成を防ぐためのフラグ
    private static bool isEffectSpawnedThisFrame = false;

    void Start()
    {
        // プレイヤーの取得とステータス参照の取得
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null)
        {
            playerTransform = pObj.transform;
            playerStatus = pObj.GetComponent<PlayerStatus>();
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                playerTransform = taggedPlayer.transform;
                playerStatus = taggedPlayer.GetComponent<PlayerStatus>();
            }
        }
    }

    void LateUpdate()
    {
        // フレームの終わりに、フラグをリセットする
        isEffectSpawnedThisFrame = false;
    }

    // 各武器のヒット時に呼び出される吸血処理
    public void OnWeaponHit(float dealtDamage)
    {
        if (playerStatus == null) return;

        // 1. HPはヒットした敵の数だけしっかりと回復させる
        playerStatus.Heal(healAmount);

        // 2. 「同じフレーム内」ですでにエフェクトを出していたら、生成を完全にスキップする
        if (isEffectSpawnedThisFrame) return;

        // 3. エフェクトの生成
        if (healEffectPrefab != null && playerTransform != null)
        {
            isEffectSpawnedThisFrame = true; // このフレームでの生成をロック

            // プレイヤーの現在位置にエフェクトを生成
            GameObject effect = Instantiate(healEffectPrefab, playerTransform.position, Quaternion.identity);

            // プレイヤーの子要素に設定して、移動に追従させる
            effect.transform.SetParent(playerTransform);

            // 大きさを固定
            effect.transform.localScale = effectScale;

            // ★追加：エフェクト（および子オブジェクト）の描画順をプレイヤーより手前にする
            SpriteRenderer[] srs = effect.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
            {
                sr.sortingOrder += sortingOrderOffset;
            }
            SpriteRenderer mainSr = effect.GetComponent<SpriteRenderer>();
            if (mainSr != null)
            {
                mainSr.sortingOrder += sortingOrderOffset;
            }

            // 1秒後に自動で消去
            Destroy(effect, 1.0f);
        }
    }
}