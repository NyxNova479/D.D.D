using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class EnemySpawnerScript : MonoBehaviour
{
    [Header("listes")]
    public GameObject[] enemies;
    public List<GameObject> enemiesExisting;

    [Header("Objets generaux")]
    public GameObject player;
    public int groundLayer;
    public int climbableLayer;

    [Header("Spawn")]
    public float spawnRate;
    public float maxSpawnDistance;
    public float minSpawnDistance;
    public int maxSpawnStacking;
    public float finalXCoords;
    public float finalYCoords;
    public float finalZCoords;

    public int failedSpawns;

    void Start()
    {
        StartCoroutine("SpawnTimer");
    }

    void GenerateSpawnCoordinates()
    {
        float rngDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

        float rngX = Random.Range(-1f, 1f);
        float rngZ = Random.Range(-1f, 1f);

        float magnitude = Mathf.Sqrt(rngX * rngX + rngZ * rngZ);

        finalXCoords = rngX * rngDistance / magnitude;
        finalZCoords = rngZ * rngDistance / magnitude;

        finalYCoords = 0f; // remise à 0 (correction)
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnRate);

        if (enemiesExisting.Count <= 200)
        {
            GenerateSpawnCoordinates();

            // LayerMask pour Raycast
            int mask = (1 << groundLayer) | (1 << climbableLayer);

            for (int a = 0; a < 10; a++)
            {
                RaycastHit gHit;

                if (Physics.Raycast(
                    player.transform.position + new Vector3(finalXCoords, 50, finalZCoords),
                    Vector3.down,
                    out gHit,
                    200f,
                    mask))
                {
                    // Vérifie que la surface est assez plate
                    if (Vector3.Angle(gHit.normal, Vector3.up) > 40f)
                    {
                        GenerateSpawnCoordinates();
                        continue;
                    }

                    // Position de base du spawn (plus haut pour éviter de tomber dans le sol)
                    float spawnBaseY = gHit.point.y + 0.5f;

                    finalYCoords = spawnBaseY;

                    for (int y = 0; y < Random.Range(1, maxSpawnStacking); y++)
                    {
                        for (int f = 0; f < failedSpawns + 1; f++)
                        {
                            Vector3 spawnPos = new Vector3(
                                player.transform.position.x + finalXCoords,
                                spawnBaseY + y + f,
                                player.transform.position.z + finalZCoords
                            );

                            GameObject instatiated = Instantiate(
                                enemies[Random.Range(0, enemies.Length)],
                                spawnPos,
                                transform.rotation
                            );

                            Collider col = instatiated.GetComponent<Collider>();
                            if (col != null)
                            {
                                float safetyHeight = col.bounds.size.y;
                                instatiated.transform.position += new Vector3(0, safetyHeight, 0); // Ajout d'une hauteur pour eviter de le faire propulser dans les airs a cause de son rigibody. (ou traverser le sol aussi)
                            }

                            instatiated.transform.parent = transform;

                            instatiated.GetComponent<UniversalEnemyScript>().target = player;
                            instatiated.GetComponent<UniversalEnemyScript>().instantiator = gameObject;

                            enemiesExisting.Add(instatiated);

                            if (failedSpawns > 0) failedSpawns--;
                        }
                    }

                    break; // place trouvée
                }

                if (a >= 9)
                {
                    failedSpawns += 1;
                    break;
                }
            }
        }

        StartCoroutine("SpawnTimer");
    }
}
