using UnityEngine;

public class VampireSkill : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private GameObject healEffectPrefab;

    [Header("吸血パラメータ")]
    [SerializeField] private float healAmount = 1f;
    [SerializeField] private Vector3 effectScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private int sortingOrderOffset = 10;

    public bool isUnlocked = false;

    private PlayerStatus playerStatus;
    private float effectCooldownTimer = 0f;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        if (!isUnlocked) return;

        if (effectCooldownTimer > 0f)
        {
            effectCooldownTimer -= Time.deltaTime;
        }
    }

    // 各武器のヒット時に呼び出される吸血処理
    public void OnWeaponHit(float dealtDamage)
    {
        if (!isUnlocked) return;

        if (playerStatus == null)
        {
            playerStatus = GetComponent<PlayerStatus>();
            if (playerStatus == null) return;
        }

        playerStatus.Heal(healAmount);

        if (effectCooldownTimer <= 0f && healEffectPrefab != null)
        {
            effectCooldownTimer = 0.2f;

            GameObject effect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
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