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

    [Header("稼働タイミング設定")]
    [Tooltip("手裏剣が出現して攻撃している時間（秒）")]
    [SerializeField] private float activeDuration = 5.0f;

    [Tooltip("手裏剣が消えているクールタイムの時間（秒）")]
    [SerializeField] private float coolDownDuration = 3.0f;

    private float currentAngle = 0f;
    private Transform playerTransform;

    // 状態管理用タイマーとフラグ
    private float stateTimer = 0f;
    private bool isActiveState = true; // true: 攻撃中(3秒), false: クールタイム中(3秒)
    private SpriteRenderer spriteRenderer;
    private Collider2D col2d;

    void Start()
    {
        // コンポーネントの取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2d = GetComponent<Collider2D>();

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

        // 初期状態の適用
        SetShurikenActive(true);
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. タイマーを進めて状態を切り替える
        stateTimer += Time.deltaTime;

        if (isActiveState)
        {
            // --- 攻撃中の処理 (3秒間) ---
            if (stateTimer >= activeDuration)
            {
                // クールタイムへ移行
                isActiveState = false;
                stateTimer = 0f;
                SetShurikenActive(false);
                Debug.Log("[手裏剣] クールタイムに入ります（3秒間非表示）");
            }
        }
        else
        {
            // --- クールタイム中の処理 (3秒間) ---
            if (stateTimer >= coolDownDuration)
            {
                // 攻撃状態へ復帰
                isActiveState = true;
                stateTimer = 0f;
                SetShurikenActive(true);
                Debug.Log("[手裏剣] 再び手裏剣が出現しました！");
            }
            // クールタイム中は位置の更新や回転を行わない
            return;
        }

        // 2. 攻撃中の回転・移動処理
        // 時間経過で公転角度を進める
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        // プレイヤーの位置を基準にして、ワールド座標上の円周上の位置を計算
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = playerTransform.position.x + Mathf.Cos(rad) * orbitRadius;
        float y = playerTransform.position.y + Mathf.Sin(rad) * orbitRadius;

        // 手裏剣の位置を更新
        transform.position = new Vector3(x, y, 0f);

        // 3. 手裏剣自体の自転（クルクル回る見た目）
        transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
    }

    // 手裏剣の表示・非表示（および当たり判定）を一括切り替えするメソッド
    void SetShurikenActive(bool active)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = active;
        if (col2d != null) col2d.enabled = active;
    }

    // 敵に接触したときの判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // クールタイム中は判定しない
        if (!isActiveState) return;

        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            // 敵にアタッチされている EnemyHealth コンポーネントを取得する
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // HPを減らす
                enemyHealth.TakeDamage(attackPower);
                Debug.Log($"[手裏剣ヒット] 敵に {attackPower} のダメージを与えた！");
            }
        }
    }
}