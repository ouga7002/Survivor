using UnityEngine;
using System.Collections;

public class GateSkill : MonoBehaviour
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
    [Tooltip("Playerからゲートまでの距離")]
    [SerializeField] private float gateBackDistance = 1.2f;

    [Tooltip("ゲート同士の横方向の間隔")]
    [SerializeField] private float gateSpread = 1.0f;

    [Tooltip("ゲート全体を下にずらす量")]
    [SerializeField] private float gateVerticalOffset = 0.2f;

    [Tooltip("ゲートごとの前後位置のずれ")]
    [SerializeField]
    private float[] gateDepthOffsets =
    {
        -0.3f,
         0.2f,
        -0.1f,
         0.35f,
         0.0f
    };

    [Header("ゲート設定")]
    [SerializeField] private int gateCount = 5;

    [Header("敵自動検知設定")]
    [Tooltip("Playerを中心に敵を探す範囲")]
    [SerializeField] private float targetSearchRadius = 10f;

    [Header("スキル状態")]
    [SerializeField] private bool isUnlocked = false;

    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    private GameObject[] gates;

    // 前回射出したゲート番号
    private int previousGateIndex = -1;

    public bool IsUnlocked => isUnlocked;

    void Start()
    {
        // AimCursorを自動取得
        if (aimCursor == null)
        {
            GameObject cursorObj = GameObject.Find("AimCursor");

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

        // 最初はすぐ発動できる状態
        cooldownTimer = cooldown;
    }

    void Update()
    {
        if (!isUnlocked) return;
        if (aimCursor == null) return;

        // スキル発動中
        if (isAttacking)
        {
            UpdateGatePositions();
            return;
        }

        // クールタイム計測
        cooldownTimer += Time.deltaTime;

        // クールタイム終了
        if (cooldownTimer >= cooldown)
        {
            cooldownTimer = 0f;
            StartCoroutine(GateAttackRoutine());
        }
    }

    // =========================================
    // スキル発動処理
    // =========================================
    IEnumerator GateAttackRoutine()
    {
        isAttacking = true;

        previousGateIndex = -1;

        // ゲート配列を作成
        gates = new GameObject[gateCount];

        // ゲートを生成
        for (int i = 0; i < gateCount; i++)
        {
            gates[i] = Instantiate(gatePrefab);
        }

        // ゲートの位置を設定
        UpdateGatePositions();

        float elapsedTime = 0f;

        // 10秒間射出
        while (elapsedTime < activeDuration)
        {
            if (aimCursor == null)
            {
                break;
            }

            // ゲート位置を常に更新
            UpdateGatePositions();

            // ランダムなゲートを選択
            int randomGateIndex = GetRandomGateIndex();

            // 選択したゲートから剣を射出
            ShootSword(gates[randomGateIndex]);

            // 0.3秒待機
            yield return new WaitForSeconds(shotInterval);

            elapsedTime += shotInterval;
        }

        // 全ゲートを消す
        DestroyAllGates();

        // スキル終了
        isAttacking = false;

        // クールタイムをリセット
        cooldownTimer = 0f;
    }

    // =========================================
    // ランダムなゲート選択
    // =========================================
    int GetRandomGateIndex()
    {
        if (gateCount <= 1)
        {
            return 0;
        }

        int randomIndex;

        // 前回と同じゲートを避ける
        do
        {
            randomIndex = Random.Range(0, gateCount);
        }
        while (randomIndex == previousGateIndex);

        previousGateIndex = randomIndex;

        return randomIndex;
    }

    // =========================================
    // ゲートの位置更新
    // =========================================
    void UpdateGatePositions()
    {
        if (aimCursor == null) return;
        if (gates == null) return;

        // Playerからカーソル方向
        Vector2 aimDirection =
            ((Vector2)aimCursor.position -
            (Vector2)transform.position).normalized;

        // 照準の反対方向
        Vector2 backwardDirection = -aimDirection;

        // 照準方向に対して垂直
        Vector2 perpendicularDirection =
            new Vector2(-aimDirection.y, aimDirection.x);

        // ゲートの基準位置
        Vector2 basePosition =
            (Vector2)transform.position
            + backwardDirection * gateBackDistance
            + Vector2.down * gateVerticalOffset;

        // 各ゲートの位置を設定
        for (int i = 0; i < gates.Length; i++)
        {
            if (gates[i] == null) continue;

            // 横方向の位置
            float spreadOffset =
                (i - (gateCount - 1) / 2f) * gateSpread;

            // 前後方向の位置ずれ
            float depthOffset = 0f;

            if (gateDepthOffsets != null &&
                i < gateDepthOffsets.Length)
            {
                depthOffset = gateDepthOffsets[i];
            }

            // ゲート位置
            Vector2 gatePosition =
                basePosition
                + perpendicularDirection * spreadOffset
                + backwardDirection * depthOffset;

            gates[i].transform.position = gatePosition;
        }
    }

    // =========================================
    // 最も近い敵を検索
    // =========================================
    Transform FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            targetSearchRadius
        );

        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            if (col == null) continue;

            // EnemyHealthを取得
            EnemyHealth enemyHealth =
                col.GetComponent<EnemyHealth>();

            // Colliderが子にある場合
            if (enemyHealth == null)
            {
                enemyHealth =
                    col.GetComponentInParent<EnemyHealth>();
            }

            // EnemyHealthがなければ対象外
            if (enemyHealth == null)
            {
                continue;
            }

            Transform enemyTransform =
                enemyHealth.transform;

            // Playerとの距離
            float distance = Vector2.Distance(
                transform.position,
                enemyTransform.position
            );

            // 最も近い敵を選択
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemyTransform;
            }
        }

        return nearestEnemy;
    }

    // =========================================
    // 剣の射出
    // =========================================
    void ShootSword(GameObject gate)
    {
        if (gate == null) return;
        if (swordPrefab == null) return;

        // 最も近い敵を検索
        Transform target = FindNearestEnemy();

        Vector2 direction;

        // 敵が存在する場合
        if (target != null)
        {
            direction = (
                (Vector2)target.position -
                (Vector2)gate.transform.position
            ).normalized;
        }
        // 敵がいない場合はカーソル方向
        else if (aimCursor != null)
        {
            direction = (
                (Vector2)aimCursor.position -
                (Vector2)transform.position
            ).normalized;
        }
        // どちらも取得できない場合
        else
        {
            direction = Vector2.right;
        }

        // 剣を生成
        GameObject swordObj = Instantiate(
            swordPrefab,
            gate.transform.position,
            Quaternion.identity
        );

        // GateSwordProjectileを取得
        GateSwordProjectile swordScript =
            swordObj.GetComponent<GateSwordProjectile>();

        if (swordScript != null)
        {
            swordScript.Initialize(
                direction,
                swordDamage,
                target
            );
        }
        else
        {
            Debug.LogError(
                "[GateSkill] 剣PrefabにGateSwordProjectileがありません。"
            );
        }
    }

    // =========================================
    // ゲートの消去
    // =========================================
    void DestroyAllGates()
    {
        if (gates == null) return;

        for (int i = 0; i < gates.Length; i++)
        {
            if (gates[i] != null)
            {
                Destroy(gates[i]);
            }
        }

        gates = null;
    }

    // =========================================
    // スキル解放
    // =========================================
    public void UnlockSkill()
    {
        isUnlocked = true;
        cooldownTimer = cooldown;
    }

    // =========================================
    // スキルロック
    // =========================================
    public void LockSkill()
    {
        isUnlocked = false;

        // 発動中ならゲートを消す
        if (isAttacking)
        {
            StopAllCoroutines();

            DestroyAllGates();

            isAttacking = false;
        }
    }

    // =========================================
    // 検知範囲の確認（Sceneビュー）
    // =========================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            targetSearchRadius
        );
    }
}