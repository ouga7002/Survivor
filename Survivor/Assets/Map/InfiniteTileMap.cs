using UnityEngine;

/// <summary>
/// 無限ループ型の2Dマップを管理するクラス。
/// 地面タイルを必要な数だけ生成し、プレイヤーの周囲で使い回す。
/// </summary>
public class InfiniteTileMap : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("マップ設定")]
    [SerializeField] private float tileSize = 10f;
    [SerializeField] private int tileRadius = 2;

    [Header("地面の色")]
    [SerializeField]
    private Color groundColor =
        new Color(0.45f, 0.65f, 0.38f);

    // 生成した地面タイルを保存する配列
    private GameObject[,] tiles;

    // タイルに使用する四角形のスプライト
    private Sprite squareSprite;

    private int gridCount;

    private void Start()
    {
        // プレイヤーが設定されていなければ停止
        if (player == null)
        {
            Debug.LogError("MapManagerにPlayerを設定してください。");
            enabled = false;
            return;
        }

        // 1ユニットの白い四角形を作成
        squareSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 2, 2),
            new Vector2(0.5f, 0.5f),
            2f
        );

        // 中心の周囲にタイルを配置する
        gridCount = tileRadius * 2 + 1;
        tiles = new GameObject[gridCount, gridCount];

        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                GameObject tile = new GameObject(
                    $"GroundTile_{x}_{y}"
                );

                tile.transform.SetParent(transform);

                // 描画用コンポーネントを追加
                SpriteRenderer renderer =
                    tile.AddComponent<SpriteRenderer>();

                renderer.sprite = squareSprite;
                renderer.color = groundColor;

                // 地面をキャラクターより後ろに表示
                renderer.sortingOrder = -10;

                // タイルを設定した大きさにする
                tile.transform.localScale =
                    Vector3.one * tileSize;

                tiles[x, y] = tile;
            }
        }

        // 最初の配置を行う
        UpdateTiles();
    }

    private void LateUpdate()
    {
        // プレイヤーの移動後にタイルの位置を更新
        UpdateTiles();
    }

    private void UpdateTiles()
    {
        // プレイヤーがいるタイルの番号を計算
        int centerX = Mathf.FloorToInt(
            player.position.x / tileSize
        );

        int centerY = Mathf.FloorToInt(
            player.position.y / tileSize
        );

        // プレイヤーの周囲にタイルを並べる
        for (int x = 0; x < gridCount; x++)
        {
            for (int y = 0; y < gridCount; y++)
            {
                // 配列のインデックスから、プレイヤーを中心としたワールド座標のオフセットを計算
                int gridX = centerX + (x - tileRadius);
                int gridY = centerY + (y - tileRadius);

                tiles[x, y].transform.position =
                    new Vector3(
                        gridX * tileSize,
                        gridY * tileSize,
                        0f
                    );
            }
        }
    }
}