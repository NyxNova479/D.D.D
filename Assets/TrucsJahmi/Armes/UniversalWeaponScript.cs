using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using static UnityEngine.GraphicsBuffer;
//using UnityEditor.ShaderGraph.Internal;
public class UniversalWeaponScript : MonoBehaviour
{
    [Header("projectile de l'arme")]
    public GameObject projectile;
    public Transform projectileOrigin;

    [Header("statistiques de l'arme")]
    public float baseDamage;
    public float currentDamage;
    public float damageMultiplier;

    public float baseFireRate;
    public float currentFireRate;
    public float fireRateMultiplier;
    float currentFireRateTimer;

    public float baseProjectileSize;
    public float currentProjectileSize;
    public float projectileSizeMultiplier;

    public float projectileSpeed;

    public float range;

    public bool attackSpeedScalesWithVelocity;
    public float ScaleSpeedIntensity; // pas 0 tdb
    public float currentMovementSpeed;

    float velocityMeasurementTimer;
    
    [Header("cible de l'arme")]
    public bool aimAtClosestTarget;
    public int targetLayer;

    [Header("limite de projectile")]
    public bool hasProjectileLimit;
    public int projectileLimit;
    public int currentAmountOfProjectile;

    public bool rememberProjectilesInList;
    public List<GameObject> liveProjectileList;

    bool resetIsNotDone;

    public bool giveInstantiator;
    public bool isDinosaurEggWeapon;
    public Vector3 oldPos;
    public float currentVelocity;

    void Start()
    {
        //StartCoroutine("SecondTimer");
        oldPos = transform.position;
        
        UpdateWeaponStats();
    }

    void Update()
    {
        if (currentFireRateTimer < currentFireRate) // un timer simple
        {
            currentFireRateTimer += Time.deltaTime;
        }
        else // quand le timer est a 100%
        {
            currentFireRateTimer = 0;
            if (hasProjectileLimit)
            {
                if (currentAmountOfProjectile < projectileLimit) // si on n'a pas ateint la limite de projectiles
                {
                    Shoot();
                }
            }
            else
            {
                Shoot();
            }
        }

        if (resetIsNotDone)
        {
            ResetThisWeapon();
        }

        if (attackSpeedScalesWithVelocity)
        {
            if (transform.position != oldPos)
            {
                oldPos = transform.position;
                currentFireRate = (baseFireRate / (currentMovementSpeed * ScaleSpeedIntensity)) * fireRateMultiplier;
            }
            else
            {
                currentFireRate = baseFireRate * fireRateMultiplier;
            }
                //MeasureVelocity();
        }        
    }
    void Shoot()
    {

        if (hasProjectileLimit)
        {
            currentAmountOfProjectile++;
        }
        
        var instantiated = Instantiate(projectile, projectileOrigin.position, Quaternion.identity);
        if (aimAtClosestTarget)
        {
            GameObject closestEnemy = FindClosestObject(transform.position, range, targetLayer); // on utilise la fonction "FindClosestObject" pour que "closestEnemy" = le resultat de la fonction

            if (closestEnemy != null) // si il y a un resultat a la fonction "FindClosestObject"
            {
                //Debug.Log("ennemi proche: " + closestEnemy);
                instantiated.transform.LookAt(closestEnemy.transform.position); // on oriente le projectile vers la cible
            }
        }

        var instantiatedScript = instantiated.GetComponent<UniversalProjectileScript>();

        if (!isDinosaurEggWeapon)
        {
            instantiatedScript.NewProjectileStats(currentDamage, projectileSpeed, projectileSizeMultiplier);
        }
        else
        {
            instantiated.GetComponent<AllyDinoScript>().NewProjectileStats(currentDamage, projectileSpeed, projectileSizeMultiplier);
        }
        if (giveInstantiator)
        {
            instantiatedScript.instantiator = gameObject;
        }
        if (hasProjectileLimit) // il faut merge avant d'utiliser tout ca sinon il y aura des conflits et tout ( en vrai je c pa )
        {
            if (!isDinosaurEggWeapon)
            {
                instantiatedScript.hasProjectileLimit = true;
                instantiatedScript.instantiator = gameObject;
            }
            else
            {
                instantiated.GetComponent<AllyDinoScript>().instantiator = gameObject;
                instantiated.GetComponent<AllyDinoScript>().hasProjectileLimit = true;
            }
            if (rememberProjectilesInList)
            {
                liveProjectileList.Add(instantiated);
            }
        }
    }

    GameObject FindClosestObject(Vector3 center, float radius, int layer) // une fonction pour trouver l'objet le plus proche depuis un point, dans un rayon sur une couche de son choix
    {
        float smallestDistance = Mathf.Infinity; // = la plus grande variable 
        GameObject closestObject = null; // on est oblige de mettre une valeur par defaut sinon unity explose

        Collider[] hitColliders = Physics.OverlapSphere(center, radius * radius, 1 << layer); // on cherche tout les gameobjects dans un rayon et on les met dans l'array "hitColliders"
        // on multiplie radius au carre pour eviter d'utiliser "magnitude" plus bas qui bien plus couteux en ressources

        foreach (var hitCollider in hitColliders) // pour chaques gameobjects trouves
        {
            
                float currentDistance = (center - hitCollider.transform.position).sqrMagnitude; // on calcul la distance entre le centre et l'objet actuel
                                                                                                // on evite la racine carre avec "sqrMagnitude"
                // on cherche la distance la plus petite
                if (smallestDistance > currentDistance) // si la distance actuelle est plus petite que la precedente
                {
                
                    smallestDistance = currentDistance; // la distance actuelle devient la distance la plus courte
                    closestObject = hitCollider.transform.gameObject; // le resultat final = l'objet actuel
                }

            
        } // apres avoir compare toutes les distances de tout les gameobject trouves on se retrouve avec la distance la plus petite

        if (closestObject != null) // si le resultat n'est pas le gameobject etabli par defaut
        {
            return (closestObject); // le resultat de la fonction = closestObject;
            
        }
        else
        {
            return (null); // le resultat de la fonction est rien
        }
    }

    public void NewWeaponStats(float newDamageMultiplier, float newFireRateMultiplier, float newProjectileSizeMultiplier) // on appel ca quand les statistiques devront changer en temps reel
    {
        damageMultiplier = newDamageMultiplier;
        fireRateMultiplier = newFireRateMultiplier;
        projectileSizeMultiplier = newProjectileSizeMultiplier;
        UpdateWeaponStats();        
    }

    void UpdateWeaponStats() // appliquer les changements de statistique
    {
        currentDamage = baseDamage * damageMultiplier;
        currentFireRate = baseFireRate * fireRateMultiplier;
        currentProjectileSize = baseProjectileSize * projectileSizeMultiplier;
        /*if (attackSpeedScalesWithVelocity)
        {
            currentMovementSpeed = GetComponentInParent<PlayerStatsScript>().currentMovementSpeed;
            currentFireRate = (baseFireRate * fireRateMultiplier) / currentMovementSpeed;
        }*/
    }

    public void UpdateMovementSpeed(float newMovementSpeed)
    {
        currentMovementSpeed = newMovementSpeed;
    }

    public void ResetThisWeapon()
    {
        if (rememberProjectilesInList)
        {
            resetIsNotDone = true;
            if (liveProjectileList.Count != 0)
            {
                if (isDinosaurEggWeapon)
                {
                    liveProjectileList[0].GetComponent<AllyDinoScript>().StartTimedDestructionInitiation();
                }
                else
                {
                    liveProjectileList[0].GetComponent<UniversalProjectileScript>().StartTimedDestructionInitiation();
                }

            }
            else
            {
                resetIsNotDone = false;
            }
            /*foreach (GameObject projectile in liveProjectileList)
            {
                if (isDinosaurEggWeapon)
                {
                    projectile.GetComponent<AllyDinoScript>().StartTimedDestructionInitiation();
                }
            }*/
            //currentAmountOfProjectile = 0;     //liveProjectileList.Clear();   
        }
    }

    private void OnDrawGizmos()
    {
        // Set the color
        //Gizmos.color = new Color(1f, 0f, 0f); // Red 

        // Draw the sphere
        //Gizmos.DrawSphere(transform.position, range);

        // Draw wire sphere outline
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void MeasureVelocity()
    {
        if (transform.position != oldPos)
        {
            if (velocityMeasurementTimer < 0.1f)
            {
                velocityMeasurementTimer += Time.deltaTime;
            }
            else
            {
                velocityMeasurementTimer = 0;
                currentVelocity = (oldPos - transform.position).magnitude;
                oldPos = transform.position;
                currentFireRate = 1 + (baseFireRate / (currentVelocity * ScaleSpeedIntensity)) * fireRateMultiplier;
            }

        }
    }

    IEnumerator SecondTimer()
    {
        if (attackSpeedScalesWithVelocity)
        {
            currentFireRate = (baseFireRate / (currentVelocity * ScaleSpeedIntensity)) * fireRateMultiplier;

            /*if (currentFireRate > 2.3f)
            {
                currentFireRate = 2.3f;
            }*/
        }
        yield return new WaitForSeconds(0.1f);
        currentVelocity = (oldPos - transform.position).magnitude;
        Debug.Log(currentFireRate);
        oldPos = transform.position;
        StartCoroutine("SecondTimer");        
    }
}
