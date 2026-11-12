using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("手裏剣のパラメータ")]
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float orbitRadius = 1.8f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float selfRotationSpeed = 360f;
    [SerializeField] private float activeDuration = 5.0f;
    [SerializeField] private float coolDownDuration = 3.0f;

    [Header("手裏剣の見た目設定")]
    [Tooltip("手裏剣として表示するスプライト画像があればここにアタッチ")]
    [SerializeField] private Sprite shurikenSprite;

    public bool isUnlocked = false;

    private float currentAngle = 0f;
    private float stateTimer = 0f;
    private bool isActiveState = true;

    // プレイヤー本体のSpriteRendererを巻き込まないように専用のオブジェクトを作るか、自分自身を回さないようにする
    private Transform visualChild;
    private SpriteRenderer visualSr;
    private Collider2D col2d;

    void Start()
    {
        // プレイヤー自身に付いている Collider2D を誤って消さないよう、
        // 自身のトリガー用コライダー（または手裏剣用）を安全に取得・追加する
        col2d = GetComponent<Collider2D>();

        // 手裏剣の見た目用の孫（または子）オブジェクトがなければ自動生成する
        Transform existingVisual = transform.Find("ShurikenVisual");
        if (existingVisual != null)
        {
            visualChild = existingVisual;
            visualSr = visualChild.GetComponent<SpriteRenderer>();
        }
        else
        {
            GameObject visualObj = new GameObject("ShurikenVisual");
            visualObj.transform.SetParent(transform);
            visualObj.transform.localPosition = Vector3.zero;
            visualSr = visualObj.AddComponent<SpriteRenderer>();
            if (shurikenSprite != null) visualSr.sprite = shurikenSprite;
            visualChild = visualObj.transform;
        }

        SetShurikenActive(false);
    }

    void Update()
    {
        if (!isUnlocked) return;

        if (!isActiveState)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= coolDownDuration)
            {
                isActiveState = true;
                stateTimer = 0f;
                SetShurikenActive(true);
            }
            return;
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= activeDuration)
        {
            isActiveState = false;
            stateTimer = 0f;
            SetShurikenActive(false);
            return;
        }

        // プレイヤーの周囲を回転させる
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        float rad = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * orbitRadius;
        float y = Mathf.Sin(rad) * orbitRadius;

        if (visualChild != null)
        {
            visualChild.localPosition = new Vector3(x, y, 0f);
            visualChild.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
        }
    }

    void SetShurikenActive(bool active)
    {
        if (visualSr != null) visualSr.enabled = active;
        // ※もしプレイヤーのコライダーまで消えてしまっている場合はここのコメントアウトを調整
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isUnlocked || !isActiveState) return;

        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackPower);
            }
        }
    }
}