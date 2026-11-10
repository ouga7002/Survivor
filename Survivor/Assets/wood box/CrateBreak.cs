using System.Collections;
using UnityEngine;

public class CrateBreak : MonoBehaviour
{
    private Animator animator;
    private Collider2D crateCollider;
    private bool isBroken = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        crateCollider = GetComponent<Collider2D>();
    }

    // 手裏剣が木箱に当たったとき
    public void Hit()
    {
        if (isBroken) return;

        if (animator == null)
        {
            Debug.LogError("木箱のAnimatorが見つかりません！");
            return;
        }

        isBroken = true;

        // 木箱の当たり判定を無効化
        if (crateCollider != null)
        {
            crateCollider.enabled = false;
        }

        // 破壊アニメーションを再生
        animator.SetTrigger("Break");

        // アニメーション終了後に削除
        StartCoroutine(DestroyAfterBreak());
    }

    // 破壊アニメーション終了を待つ
    private IEnumerator DestroyAfterBreak()
    {
        // Crate_Break ステートに切り替わるまで待つ
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Crate_Break"))
        {
            yield return null;
        }

        // アニメーションが最後まで再生されるのを待つ
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // 木箱を削除
        Destroy(gameObject);

        Debug.Log("[木箱] 破壊完了！木箱を削除しました。");
    }
}