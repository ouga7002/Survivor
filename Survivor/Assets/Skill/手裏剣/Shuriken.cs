using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("プレイヤーのトランスフォーム")]
    [SerializeField] private Transform player;

    [Header("手裏剣のパラメータ")]
    [Tooltip("攻撃力（ATK: 10）")]
    [SerializeField] private float attackPower = 10f;

    [Tooltip("プレイヤーからの回転半径（距離）")]
    [SerializeField] private float orbitRadius = 1.8f;

    [Tooltip("回転速度（1秒間に進む度数。例: 180なら2秒で1周）")]
    [SerializeField] private float rotationSpeed = 180f;

    [Tooltip("手裏剣自体の自転速度（クルクル回る見た目用）")]
    [SerializeField] private float selfRotationSpeed = 360f;

    private float currentAngle = 0f;

    void Start()
    {
        // 未設定の場合、自動でプレイヤーを探す
        if (player == null)
        {
            GameObject pObj = GameObject.Find("player2_0");
            if (pObj != null)
            {
                player = pObj.transform;
            }
            else
            {
                Debug.LogError("【Shuriken】プレイヤーが見つかりませんでした！");
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. 角度を時間経過とともに進めて円運動（公転）させる
        currentAngle += rotationSpeed * Time.deltaTime;

        // 360度を超えたらループさせる
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        // ラジアンに変換して座標を計算
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = player.position.x + Mathf.Cos(rad) * orbitRadius;
        float y = player.position.y + Mathf.Sin(rad) * orbitRadius;

        // 手裏剣の位置を更新
        transform.position = new Vector3(x, y, 0f);

        // 2. 手裏剣自体をクルクル自転させる（見た目の演出）
        transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
    }

    // 敵に接触したときの判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトが「Enemy」の場合
        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            Debug.Log($"[手裏剣ヒット] 敵に {attackPower} のダメージを与えて撃破！");

            // 敵を消滅させる
            Destroy(collision.gameObject);
        }
    }
}