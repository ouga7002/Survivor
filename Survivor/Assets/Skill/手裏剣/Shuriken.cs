using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("手裏剣のパラメータ")]
    [Tooltip("攻撃力（ATK: 10）")]
    [SerializeField] private float attackPower = 10f;

    [Tooltip("プレイヤーからの回転半径（距離）")]
    [SerializeField] private float orbitRadius = 1.8f;

    [Tooltip("回転速度（1秒間に進む度数）")]
    [SerializeField] private float rotationSpeed = 180f;

    [Tooltip("手裏剣自体の自転速度")]
    [SerializeField] private float selfRotationSpeed = 360f;

    private float currentAngle = 0f;
    private Transform playerTransform;

    void Start()
    {
        // シーン内から「player2_0」または「Player」タグを持つオブジェクトを自動で安全に探す
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null)
        {
            playerTransform = pObj.transform;
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                playerTransform = taggedPlayer.transform;
            }
            else
            {
                Debug.LogError("【Shuriken】プレイヤーが見つかりませんでした！プレイヤーの Tag を 'Player' に設定するか、名前を確認してください。");
            }
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. 時間経過で公転角度を進める
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        // 2. プレイヤーの位置を基準にして、ワールド座標上の円周上の位置を計算
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = playerTransform.position.x + Mathf.Cos(rad) * orbitRadius;
        float y = playerTransform.position.y + Mathf.Sin(rad) * orbitRadius;

        // 手裏剣の位置を更新
        transform.position = new Vector3(x, y, 0f);

        // 3. 手裏剣自体の自転（クルクル回る見た目）
        transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
    }

    // 敵に接触したときの判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            // 敵にアタッチされている EnemyHealth コンポーネントを取得する
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // HPを減らす（0になったら敵側の Die() で自動消滅します）
                enemyHealth.TakeDamage(attackPower);
                Debug.Log($"[手裏剣ヒット] 敵に {attackPower} のダメージを与えた！");
            }
        }
    }
}