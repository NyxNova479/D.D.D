using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawnerScript : MonoBehaviour
{
    [Header("listes")]
    public GameObject[] enemies;
    public List<GameObject> enemiesExisting;
    //public List<GameObject> closeEnemies;
    //public GameObject[] closeByEnemies;
    //public List<GameObject> oldCloseEnemies;

    

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

    void Update()
    {

    }
        
    void GenerateSpawnCoordinates()
    {
        // on genere une coordonee autour du joueur dans une direction au hasard, a une didtance au hasard entre a et b
        float rngDistance = Random.Range(minSpawnDistance, maxSpawnDistance); // la distance d'instantiation en a et b

        float rngX = Random.Range(-1f, 1f); // la direction d'instantiation
        float rngZ = Random.Range(-1f, 1f);

        float magnitude = Mathf.Sqrt(rngX * rngX + rngZ * rngZ); // on trouve la magnitude de la direction

        finalXCoords = rngX * rngDistance / magnitude; // on aligne la magnitude sur la distance voulue
        finalZCoords = rngZ * rngDistance / magnitude;
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnRate);

        if (enemiesExisting.Count <= 200) // 200 enemy au maximum
        {
            GenerateSpawnCoordinates();
            RaycastHit gHit; // on déclare une variable RaycastHit qui permet d'avoir des informations sur se qu'a touché le Raycast
            for (int a = 0; a < 10; a++) // On boucle 10 fois au maximum au cas ou il n'y a pas d'endroit approprie pour eviter un blocage 6 7
            {
                if (Physics.Raycast(player.transform.position + new Vector3(finalXCoords, 50, finalZCoords), -transform.up, out gHit, Mathf.Infinity)) // on genere un raycast vers le bas depuis 50 unites de haut aux coordonees X,Z
                {
                    if (gHit.transform.gameObject.layer == groundLayer || gHit.transform.gameObject.layer == climbableLayer) // si on touche le sol ou un mur
                    {
                        finalYCoords = gHit.point.y; // on garde la coordone y du point touche

                        for (int y = 0; y < (Random.Range(1, maxSpawnStacking)); y++) // le nombre d'ennemi a empiler l'un sur l'autre au spawn
                        {
                            for (int f = 0; f < failedSpawns + 1; f++) // on ajoute les enemis qui ont rates leurs spawn
                            {
                                //                   instantie           un ennemi au hasard                         autour du joueur   la ou il y a un sol approprie + le nombre d'enemi a empiler - la hauteur du joueur pour compensser 
                                var instatiated = Instantiate(enemies[Random.Range(0, enemies.Length)], player.transform.position + new Vector3(finalXCoords, finalYCoords + f - player.transform.position.y, finalZCoords), transform.rotation); // tu apparait ici                                                                                                                                  
                                instatiated.GetComponent<UniversalEnemyScript>().target = player; // ta cible c'est lui
                                instatiated.GetComponent<UniversalEnemyScript>().instantiator = gameObject; // c'est moi qui t'ai creer
                                enemiesExisting.Add(instatiated); // tu vas dans la liste
                                //Debug.Log(finalYCoords + f);
                                if (failedSpawns > 0)
                                {
                                    failedSpawns--;
                                }
                            }
                        }
                        break; // on arrete la boucle for
                    }
                    else //sinon on regenere des nouvelles coordonees X,Z
                    {
                        GenerateSpawnCoordinates();
                    }
                }
                if (a >= 9)
                {
                    failedSpawns += 1; // si on rate les 10 essais on n'instantie pas d'ennemi et on l'ajoutera plus tards
                    break;
                }
            }
        }
        StartCoroutine("SpawnTimer");
    }
}
