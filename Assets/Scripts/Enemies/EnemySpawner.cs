using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 10f; // spawn just outside camera view
    [SerializeField] private int prewarmCount = 20;

    [Header("Difficulty Ramp")]
    [SerializeField] private float rampInterval = 15f;   // seconds between speed-ups
    [SerializeField] private float rampMultiplier = 0.9f; // interval shrinks by 10% each ramp
    [SerializeField] private float minSpawnInterval = 0.3f;

    private ObjectPool<EnemyHealth> enemyPool;
    private float spawnTimer;
    private float rampTimer;

    private void Start()
    {
        enemyPool = new ObjectPool<EnemyHealth>(enemyPrefab, transform, prewarmCount);
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }

        rampTimer += Time.deltaTime;
        if (rampTimer >= rampInterval)
        {
            rampTimer = 0f;
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval * rampMultiplier);
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPos = GetSpawnPositionAroundPlayer();
        EnemyHealth enemy = enemyPool.Get(spawnPos, Quaternion.identity);

        // Subscribe fresh each time this enemy re-enters play; the handler
        // unsubscribes itself on death (see HandleEnemyDeath below)
        enemy.OnDeath += HandleEnemyDeath;

        if (enemy.TryGetComponent<EnemyChase>(out var chase))
        {
            chase.SetTarget(player);
        }
    }

    private Vector2 GetSpawnPositionAroundPlayer()
    {
        // Random angle around the player, fixed distance out —
        // simple ring spawn, good enough until you want off-screen-only spawning
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        return (Vector2)player.position + offset;
    }

    private void HandleEnemyDeath(EnemyHealth enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath; // prevent stacking subscriptions on reuse
        enemyPool.Release(enemy);
    }
}