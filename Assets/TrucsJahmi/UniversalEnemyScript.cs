using UnityEngine;
using System.Collections;
//using TMPro;
public class UniversalEnemyScript : MonoBehaviour
{
    [Header("Objets generaux")]
    public GameObject target;
    public GameObject playerHeath;
    public GameObject instantiator;
    public GameObject model3D;
    public Enemy3DModelScript scriptModel3D;
    public int groundLayer;
    public int climbableLayer;
    public int enemyLayer;

    [Header("Points de vie")]
    public float baseHealthPoint;
    public float scaledBaseHealthPoint;
    public float oldScaledBaseHealthPoint;
    public float currentHealthPoint;
    public float currentHealthPercentage;
    public float oldHealthPercentage;
    public float oldHP;
    
    [Header("Degats")]
    public float baseDamage;
    public float scaledBaseDamage;
    public float damageToDeal;
    public float attackTimer;
    public float attackRate;
    public float attackReach;

    [Header("Mouvements")]
    public float movementSpeed;
    public bool isWalking;
    public Rigidbody rb;
    public float fallTimer;

    [Header("Capacites speciales")]
    public float minEnemySizeMultiplier;
    public float maxEnemySizeMultiplier;
    public float enemySizeMultiplier; // si c'est 0 les ennemis n'aparaitrons pas /!\

    public bool canMerge;
    public float maxMergeSize;
    [Header("Afficher degats subits")]
    public GameObject billboard;
    void Start()
    {
        StartCoroutine("SecondTimer");
        scriptModel3D = model3D.GetComponent<Enemy3DModelScript>();
        isWalking = true;
        rb.useGravity = true;        

        if (maxEnemySizeMultiplier != minEnemySizeMultiplier)
        {
            enemySizeMultiplier = Random.Range(minEnemySizeMultiplier, maxEnemySizeMultiplier);
        }
        if (enemySizeMultiplier != 1)
        {
            transform.localScale *= enemySizeMultiplier;
        }
        baseHealthPoint *= transform.localScale.x;
        currentHealthPoint = baseHealthPoint;
        oldScaledBaseHealthPoint = baseHealthPoint;
        scaledBaseHealthPoint = baseHealthPoint;
        oldHP = currentHealthPoint;
        currentHealthPercentage = 100;
        oldHealthPercentage = currentHealthPercentage;
        //model3D.GetComponent<Enemy3DModelScript>().normalSize = transform.localScale.x;
        //Debug.Log(transform.localScale);
    }
    void Update()
    {
        
        
       
        if (currentHealthPoint < oldHP) // calculer les degats qu'on a subit
        { 
            currentHealthPercentage = currentHealthPoint * 100 / scaledBaseHealthPoint;
            // afficher les degats
            var instantiated = Instantiate(billboard, transform.position, Quaternion.identity);
            instantiated.GetComponent<BillboardSpriteScript>().billboardText.text = ((int)(oldHP - currentHealthPoint)).ToString();
            instantiated.GetComponent<BillboardSpriteScript>().instantiatorSizeOffset = transform.localScale.z / 2;
            oldHP = currentHealthPoint;
            //model3D.GetComponent<Enemy3DModelScript>().tookAHit = true;

            scriptModel3D.TookDamage(oldHealthPercentage - currentHealthPercentage); // force du coup
            oldHealthPercentage = currentHealthPercentage;
        }
        if (isWalking && fallTimer > 0) // timer pour permettre au rb de grimper les bords de mesh, sans ca il retombe arrive en haut
        {
            fallTimer -= 10 * Time.deltaTime; // c'est moche, pas tres elegant
        }
        else
        {
            rb.useGravity = true;
        }
        var targetDir = target.transform.position - transform.position; // la direction de la cible par rapport a nous
        transform.rotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z)); // on s'oriente vers la cible uniquement dans l'axe x,z        
        if(isWalking)
        {
            Walk();
            GroundDetection();
        }
        else
        {
            //transform.position += (transform.up - (transform.forward * 0.5f)) * movementSpeed * Time.deltaTime;
            transform.position += transform.up * movementSpeed * Time.deltaTime; // si on touche un mur on monte
            fallTimer = 1;
            rb.useGravity = false;
        }
        
        if (currentHealthPoint <= 0)
        {
            Destruction();
        }
        if (transform.position.y < -100)
        {
            Destruction();
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == climbableLayer)
        {
            transform.position -= new Vector3(collision.GetContact(0).point.x - transform.position.x, 0, collision.GetContact(0).point.z - transform.position.z) * movementSpeed * Time.deltaTime; // on s'eloigne du point de contact du mur touche pour pouvoir grimper a plus de 90 degres
            isWalking = false;
        }
        if (canMerge && transform.localScale.x < maxMergeSize && collision.gameObject.layer == enemyLayer) // manger les autres enemis plus petits que soit et grossir proportionellement
        {
            if (collision.gameObject.transform.localScale.x < transform.localScale.x)
            {
                if (transform.localScale.x < 1)
                {
                    transform.localScale += collision.gameObject.transform.localScale / 2; // plus il est gros moins il grossit
                }
                else
                {
                    transform.localScale += collision.gameObject.transform.localScale / transform.localScale.x; // plus il est gros moins il grossit
                }
                collision.gameObject.GetComponent<UniversalEnemyScript>().Destruction();
                ScaleStatsWithSize();
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == climbableLayer)
        {
            isWalking = true;
        }
    }
    void Walk()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime; // on avance vers l'avant        
    }
    float CalculateDistanceFromTarget()
    {
        float distance = (target.transform.position - transform.position).magnitude;
        return distance;
    }
    void GroundDetection()
    {
        //RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, 0.65f)) // on utilise un raycast au lieu d'un spherecast car c'est suffisant pour l'IA des enemis
        {
            fallTimer = 1;
            rb.useGravity = false;
        }
        //Debug.DrawLine(transform.position, transform.position - transform.up * 0.65f, Color.red);
    }
    void ScaleStatsWithSize() // augmente les statistiques de l'ennemi selon la nouvelle taille en gardant les proportions actuelles
    {
        scaledBaseHealthPoint = (baseHealthPoint * transform.localScale.x) * transform.localScale.x * 2;
        float newCurrentHP = (currentHealthPoint * scaledBaseHealthPoint) / oldScaledBaseHealthPoint;
        currentHealthPoint = newCurrentHP;
        oldScaledBaseHealthPoint = scaledBaseHealthPoint;
        scaledBaseDamage = baseDamage * transform.localScale.x * transform.localScale.x * 3;
        oldHP = currentHealthPoint;
        //model3D.GetComponent<Enemy3DModelScript>().normalSize = transform.localScale.x;
    }
    void Destruction()
    {
        instantiator.GetComponent<EnemySpawnerScript>().enemiesExisting.Remove(gameObject); // on se retire des listes avant de partir :'(
        Destroy(gameObject);
    }

    //
    
    void Attack()
    {
        if( attackTimer < attackRate)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            attackTimer = 0;
            target.GetComponent<BOBMNUCLAIRscripttest>().TakeDamage(damageToDeal);
        }
    }

    IEnumerator SecondTimer() // detruire apres 0.1 seconde
    {
        
        yield return new WaitForSeconds(0.1f);
        float dist = CalculateDistanceFromTarget();
        if (dist <= attackReach)
        {
            Attack();
        }
        StartCoroutine("SecondTimer");
    }
}
