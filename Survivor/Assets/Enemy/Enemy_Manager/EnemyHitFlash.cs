using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    [Header("被弾エフェクト設定")]
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("被弾時に色を変更しないSpriteRenderer")]
    public SpriteRenderer[] ignoreRenderers;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine flashCoroutine;

    void Awake()
    {
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
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null &&
                !IsIgnored(spriteRenderers[i]))
            {
                spriteRenderers[i].color = hitColor;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        RestoreColor();

        flashCoroutine = null;
    }

    bool IsIgnored(SpriteRenderer target)
    {
        if (ignoreRenderers == null) return false;

        foreach (SpriteRenderer ignored in ignoreRenderers)
        {
            if (ignored == target)
                return true;
        }

        return false;
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
