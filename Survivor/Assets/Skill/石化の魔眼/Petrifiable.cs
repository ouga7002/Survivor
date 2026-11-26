
using System.Collections;
using UnityEngine;

public class Petrifiable : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;

    private RigidbodyConstraints2D originalConstraints;
    private Color[] originalColors;

    private bool isPetrified = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 子オブジェクトも含めてSpriteRendererを取得
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 元の物理設定を保存
        if (rb != null)
        {
            originalConstraints = rb.constraints;
        }

        // 元の色を保存
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    // 石化させる
    public void Petrify(float duration)
    {
        // 石化中なら再度発動しない
        if (isPetrified)
        {
            return;
        }

        StartCoroutine(PetrifyCoroutine(duration));
    }

    private IEnumerator PetrifyCoroutine(float duration)
    {
        isPetrified = true;

        // Rigidbody2Dを停止
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // スプライトを灰色にする
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = Color.gray;
            }
        }

        // 指定時間待機
        yield return new WaitForSeconds(duration);

        // Rigidbody2Dを元に戻す
        if (rb != null)
        {
            rb.constraints = originalConstraints;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // スプライトの色を元に戻す
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = originalColors[i];
            }
        }

        isPetrified = false;
    }

    public bool IsPetrified()
    {
        return isPetrified;
    }
}
