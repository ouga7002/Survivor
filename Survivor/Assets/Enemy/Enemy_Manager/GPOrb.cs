using UnityEngine;

public enum OrbEffectType
{
    GP,
    Heal,
    AttackUp
}

public class GPOrb : MonoBehaviour
{
    [Header("オーブの効果")]
    public OrbEffectType effectType;

    public float effectAmount = 5f;

    [Tooltip("攻撃力アップの持続時間")]
    public float duration = 10f;

    [Header("吸引設定")]
    public float attractionRange = 5f;
    public float moveSpeed = 8f;
    public float pickupRange = 0.3f;

    private Transform player;
    private PlayerStatus playerStatus;
    private GPOrbPool pool;

    private bool isAttracted;

    public void Initialize(GPOrbPool orbPool)
    {
        pool = orbPool;
    }

    private GameObject originalPrefab;

    public GameObject OriginalPrefab => originalPrefab;

    public void SetOriginalPrefab(GameObject prefab)
    {
        originalPrefab = prefab;
    }

    public void Setup()
    {
        isAttracted = false;
        player = null;
        playerStatus = null;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null) return;

            player = playerObject.transform;
            playerStatus = playerObject.GetComponent<PlayerStatus>();
        }

        if (playerStatus == null) return;

        Vector2 difference =
            (Vector2)player.position - (Vector2)transform.position;

        float distanceSquared = difference.sqrMagnitude;

        if (!isAttracted)
        {
            if (distanceSquared > attractionRange * attractionRange)
                return;

            isAttracted = true;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        if (distanceSquared <= pickupRange * pickupRange)
        {
            ApplyEffect();
            pool.Return(this);
        }
    }

    void ApplyEffect()
    {
        switch (effectType)
        {
            case OrbEffectType.GP:
                playerStatus.AddGatyaPoint(
                    Mathf.RoundToInt(effectAmount)
                );
                break;

            case OrbEffectType.Heal:
                playerStatus.Heal(effectAmount);
                break;

            case OrbEffectType.AttackUp:
                playerStatus.ApplyAttackUp(
                    effectAmount,
                    duration
                );
                break;
        }
    }
}