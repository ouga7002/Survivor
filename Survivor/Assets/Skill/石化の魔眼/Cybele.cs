
using UnityEngine;
using System.Collections;

public class Cybele : MonoBehaviour, ISkillUnlockable
{
    [Header("参照設定")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform aimCursor;

    [Header("スキル設定")]
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float petrifyDuration = 3f;
    [SerializeField] private float coolDownDuration = 8f;

    [Header("魔眼エフェクト設定")]
    [SerializeField] private Sprite[] effectFrames;
    [SerializeField] private float effectFramesPerSecond = 12f;
    [SerializeField] private float effectDistance = 1.5f;
    [SerializeField] private float effectScale = 2f;
    [SerializeField] private int effectSortingOrder = 10;

    // スキルガチャ用
    private bool isUnlocked = false;

    public string SkillId => "cybele";

    private float coolDownTimer = 0f;

    private void Start()
    {
        // Playerを自動取得
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // AimCursorを自動取得
        if (aimCursor == null)
        {
            GameObject cursorObject = GameObject.Find("AimCursor");

            if (cursorObject != null)
            {
                aimCursor = cursorObject.transform;
            }
        }
    }

    private void Update()
    {
        if (!isUnlocked)
        {
            return;
        }

        if (player == null || aimCursor == null)
        {
            return;
        }

        // クールタイム中
        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;
            return;
        }

        // 石化の魔眼を発動
        ActivateCybele();

        // クールタイム開始
        coolDownTimer = coolDownDuration;
    }

    private void ActivateCybele()
    {
        Vector2 playerPosition = player.position;

        // カーソル方向を取得
        Vector2 aimDirection =
            ((Vector2)aimCursor.position - playerPosition).normalized;

        // 魔眼エフェクトを再生
        PlayEffect(aimDirection);

        // 敵を検索
        Petrifiable[] enemies = FindObjectsOfType<Petrifiable>();

        foreach (Petrifiable enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            Vector2 enemyPosition = enemy.transform.position;
            Vector2 toEnemy = enemyPosition - playerPosition;

            // 距離判定
            if (toEnemy.magnitude > searchRadius)
            {
                continue;
            }

            // 前方180度の角度判定
            float angle = Vector2.Angle(aimDirection, toEnemy);

            if (angle > 90f)
            {
                continue;
            }

            // 石化中の敵は無視
            if (enemy.IsPetrified())
            {
                continue;
            }

            // 敵を石化
            enemy.Petrify(petrifyDuration);
        }
    }

    // 魔眼エフェクトを生成
    private void PlayEffect(Vector2 direction)
    {
        if (effectFrames == null || effectFrames.Length == 0)
        {
            return;
        }

        // エフェクト生成位置
        Vector2 spawnPosition =
            (Vector2)player.position + direction * effectDistance;

        // エフェクト用オブジェクトを作成
        GameObject effectObject = new GameObject("CybeleEffect");

        effectObject.transform.position = spawnPosition;
        effectObject.transform.localScale =
            Vector3.one * effectScale;

        // SpriteRendererを追加
        SpriteRenderer sr =
            effectObject.AddComponent<SpriteRenderer>();

        sr.sprite = effectFrames[0];
        sr.sortingOrder = effectSortingOrder;

        // アニメーション開始
        StartCoroutine(PlayEffectAnimation(effectObject, sr));
    }

    // スプライトシートのアニメーション再生
    private IEnumerator PlayEffectAnimation(
        GameObject effectObject,
        SpriteRenderer sr)
    {
        float frameInterval = 1f / Mathf.Max(1f, effectFramesPerSecond);

        for (int i = 0; i < effectFrames.Length; i++)
        {
            if (effectObject == null || sr == null)
            {
                yield break;
            }

            sr.sprite = effectFrames[i];

            yield return new WaitForSeconds(frameInterval);
        }

        // 再生終了後に削除
        if (effectObject != null)
        {
            Destroy(effectObject);
        }
    }

    // スキルガチャから呼び出される
    public void SetSkillUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        {
            coolDownTimer = 0f;
        }
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
