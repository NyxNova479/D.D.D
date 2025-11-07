using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.ShaderGraph.Internal;
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
    
    [Header("cible de l'arme")]
    public bool aimAtClosestTarget;
    public int targetLayer;

    [Header("limite de projectile")]
    public bool hasProjectileLimit;
    public int projectileLimit;
    public int currentAmountOfProjectile;

    public bool giveInstantiator;
    public bool isDinosaurEggWeapon;
    public Vector3 oldPos;
    public float currentVelocity;
    void Start()
    {
        StartCoroutine("SecondTimer");
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

        

    }
    void Shoot()
    {
        
        currentAmountOfProjectile++;
        var instantiated = Instantiate(projectile, projectileOrigin.position, Quaternion.identity);
        if (aimAtClosestTarget)
        {
            GameObject closestEnemy = FindClosestObject(transform.position, range, targetLayer); // on utilise la fonction "FindClosestObject" pour que "closestEnemy" = le resultat de la fonction

            if (closestEnemy != null) // si il y a un resultat a la fonction "FindClosestObject"
            {
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
            if(!isDinosaurEggWeapon)
        {
             instantiatedScript.hasProjectileLimit = true;
            instantiatedScript.instantiator = gameObject;
        }
            else
            {
                instantiated.GetComponent<AllyDinoScript>().instantiator = gameObject;
                instantiated.GetComponent<AllyDinoScript>().hasProjectileLimit = true;
            }
               
            

            //dans le projectile dans la fonction destruction:
            /*if (hasProjectileLimit)
            {
                instantiator.GetComponent<UniversalWeaponScript>().currentAmountOfProjectile--;
            }

            public void NewProjectileStats(float damage,float speed,float SizeMultiplier)
            {
                currentDamage = damage;
                currentSpeed = speed;
                currentSizeMultiplier = SizeMultiplier;
            }
            */
        }
    }

    GameObject FindClosestObject(Vector3 center, float radius, int layer) // une fonction pour trouver l'objet le plus proche depuis un point, dans un rayon sur une couche de son choix
    {
        float smallestDistance = radius * radius; // = la plus grande variable 
        GameObject closestObject = gameObject; // on est oblige de mettre un gameObject par defaut sinon unity explose

        Collider[] hitColliders = Physics.OverlapSphere(center, radius * radius, 1 << layer); // on cherche tout les gameobjects dans un rayon et on les met dans l'array "hitColliders"
        // on multiplie radius au carre pour eviter d'utiliser "magnitude" plus bas qui bien plus couteux en ressources

        foreach (var hitCollider in hitColliders) // pour chaques gameobjects trouves
        {
            float currentDistance = (center - hitCollider.transform.position).sqrMagnitude; // on calcul la distance entre le centre et l'objet actuel
                                                                           // on evite la racine carre avec "sqrMagnitude"
            Debug.Log(currentDistance);                                                                                 // on cherche la distance la plus petite
            if (smallestDistance > currentDistance) // si la distance actuelle est plus petite que la precedente
            {
                
                smallestDistance = currentDistance; // la distance actuelle devient la distance la plus courte
                closestObject = hitCollider.transform.gameObject; // le resultat final = l'objet actuel
            }
            
        } // apres avoir compare toutes les distances de tout les gameobject trouves on se retrouve avec la distance la plus petite

        if (closestObject != gameObject) // si le resultat n'est pas le gameobject etabli par defaut
        {
            
            return (closestObject); // le resultat de la fonction = closestObject;
        }
        else
        {
            return (null); // le resultat de la fonction est rien
        }
    }

    public void NewWeaponStats(float newDamageMultiplierToAdd, float newFireRateMultiplierToAdd, float newProjectileSizeMultiplierToAdd) // on appel ca quand les statistiques devront changer en temps reel
    {
        damageMultiplier += newDamageMultiplierToAdd;
        fireRateMultiplier += newFireRateMultiplierToAdd;
        projectileSizeMultiplier += newProjectileSizeMultiplierToAdd;
        UpdateWeaponStats();        
    }

    void UpdateWeaponStats() // appliquer les changements de statistique
    {
        currentDamage = baseDamage * damageMultiplier;
        currentFireRate = baseFireRate * fireRateMultiplier;
        currentProjectileSize = baseProjectileSize * projectileSizeMultiplier;
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

    IEnumerator SecondTimer()
    {
        if (attackSpeedScalesWithVelocity)
        {
            currentFireRate = (baseFireRate / (currentVelocity * ScaleSpeedIntensity));
            if (currentFireRate > 2.3f)
            {
                currentFireRate = 2.3f;
            }
        }
        yield return new WaitForSeconds(0.1f);
        currentVelocity = (oldPos - transform.position).magnitude;
        oldPos = transform.position;
        StartCoroutine("SecondTimer");
        
    }
}
