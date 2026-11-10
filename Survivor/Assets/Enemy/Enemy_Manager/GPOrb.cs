using UnityEngine;

public class GPOrb : MonoBehaviour
{
    [Header("吸引設定")]
    public float attractionRange = 5f;
    public float moveSpeed = 8f;
    public float pickupRange = 0.3f;

    private Transform player;
    private PlayerStatus playerStatus;
    private GPOrbPool pool;

    private int expAmount;
    private bool isAttracted;

    public void Initialize(GPOrbPool orbPool)
    {
        pool = orbPool;
    }

    public void Setup(int amount)
    {
        expAmount = amount;
        isAttracted = false;
        player = null;
        playerStatus = null;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null) return;

            player = playerObject.transform;
            playerStatus = playerObject.GetComponent<PlayerStatus>();
        }

        if (playerStatus == null) return;

        Vector2 difference = (Vector2)player.position - (Vector2)transform.position;
        float distanceSquared = difference.sqrMagnitude;

        // 吸引範囲に入ったら吸引開始
        if (!isAttracted)
        {
            if (distanceSquared > attractionRange * attractionRange) return;

            isAttracted = true;
        }

        // プレイヤーに向かって移動
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        // プレイヤーに接触したら経験値獲得
        if (distanceSquared <= pickupRange * pickupRange)
        {
            playerStatus.AddGatyaPoint(expAmount);
            pool.Return(this);
        }
    }
}