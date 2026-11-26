
using UnityEngine;
using System.Collections;

public class GateSkill : MonoBehaviour, ISkillUnlockable
{
    [Header("参照設定")]
    [SerializeField] private Transform aimCursor;
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private GameObject swordPrefab;

    [Header("スキル設定")]
    [SerializeField] private float swordDamage = 10f;
    [SerializeField] private float cooldown = 6f;
    [SerializeField] private float shotInterval = 0.3f;
    [SerializeField] private float activeDuration = 10f;

    [Header("ゲート配置設定")]
    [Tooltip("ゲート1～5の位置を個別に調整")]
    [SerializeField]
    private Vector2[] gateOffsets =
    {
        new Vector2(-1.6f, 1.0f),
        new Vector2(-0.9f, 1.5f),
        new Vector2( 0.0f, 1.7f),
        new Vector2( 0.9f, 1.5f),
        new Vector2( 1.6f, 1.0f)
    };

    [Header("ゲート設定")]
    [SerializeField] private int gateCount = 5;

    [Header("敵自動検知設定")]
    [SerializeField] private float targetSearchRadius = 10f;

    [Header("スキル解放設定")]
    [SerializeField] private bool isUnlocked = false;

    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    private GameObject[] gates;
    private int previousGateIndex = -1;

    // =====================================
    // 共通スキル管理
    // =====================================

    public string SkillId => "gate";

    public void SetSkillUnlocked(bool unlocked)
    {
        if (unlocked)
        {
            UnlockSkill();
        }
        else
        {
            LockSkill();
        }
    }

    public bool IsUnlocked => isUnlocked;

    // =====================================
    // 初期設定
    // =====================================

    void Start()
    {
        if (aimCursor == null)
        {
            GameObject cursorObj =
                GameObject.Find("AimCursor");

            if (cursorObj != null)
            {
                aimCursor = cursorObj.transform;
            }
            else
            {
                Debug.LogError(
                    "[GateSkill] AimCursorが見つかりません。"
                );
            }
        }

        cooldownTimer = cooldown;
    }

    // =====================================
    // 更新処理
    // =====================================

    void Update()
    {
        if (!isUnlocked) return;

        if (isAttacking)
        {
            UpdateGatePositions();
            return;
        }

        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= cooldown)
        {
            cooldownTimer = 0f;
            StartCoroutine(GateAttackRoutine());
        }
    }

    // =====================================
    // スキル発動
    // =====================================

    IEnumerator GateAttackRoutine()
    {
        isAttacking = true;
        previousGateIndex = -1;

        gates = new GameObject[gateCount];

        for (int i = 0; i < gateCount; i++)
        {
            gates[i] = Instantiate(gatePrefab);

            SetGateSortingOrder(gates[i]);
        }

        UpdateGatePositions();

        float elapsedTime = 0f;

        while (elapsedTime < activeDuration)
        {
            UpdateGatePositions();

            int randomIndex = GetRandomGateIndex();

            ShootSword(gates[randomIndex]);

            yield return new WaitForSeconds(shotInterval);

            elapsedTime += shotInterval;
        }

        DestroyAllGates();

        isAttacking = false;
        cooldownTimer = 0f;
    }

    // =====================================
    // ゲートの描画順設定
    // =====================================

    void SetGateSortingOrder(GameObject gate)
    {
        if (gate == null) return;

        SpriteRenderer playerRenderer =
            GetComponent<SpriteRenderer>();

        if (playerRenderer == null) return;

        SpriteRenderer[] gateRenderers =
            gate.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in gateRenderers)
        {
            renderer.sortingLayerID =
                playerRenderer.sortingLayerID;

            renderer.sortingOrder =
                playerRenderer.sortingOrder - 1;
        }
    }

    // =====================================
    // ゲート位置更新
    // =====================================

    void UpdateGatePositions()
    {
        if (gates == null) return;

        for (int i = 0; i < gates.Length; i++)
        {
            if (gates[i] == null) continue;

            if (gateOffsets == null ||
                i >= gateOffsets.Length)
                continue;

            Vector2 gatePosition =
                (Vector2)transform.position + gateOffsets[i];

            gates[i].transform.position = gatePosition;
        }
    }

    // =====================================
    // ランダムなゲート選択
    // =====================================

    int GetRandomGateIndex()
    {
        if (gates == null || gates.Length <= 1)
            return 0;

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, gates.Length);
        }
        while (randomIndex == previousGateIndex);

        previousGateIndex = randomIndex;

        return randomIndex;
    }

    // =====================================
    // 最も近い敵を検索
    // =====================================

    Transform FindNearestEnemy()
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                targetSearchRadius
            );

        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            if (col == null) continue;

            EnemyHealth enemyHealth =
                col.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth =
                    col.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth == null) continue;

            float distance = Vector2.Distance(
                transform.position,
                enemyHealth.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemyHealth.transform;
            }
        }

        return nearestEnemy;
    }

    // =====================================
    // 剣の射出
    // =====================================

    void ShootSword(GameObject gate)
    {
        if (gate == null) return;
        if (swordPrefab == null) return;

        Transform target = FindNearestEnemy();

        Vector2 direction;

        if (target != null)
        {
            direction = (
                (Vector2)target.position -
                (Vector2)gate.transform.position
            ).normalized;
        }
        else if (aimCursor != null)
        {
            direction = (
                (Vector2)aimCursor.position -
                (Vector2)transform.position
            ).normalized;
        }
        else
        {
            direction = Vector2.right;
        }

        GameObject swordObj = Instantiate(
            swordPrefab,
            gate.transform.position,
            Quaternion.identity
        );

        GateSwordProjectile swordScript =
            swordObj.GetComponent<GateSwordProjectile>();

        if (swordScript != null)
        {
            swordScript.Initialize(
                direction,
                swordDamage
            );
        }
        else
        {
            Debug.LogError(
                "[GateSkill] 剣PrefabにGateSwordProjectileがありません。"
            );

            Destroy(swordObj);
        }
    }

    // =====================================
    // ゲート消去
    // =====================================

    void DestroyAllGates()
    {
        if (gates == null) return;

        foreach (GameObject gate in gates)
        {
            if (gate != null)
            {
                Destroy(gate);
            }
        }

        gates = null;
    }

    // =====================================
    // スキル解放
    // =====================================

    public void UnlockSkill()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        cooldownTimer = cooldown;
    }

    // =====================================
    // スキルロック
    // =====================================

    public void LockSkill()
    {
        isUnlocked = false;

        if (isAttacking)
        {
            StopAllCoroutines();

            DestroyAllGates();

            isAttacking = false;
        }
    }

    // =====================================
    // 検知範囲の表示
    // =====================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            targetSearchRadius
        );
    }
}
