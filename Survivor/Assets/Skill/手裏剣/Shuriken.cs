using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("手裏剣のプレハブをここにアタッチする")]
    [SerializeField] private GameObject shurikenPrefab;

    [Header("手裏剣のパラメータ")]
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float orbitRadius = 1.8f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float selfRotationSpeed = 360f;
    [SerializeField] private float activeDuration = 5.0f;
    [SerializeField] private float coolDownDuration = 3.0f;

    // ガチャで獲得したかどうか
    public bool isUnlocked = false;

    private GameObject spawnedShuriken;
    private float currentAngle = 0f;
    private float stateTimer = 0f;
    private bool isActiveState = true;

    void Update()
    {
        // 未取得の場合は、もし手裏剣が出ていれば消して何もしない
        if (!isUnlocked)
        {
            if (spawnedShuriken != null)
            {
                Destroy(spawnedShuriken);
            }
            return;
        }

        // 取得済みで、まだ手裏剣の実体が生成されていなければ生成する
        if (spawnedShuriken == null && shurikenPrefab != null)
        {
            spawnedShuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            spawnedShuriken.transform.SetParent(transform); // プレイヤーの子にする
        }

        // アクティブ（表示・攻撃中）とクールダウンの切り替え
        if (isActiveState)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= activeDuration)
            {
                isActiveState = false;
                stateTimer = 0f;
                if (spawnedShuriken != null) spawnedShuriken.SetActive(false);
            }
        }
        else
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= coolDownDuration)
            {
                isActiveState = true;
                stateTimer = 0f;
                if (spawnedShuriken != null) spawnedShuriken.SetActive(true);
            }
            return;
        }

        // 手裏剣をプレイヤーの周りで回転させる
        if (spawnedShuriken != null && spawnedShuriken.activeSelf)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            float rad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * orbitRadius;
            float y = Mathf.Sin(rad) * orbitRadius;

            spawnedShuriken.transform.localPosition = new Vector3(x, y, 0f);
            spawnedShuriken.transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
        }
    }
}