using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class R_Boss1Controller : MonoBehaviour, IEnemyAttack
{
    private Transform player;
    private EnemyAttackManager attackManager;

    [Header("弾のPrefab")]
    public GameObject homingBullet;
    public GameObject straightBullet;

    [Header("弾の生成位置")]
    public Transform bulletSpawnPoint;
    public float spawnHeight = 1.0f;

    [Header("攻撃設定")]
    public float attackInterval = 5f;
    public float firstAttackDelay = 2f;
    public float attackWarningDuration = 1.5f;
    public float attackBetweenDelay = 0.5f;

    [Header("32方向拡散弾の設定")]
    public int spreadBulletCount = 32;
    public float spreadAngle = 360f;
    public float spreadBulletSpawnDistance = 1.0f;

    [Header("岩の棘")]
    public GameObject rockSpikePrefab;
    public int rockSpikeCount = 3;
    public float rockSpikeInterval = 0.6f;

    [Header("攻撃予告設定")]
    public float flashInterval = 0.12f;

    [Header("ボス点滅用")]
    public SpriteRenderer warningSpriteRenderer;

    [Header("攻撃直前の目の発光")]
    public SpriteRenderer eyeGlowSpriteRenderer;
    public float eyeGlowDuration = 0.1f;

    private Coroutine attackCoroutine;
    private bool isAttacking = true;
    private bool isDead;

    private enum AttackType
    {
        Homing,
        Spread32,
        RockSpike,
        RockPrison,
        SplitBullet
    }

    void Start()
    {
        attackManager = GetComponent<EnemyAttackManager>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (warningSpriteRenderer != null)
        {
            if (spriteRenderer != null)
                warningSpriteRenderer.sprite = spriteRenderer.sprite;

            warningSpriteRenderer.enabled = false;
            warningSpriteRenderer.color =
                new Color(1f, 0.5f, 0f, 0.7f);
        }

        if (eyeGlowSpriteRenderer != null)
            eyeGlowSpriteRenderer.enabled = false;

        attackCoroutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(firstAttackDelay);

        while (isAttacking && !isDead)
        {
            if (player == null)
                yield break;

            // 5種類の攻撃から選択
            List<AttackType> attacks = new List<AttackType>
            {
                AttackType.Homing,
                AttackType.Spread32,
                AttackType.RockSpike,
                AttackType.RockPrison,
                AttackType.SplitBullet
            };

            // 1つ目をランダム選択
            AttackType attack1 =
                attacks[Random.Range(0, attacks.Count)];

            attacks.Remove(attack1);

            // 残り4種類から2つ目を選択
            AttackType attack2 =
                attacks[Random.Range(0, attacks.Count)];

            // 攻撃予告
            yield return StartCoroutine(ShowAttackWarning());

            if (!isAttacking || isDead)
                yield break;

            // 1つ目の攻撃
            yield return StartCoroutine(ExecuteAttack(attack1));

            if (!isAttacking || isDead)
                yield break;

            yield return new WaitForSeconds(attackBetweenDelay);

            // 2つ目の攻撃
            yield return StartCoroutine(ExecuteAttack(attack2));

            // 次の攻撃周期まで待機
            yield return new WaitForSeconds(attackInterval);
        }
    }

    // 攻撃予告
    IEnumerator ShowAttackWarning()
    {
        float elapsed = 0f;

        float glowDuration =
            Mathf.Clamp(eyeGlowDuration, 0f, attackWarningDuration);

        float warningDuration =
            attackWarningDuration - glowDuration;

        // 通常の点滅予告
        while (elapsed < warningDuration && isAttacking)
        {
            if (warningSpriteRenderer != null)
                warningSpriteRenderer.enabled = true;

            float waitTime = Mathf.Min(
                flashInterval,
                warningDuration - elapsed);

            yield return new WaitForSeconds(waitTime);

            if (warningSpriteRenderer != null)
                warningSpriteRenderer.enabled = false;

            yield return new WaitForSeconds(waitTime);

            elapsed += waitTime * 2f;
        }

        // 攻撃直前の発光
        if (eyeGlowSpriteRenderer != null)
            eyeGlowSpriteRenderer.enabled = true;

        yield return new WaitForSeconds(glowDuration);

        HideAttackWarning();
    }

    void HideAttackWarning()
    {
        if (warningSpriteRenderer != null)
            warningSpriteRenderer.enabled = false;

        if (eyeGlowSpriteRenderer != null)
            eyeGlowSpriteRenderer.enabled = false;
    }

    // 選択した攻撃を実行
    IEnumerator ExecuteAttack(AttackType attack)
    {
        switch (attack)
        {
            case AttackType.Homing:
                yield return StartCoroutine(AttackHoming());
                break;

            case AttackType.Spread32:
                yield return StartCoroutine(AttackSpread32());
                break;

            case AttackType.RockSpike:
                yield return StartCoroutine(AttackRockSpike());
                break;

            case AttackType.RockPrison:
                yield return StartCoroutine(AttackRockPrison());
                break;

            case AttackType.SplitBullet:
                yield return StartCoroutine(AttackSplitBullet());
                break;
        }
    }

    // 追尾弾2発
    IEnumerator AttackHoming()
    {
        if (homingBullet == null)
            yield break;

        Vector3 basePosition = GetSpawnPosition();

        // 左右に2発生成
        GameObject bullet1 = Instantiate(
            homingBullet,
            basePosition + Vector3.left * 0.5f,
            Quaternion.identity
        );

        GameObject bullet2 = Instantiate(
            homingBullet,
            basePosition + Vector3.right * 0.5f,
            Quaternion.identity
        );

        if (attackManager != null)
        {
            attackManager.RegisterBullet(bullet1);
            attackManager.RegisterBullet(bullet2);
        }

        yield return null;
    }

    // 32方向拡散弾
    IEnumerator AttackSpread32()
    {
        if (straightBullet == null || player == null)
            yield break;

        Vector2 baseDirection =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        float baseAngle =
            Mathf.Atan2(baseDirection.y, baseDirection.x)
            * Mathf.Rad2Deg;

        Vector2 spawnPosition = GetSpawnPosition();

        float angleStep = spreadAngle / spreadBulletCount;

        for (int i = 0; i < spreadBulletCount; i++)
        {
            float angle =
                baseAngle - spreadAngle / 2f + angleStep * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 createPosition =
                spawnPosition + direction * spreadBulletSpawnDistance;

            GameObject bullet = Instantiate(
                straightBullet,
                createPosition,
                Quaternion.identity
            );

            B1_Bullet1 bulletScript =
                bullet.GetComponent<B1_Bullet1>();

            if (bulletScript != null)
                bulletScript.Initialize(direction);

            if (attackManager != null)
                attackManager.RegisterBullet(bullet);
        }

        yield return null;
    }

    // プレイヤー足元の岩の棘3連
    IEnumerator AttackRockSpike()
    {
        if (rockSpikePrefab == null || player == null)
            yield break;

        for (int i = 0; i < rockSpikeCount; i++)
        {
            if (player == null || isDead)
                yield break;

            // 出現時点でのプレイヤー位置を狙う
            Vector3 spawnPosition = player.position;

            GameObject spike = Instantiate(
                rockSpikePrefab,
                spawnPosition,
                Quaternion.identity
            );

            if (attackManager != null)
                attackManager.RegisterBullet(spike);

            // 次の棘まで待機
            if (i < rockSpikeCount - 1)
                yield return new WaitForSeconds(rockSpikeInterval);
        }
    }

    // 岩の牢獄
    IEnumerator AttackRockPrison()
    {
        Debug.Log("岩の牢獄");

        // 後で実装

        yield return null;
    }

    // 4分裂炸裂弾
    IEnumerator AttackSplitBullet()
    {
        Debug.Log("4分裂炸裂弾");

        // 後で実装

        yield return null;
    }

    // 弾の生成位置取得
    Vector3 GetSpawnPosition()
    {
        if (bulletSpawnPoint != null)
            return bulletSpawnPoint.position;

        Vector3 position = transform.position;
        position.y += spawnHeight;

        return position;
    }

    // 攻撃停止
    public void StopAttack()
    {
        isDead = true;
        isAttacking = false;

        HideAttackWarning();

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }
}
