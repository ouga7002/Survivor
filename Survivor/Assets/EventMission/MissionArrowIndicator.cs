using UnityEngine;
using UnityEngine.UI;

public class MissionArrowIndicator : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject arrowUIPrefab; // 先ほど作った矢印のプレハブ
    private GameObject spawnedArrow;
    private RectTransform arrowRectTransform;
    private Image arrowImage;

    private Transform targetTransform;
    private Camera mainCamera;
    private Canvas parentCanvas;

    void Awake()
    {
        mainCamera = Camera.main;
        // シーン内のCanvasを自動検索
        parentCanvas = FindFirstObjectByType<Canvas>();

        if (arrowUIPrefab != null && parentCanvas != null)
        {
            spawnedArrow = Instantiate(arrowUIPrefab, parentCanvas.transform);
            arrowRectTransform = spawnedArrow.GetComponent<RectTransform>();
            arrowImage = spawnedArrow.GetComponent<Image>();
            spawnedArrow.SetActive(false); // 初期状態は非表示
        }
    }

    public void Setup(Transform target)
    {
        targetTransform = target;
    }

    void LateUpdate()
    {
        if (targetTransform == null || spawnedArrow == null || mainCamera == null)
        {
            if (spawnedArrow != null) spawnedArrow.SetActive(false);
            return;
        }

        // ターゲットのスクリーン座標を計算
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetTransform.position);

        // カメラの視界内（0〜1の範囲）にいるかどうか
        bool isOffScreen = viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f || viewportPos.z < 0f;

        if (isOffScreen)
        {
            if (!spawnedArrow.activeSelf) spawnedArrow.SetActive(true);

            // Z手前側に補正（カメラ後ろにいる場合の反転対策）
            if (viewportPos.z < 0f)
            {
                viewportPos *= -1f;
            }

            // ビューポート座標を画面の端にクランプ（制限）する
            Vector3 clampedViewportPos = new Vector3(
                Mathf.Clamp(viewportPos.x, 0.1f, 0.9f),
                Mathf.Clamp(viewportPos.y, 0.1f, 0.9f),
                0f
            );

            // スクリーン座標へ変換
            Vector2 screenPos = mainCamera.ViewportToScreenPoint(clampedViewportPos);

            // UIの座標系に反映（CanvasがOverlayの場合）
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                screenPos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
            arrowRectTransform.anchoredPosition = localPoint;

            // ターゲットの方向を向くように矢印を回転させる
            Vector3 direction = (targetTransform.position - mainCamera.transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            // 画面内にいるときは矢印を非表示にする
            if (spawnedArrow.activeSelf) spawnedArrow.SetActive(false);
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