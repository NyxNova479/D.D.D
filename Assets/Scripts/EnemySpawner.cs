using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject spawnVFXPrefab;

    [Header("References")]
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Spawn Settings")]
    public float minSpawnRadius = 5f;
    public float maxSpawnRadius = 15f;
    public float spawnHeightOffset = 1f;
    public float riseDuration = 0.5f;
    public float checkRadius = 1f; // Zone libre autour du spawn

    [Header("Wave System")]
    public int initialEnemiesPerWave = 3;
    public float timeBetweenWaves = 5f;
    public float spawnDelayBetweenEnemies = 0.3f;
    public bool progressiveDifficulty = true;
    public float difficultyIncreaseRate = 1.2f;
    public int maxEnemiesPerWave = 20;

    [Header("Spawn Limits")]
    public int maxActiveEnemies = 30;
    public float vfxDestroyDelay = 2f;

    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Assign player or add 'Player' tag.");
        }

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        // Nettoyer la liste des ennemis morts
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            if (player == null) yield break;

            currentWave++;
            int enemiesThisWave = CalculateEnemiesForWave(currentWave);

            Debug.Log($"=== WAVE {currentWave} === Spawning {enemiesThisWave} enemies");

            isSpawning = true;

            for (int i = 0; i < enemiesThisWave; i++)
            {
                // Respecter la limite d'ennemis actifs
                if (activeEnemies.Count >= maxActiveEnemies)
                {
                    Debug.Log("Max enemies reached, waiting...");
                    yield return new WaitUntil(() => activeEnemies.Count < maxActiveEnemies);
                }

                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelayBetweenEnemies);
            }

            isSpawning = false;

            // Attendre que tous les ennemis soient tu�s avant la prochaine vague
            yield return new WaitUntil(() => activeEnemies.Count == 0);

            Debug.Log("Wave cleared! Next wave in " + timeBetweenWaves + " seconds");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    int CalculateEnemiesForWave(int wave)
    {
        if (!progressiveDifficulty)
            return initialEnemiesPerWave;

        int enemies = Mathf.RoundToInt(initialEnemiesPerWave * Mathf.Pow(difficultyIncreaseRate, wave - 1));
        return Mathf.Min(enemies, maxEnemiesPerWave);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos;
        bool validPosFound = FindValidSpawnPosition(out spawnPos);

        if (!validPosFound)
        {
            Debug.LogWarning("Could not find valid spawn position after multiple tries");
            return;
        }

        // Position de d�part sous le sol
        Vector3 startPos = spawnPos - Vector3.up * spawnHeightOffset;

        // Instancier l'ennemi
        GameObject enemy = Instantiate(enemyPrefab, startPos, Quaternion.identity);

        // D�sactiver temporairement le Rigidbody pendant l'animation
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // D�sactiver l'AI pendant le spawn
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        // Ajouter � la liste des ennemis actifs
        activeEnemies.Add(enemy);

        // FX de spawn
        if (spawnVFXPrefab != null)
        {
            GameObject vfx = Instantiate(spawnVFXPrefab, startPos, Quaternion.identity);
            Destroy(vfx, vfxDestroyDelay);
        }

        // Animation de mont�e
        StartCoroutine(RiseFromGround(enemy, spawnPos, startPos, rb, ai));
    }

    bool FindValidSpawnPosition(out Vector3 position)
    {
        position = Vector3.zero;
        int maxTries = 30;

        for (int i = 0; i < maxTries; i++)
        {
            // Position al�atoire autour du joueur (anneau)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );

            Vector3 testPos = player.position + offset;

            // Raycast depuis le haut pour trouver le sol
            if (Physics.Raycast(testPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, groundLayer))
            {
                testPos = hit.point;

                // V�rifier qu'il n'y a pas d'obstacles autour
                if (!Physics.CheckSphere(testPos, checkRadius, obstacleLayer))
                {
                    // V�rifier qu'il n'y a pas d'autres ennemis trop proches
                    bool tooClose = false;
                    foreach (GameObject enemy in activeEnemies)
                    {
                        if (enemy != null && Vector3.Distance(enemy.transform.position, testPos) < checkRadius * 2f)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        position = testPos;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    IEnumerator RiseFromGround(GameObject enemy, Vector3 targetPos, Vector3 startPos, Rigidbody rb, EnemyAI ai)
    {
        if (enemy == null) yield break;

        float elapsed = 0f;

        // D�sactiver les collisions pendant l'animation pour �viter les bugs
        Collider[] colliders = enemy.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        while (elapsed < riseDuration)
        {
            if (enemy == null) yield break;

            float t = elapsed / riseDuration;
            // Courbe d'ease-out pour un mouvement plus naturel
            t = 1f - Mathf.Pow(1f - t, 3f);

            enemy.transform.position = Vector3.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (enemy != null)
        {
            // Position finale forc�e l�g�rement au-dessus du sol
            enemy.transform.position = targetPos + Vector3.up * 0.1f;

            // R�activer les collisions
            foreach (var col in colliders)
            {
                col.enabled = true;
            }

            // R�activer le Rigidbody
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero; // Important: pas de v�locit� r�siduelle
            }

            // R�activer l'AI apr�s un petit d�lai
            yield return new WaitForSeconds(0.1f);

            if (ai != null && enemy != null)
            {
                ai.enabled = true;
            }
        }
    }

    // M�thodes utilitaires publiques
    public int GetCurrentWave() => currentWave;
    public int GetActiveEnemyCount() => activeEnemies.Count;
    public bool IsSpawning() => isSpawning;

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Rayon minimum de spawn (rouge)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);

        // Rayon maximum de spawn (jaune)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, maxSpawnRadius);

        // Zone de spawn (anneau vert)
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        for (float angle = 0; angle < 360; angle += 15)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = player.position + new Vector3(
                Mathf.Cos(rad) * maxSpawnRadius,
                0,
                Mathf.Sin(rad) * maxSpawnRadius
            );
            Gizmos.DrawLine(player.position, pos);
        }
    }
}