using UnityEngine;
using System.Collections;

public class GunAttack : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Transform aimCursor;
    [SerializeField] private GameObject bulletPrefab;      // 弾丸のプレハブ
    [SerializeField] private GameObject gunVisualObject;   // 銃の見た目（スプライトを持つ子オブジェクトなど）

    [Header("銃のパラメータ")]
    [SerializeField] private float baseWeaponAtk = 15f;    // 銃の基本攻撃力
    [SerializeField] private float attackInterval = 2.0f;  // 発射間隔（秒）
    [SerializeField] private float aimDistance = 1.2f;     // プレイヤーから銃が出る距離
    [SerializeField] private float showDuration = 0.2f;    // 銃が表示されている時間（発射の瞬間）

    private float attackTimer = 0f;
    private PlayerStatus playerStatus;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();

        if (aimCursor == null)
        {
            GameObject cObj = GameObject.Find("AimCursor");
            if (cObj != null) aimCursor = cObj.transform;
        }

        // 最初の構えとして、銃のグラフィックは非表示にしておく
        if (gunVisualObject != null)
        {
            gunVisualObject.SetActive(false);
        }
    }

    void Update()
    {
        if (aimCursor == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            StartCoroutine(ShootRoutine());
        }
    }

    // 銃を表示して弾を発射し、少し経ったら消すコルーチン
    IEnumerator ShootRoutine()
    {
        if (aimCursor == null) yield break;

        // 1. プレイヤーからカーソルへの方向を計算
        Vector3 dirToCursor = (aimCursor.position - transform.position).normalized;
        float angle = Mathf.Atan2(dirToCursor.y, dirToCursor.x) * Mathf.Rad2Deg;

        // 2. 銃の位置と向きを調整して「表示」する
        if (gunVisualObject != null)
        {
            gunVisualObject.transform.position = transform.position + (Vector3)(dirToCursor * aimDistance);
            gunVisualObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            gunVisualObject.SetActive(true);
        }

        // 3. 発射前のわずかなタメ（0.05秒待機）
        yield return new WaitForSeconds(0.05f);

        // 4. 弾の生成と発射
        if (bulletPrefab != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, gunVisualObject.transform.position, Quaternion.Euler(0f, 0f, angle));
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                float totalAtk = baseWeaponAtk;
                if (playerStatus != null) totalAtk += playerStatus.CurrentAtk;

                bulletScript.Initialize(dirToCursor, totalAtk);
            }
        }

        // 5. 銃が表示されている時間を待つ
        yield return new WaitForSeconds(showDuration);

        // 6. 銃を非表示にする
        if (gunVisualObject != null)
        {
            gunVisualObject.SetActive(false);
        }
    }
}