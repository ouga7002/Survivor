using UnityEngine;
using UnityEngine.UI;

public class MissionArrowIndicator : MonoBehaviour
{
    private GameObject arrowUIPrefab;
    private GameObject spawnedArrow;
    private RectTransform arrowRectTransform;
    private Image arrowImage;

    private Transform targetTransform;
    private Camera mainCamera;
    private Canvas parentCanvas;

    [Header("画面端のマージン調整")]
    [SerializeField] private float edgeBuffer = 0.08f;

    // 外部からプレハブとターゲットを受け取って初期化する
    public void Init(GameObject prefab, Transform target)
    {
        arrowUIPrefab = prefab;
        targetTransform = target;

        mainCamera = Camera.main;
        parentCanvas = FindFirstObjectByType<Canvas>();

        if (arrowUIPrefab != null && parentCanvas != null)
        {
            spawnedArrow = Instantiate(arrowUIPrefab, parentCanvas.transform);
            arrowRectTransform = spawnedArrow.GetComponent<RectTransform>();
            arrowImage = spawnedArrow.GetComponent<Image>();
            spawnedArrow.SetActive(false); // 初期状態は非表示
        }
    }

    void LateUpdate()
    {
        // ターゲットやカメラが無効なら非表示にして終了
        if (targetTransform == null || spawnedArrow == null || mainCamera == null)
        {
            if (spawnedArrow != null && spawnedArrow.activeSelf)
                spawnedArrow.SetActive(false);
            return;
        }

        // ターゲットのビューポート座標（画面内は 0.0 ～ 1.0）
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetTransform.position);

        // 2DゲームでZ座標がカメラの前方にあることを考慮し、画面内判定を厳密に行う
        // カメラの視界内（X: 0〜1, Y: 0〜1, Z > 0）にいるかどうか
        bool isInScreen = viewportPos.x >= 0f && viewportPos.x <= 1f &&
                          viewportPos.y >= 0f && viewportPos.y <= 1f &&
                          viewportPos.z > 0f;

        if (!isInScreen)
        {
            // --- 画面外にいるとき：矢印を表示して端に張り付ける ---
            if (!spawnedArrow.activeSelf) spawnedArrow.SetActive(true);

            // カメラの後ろにいる場合の反転処理
            if (viewportPos.z < 0f)
            {
                viewportPos *= -1f;
            }

            Vector2 center = new Vector2(0.5f, 0.5f);
            Vector2 dir = ((Vector2)viewportPos - center).normalized;

            float minX = edgeBuffer;
            float maxX = 1f - edgeBuffer;
            float minY = edgeBuffer;
            float maxY = 1f - edgeBuffer;

            float slope = Mathf.Abs(dir.y) / Mathf.Max(Mathf.Abs(dir.x), 0.0001f);
            Vector3 targetVP = viewportPos;

            if (targetVP.z > 0)
            {
                if (dir.x > 0)
                {
                    targetVP.x = maxX;
                    targetVP.y = center.y + (maxX - center.x) * slope * Mathf.Sign(dir.y);
                }
                else
                {
                    targetVP.x = minX;
                    targetVP.y = center.y + (minX - center.x) * slope * Mathf.Sign(dir.y);
                }

                if (targetVP.y > maxY)
                {
                    targetVP.y = maxY;
                    targetVP.x = center.x + (maxY - center.y) / slope * Mathf.Sign(dir.x);
                }
                else if (targetVP.y < minY)
                {
                    targetVP.y = minY;
                    targetVP.x = center.x + (minY - center.y) / slope * Mathf.Sign(dir.x);
                }
            }
            else
            {
                targetVP.x = dir.x > 0 ? minX : maxX;
                targetVP.y = dir.y > 0 ? minY : maxY;
            }

            targetVP.x = Mathf.Clamp(targetVP.x, minX, maxX);
            targetVP.y = Mathf.Clamp(targetVP.y, minY, maxY);

            Vector2 screenPos = mainCamera.ViewportToScreenPoint(targetVP);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                screenPos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );

            arrowRectTransform.anchoredPosition = Vector2.Lerp(arrowRectTransform.anchoredPosition, localPoint, Time.deltaTime * 20f);

            // 矢印の向きをターゲットに向ける
            Vector3 direction = (targetTransform.position - mainCamera.transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            // --- 画面内にいるとき：確実に矢印を消す ---
            if (spawnedArrow.activeSelf)
            {
                spawnedArrow.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
        if (spawnedArrow != null)
        {
            Destroy(spawnedArrow);
        }
    }
}