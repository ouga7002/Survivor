
using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Rigidbody2D rb;

    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;

    private Petrifiable petrifiable;

    private void Awake()
    {
        // Rigidbody2Dを取得
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // 石化コンポーネントを取得
        petrifiable = GetComponent<Petrifiable>();
    }

    // ノックバック中かどうかを取得
    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }

    // 発勁などから呼び出すノックバック処理
    public void ApplyKnockback(
        Vector2 direction,
        float force,
        float duration)
    {
        if (rb == null)
            return;

        // 石化中はノックバックを受けない
        if (petrifiable != null &&
            petrifiable.IsPetrified())
        {
            return;
        }

        // すでにノックバック中なら前の処理を終了
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        knockbackCoroutine = StartCoroutine(
            KnockbackCoroutine(direction, force, duration)
        );
    }

    private IEnumerator KnockbackCoroutine(
        Vector2 direction,
        float force,
        float duration)
    {
        isKnockedBack = true;

        // 吹き飛ばす方向を正規化
        direction = direction.normalized;

        // 既存の速度をリセット
        rb.linearVelocity = Vector2.zero;

        // ノックバック速度を設定
        rb.linearVelocity = direction * force;

        // 指定時間だけ吹き飛ばす
        yield return new WaitForSeconds(duration);

        // ノックバック終了
        rb.linearVelocity = Vector2.zero;

        isKnockedBack = false;
        knockbackCoroutine = null;
    }
}
