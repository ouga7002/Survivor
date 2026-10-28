using UnityEngine;

/// <summary>
/// 無限ループ型の2Dマップを管理するクラス（中心ズレ修正版）。
/// </summary>
public class InfiniteTileMap : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("マップ設定")]
    [Tooltip("タイルの基準サイズ（画像に合わせて自動調整されます）")]
    [SerializeField] private float tileSize = 20f;

    [Tooltip("プレイヤーの周囲に配置するタイルの半径（横方向）")]
    [SerializeField] private int tileRadiusX = 4;

    [Tooltip("プレイヤーの周囲に配置するタイルの半径（縦方向）")]
    [SerializeField] private int tileRadiusY = 3;

    [Header("背景素材の設定")]
    [Tooltip("使用したい背景のスプライト画像")]
    [SerializeField] private Sprite customSprite;

    [Tooltip("スプライトの色")]
    [SerializeField] private Color groundColor = Color.white;

    private GameObject[,] tiles;
    private Sprite defaultSquareSprite;
    private int gridWidth;
    private int gridHeight;

    private float tileWidth;
    private float tileHeight;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("InfiniteTileMapにPlayerを設定してください。");
            enabled = false;
            return;
        }

        defaultSquareSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 2, 2),
            new Vector2(0.5f, 0.5f),
            2f
        );

        Sprite spriteToUse = customSprite != null ? customSprite : defaultSquareSprite;

        if (spriteToUse != null)
        {
            Vector2 spriteSize = spriteToUse.rect.size / spriteToUse.pixelsPerUnit;
            if (spriteSize.x > 0 && spriteSize.y > 0)
            {
                float scale = tileSize / spriteSize.x;
                tileWidth = tileSize;
                tileHeight = spriteSize.y * scale;
            }
            else
            {
                tileWidth = tileSize;
                tileHeight = tileSize;
            }
        }
        else
        {
            tileWidth = tileSize;
            tileHeight = tileSize;
        }

        gridWidth = tileRadiusX * 2 + 1;
        gridHeight = tileRadiusY * 2 + 1;
        tiles = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject tile = new GameObject($"GroundTile_{x}_{y}");
                tile.transform.SetParent(transform);

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = spriteToUse;
                renderer.color = groundColor;
                renderer.sortingOrder = -10;

                if (spriteToUse != null)
                {
                    Vector2 spriteSize = spriteToUse.rect.size / spriteToUse.pixelsPerUnit;
                    float scale = tileSize / spriteSize.x;
                    tile.transform.localScale = new Vector3(scale, scale, 1f);
                }

                tiles[x, y] = tile;
            }
        }

        UpdateTiles();
    }

    private void LateUpdate()
    {
        if (player == null) return;
        UpdateTiles();
    }

    private void UpdateTiles()
    {
        // FloorToInt から RoundToInt に変更し、プレイヤーがタイルの中心にきやすくする
        int centerX = Mathf.RoundToInt(player.position.x / tileWidth);
        int centerY = Mathf.RoundToInt(player.position.y / tileHeight);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int gridX = centerX + (x - tileRadiusX);
                int gridY = centerY + (y - tileRadiusY);

                tiles[x, y].transform.position = new Vector3(
                    gridX * tileWidth,
                    gridY * tileHeight,
                    0f
                );
            }
        }
    }
}