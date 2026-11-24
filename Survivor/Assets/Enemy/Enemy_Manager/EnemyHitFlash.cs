using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    [Header("被弾エフェクト設定")]
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine flashCoroutine;

    void Awake()
    {
        // 子オブジェクトも含めてSpriteRendererを取得
        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(true);

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    void OnEnable()
    {
        // プールから再利用されたときに色を元に戻す
        RestoreColor();
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        // 赤くする
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = hitColor;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        // 元の色に戻す
        RestoreColor();

        flashCoroutine = null;
    }

    void RestoreColor()
    {
        if (spriteRenderers == null) return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = originalColors[i];
            }
        }

        flashCoroutine = null;
    }
}

