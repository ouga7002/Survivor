using UnityEngine;

public class LightningSkill : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float baseLightningAtk = 20f;
    [SerializeField] private float attackInterval = 4.0f;
    [SerializeField] private float displayDuration = 0.5f;
    [SerializeField] private float attackRadius = 2.5f;

    private float timer = 0f;
    private Transform playerTransform;
    private PlayerStatus playerStatus;

    void OnEnable()
    {
        FindPlayerAndStatus();
        timer = 0f;
    }

    void Start()
    {
        FindPlayerAndStatus();
    }

    void FindPlayerAndStatus()
    {
        GameObject pObj = GameObject.Find("player2_0");
        if (pObj != null)
        {
            playerTransform = pObj.transform;
            playerStatus = pObj.GetComponent<PlayerStatus>();
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                playerTransform = taggedPlayer.transform;
                playerStatus = taggedPlayer.GetComponent<PlayerStatus>();
            }
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayerAndStatus();
            if (playerTransform == null) return;
        }

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            StrikeLightning();
        }
    }

    void StrikeLightning()
    {
        if (lightningPrefab == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject targetEnemy = enemies[Random.Range(0, enemies.Length)];

        if (targetEnemy != null)
        {
            Vector3 spawnPos = targetEnemy.transform.position;
            GameObject lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            float totalAtk = baseLightningAtk;
            if (playerStatus != null) totalAtk += playerStatus.CurrentAtk;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPos, attackRadius);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") || hit.name.ToLower().Contains("enemy"))
                {
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null) enemyHealth.TakeDamage(totalAtk);
                }
            }

            Destroy(lightning, displayDuration);
        }
    }
}