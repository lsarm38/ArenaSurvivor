using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth[] enemyPrefabs; // e.g. base Enemy, Runner, Brute
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 10f; // spawn just outside camera view
    [SerializeField] private int prewarmCountPerType = 10;

    [Header("Difficulty Ramp")]
    [SerializeField] private float rampInterval = 15f;   // seconds between speed-ups
    [SerializeField] private float rampMultiplier = 0.9f; // interval shrinks by 10% each ramp
    [SerializeField] private float minSpawnInterval = 0.3f;

    // One pool per enemy TYPE, since ObjectPool<T> is built around a single prefab
    private List<ObjectPool<EnemyHealth>> pools;

    // Tracks which pool a given live enemy instance came from, so it gets
    // released back to the correct pool (not just "a" pool) on death
    private Dictionary<EnemyHealth, ObjectPool<EnemyHealth>> instanceToPool;

    private float spawnTimer;
    private float rampTimer;

    private void Start()
    {
        pools = new List<ObjectPool<EnemyHealth>>();
        instanceToPool = new Dictionary<EnemyHealth, ObjectPool<EnemyHealth>>();

        foreach (EnemyHealth prefab in enemyPrefabs)
        {
            pools.Add(new ObjectPool<EnemyHealth>(prefab, transform, prewarmCountPerType));
        }
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
        if (pools.Count == 0) return;

        ObjectPool<EnemyHealth> pool = pools[Random.Range(0, pools.Count)];

        Vector2 spawnPos = GetSpawnPositionAroundPlayer();
        EnemyHealth enemy = pool.Get(spawnPos, Quaternion.identity);

        instanceToPool[enemy] = pool; // remember which pool to return this exact instance to

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

        if (instanceToPool.TryGetValue(enemy, out ObjectPool<EnemyHealth> pool))
        {
            pool.Release(enemy);
        }
    }
}