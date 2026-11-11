using UnityEngine;

public class VampireSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("回復時にプレイヤーの頭上などに表示するエフェクトのプレハブ")]
    [SerializeField] private GameObject healEffectPrefab;

    [Header("吸血パラメータ")]
    [SerializeField] private float healAmount = 1f;
    [SerializeField] private Vector3 effectScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private int sortingOrderOffset = 10;

    private Transform playerTransform;
    private PlayerStatus playerStatus;
    private float effectCooldownTimer = 0f;

    void OnEnable()
    {
        FindPlayerAndStatus();
        effectCooldownTimer = 0f;
    }

    void Start()
    {
        FindPlayerAndStatus();
    }

    void Update()
    {
        // プレイヤーが見つからない場合は毎フレーム探し直す
        if (playerTransform == null || playerStatus == null)
        {
            FindPlayerAndStatus();
        }

        // エフェクト生成用のクールダウンタイマーを減らす
        if (effectCooldownTimer > 0f)
        {
            effectCooldownTimer -= Time.deltaTime;
        }
    }

    void FindPlayerAndStatus()
    {
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

    // 各武器のヒット時に呼び出される吸血処理
    public void OnWeaponHit(float dealtDamage)
    {
        if (playerStatus == null)
        {
            FindPlayerAndStatus();
            if (playerStatus == null) return;
        }

        // 1. HP回復は常に行う
        playerStatus.Heal(healAmount);

        // 2. クールダウン中でなければエフェクトを生成する（連続発動でのバグを防ぐ）
        if (effectCooldownTimer <= 0f && healEffectPrefab != null && playerTransform != null)
        {
            effectCooldownTimer = 0.2f; // 0.2秒間はエフェクトの連続生成をガード

            GameObject effect = Instantiate(healEffectPrefab, playerTransform.position, Quaternion.identity);
            effect.transform.SetParent(playerTransform);
            effect.transform.localScale = effectScale;

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

            Destroy(effect, 1.0f);
        }
    }
}