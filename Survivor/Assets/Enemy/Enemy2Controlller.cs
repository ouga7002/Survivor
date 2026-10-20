using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    public float moveSpeed;
    public float Attack = 1;
    public float HP = 10;

    public int Spd;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        int Spd = Random.Range(1,4);

        moveSpeed = Spd;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
}