
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
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        petrifiable = GetComponent<Petrifiable>();
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }

    public void ApplyKnockback(
        Vector2 direction,
        float force,
        float duration)
    {
        // 非アクティブな敵には処理しない
        if (!gameObject.activeInHierarchy ||
            !isActiveAndEnabled)
        {
            return;
        }

        if (rb == null)
            return;

        // 石化中はノックバックしない
        if (petrifiable != null &&
            petrifiable.IsPetrified())
        {
            return;
        }

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
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

        direction = direction.normalized;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direction * force;

        yield return new WaitForSeconds(duration);

        // オブジェクトが非アクティブになっていないか確認
        if (gameObject.activeInHierarchy && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        isKnockedBack = false;
        knockbackCoroutine = null;
    }

    private void OnDisable()
    {
        // 無効化された場合はコルーチンを停止
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }

        isKnockedBack = false;
    }
}
