using UnityEngine;
using System.Collections;

public class UniversalProjectileScript : MonoBehaviour
{
    [Header("degats")]
    public float baseDamage;
    public float currentDamage;
    [Header("mouvement")]
    public float currentSpeed;
    public bool useGravity;
    [Header("collision")]
    public SphereCollider sphereCollider;
    public int hitsBeforeDestroy;
    public int currentHitsAmount;
    [Header("tete chercheuse")]
    public GameObject target;
    [Header("explosion")]
    public bool explodes;
    public float explosionRadius;
    public GameObject visualExplosion;
    [Header("taille")]
    public float size;
    public float currentSizeMultiplier;
    [Header("destruction")]
    public GameObject instantiateOnDestruction;
    public float lifeSpan;
    [Header("object generaux")]
    public GameObject instantiator;
    public bool hasProjectileLimit;
    public GameObject instantiateOnSpawn;
    public int targetLayer;
    public Color damageDisplayColor;
    void Start()
    {
        if (instantiateOnSpawn != null)
        {
            var instantiated = Instantiate(instantiateOnSpawn, transform.position, Quaternion.identity);
            instantiated.transform.localScale = new Vector3(size, size, size);
        }
        StartCoroutine("LifeSpanTimer");
        sphereCollider = GetComponent<SphereCollider>();
        //currentDamage = baseDamage;
        gameObject.transform.localScale = new Vector3(size, size, size) * currentSizeMultiplier; // modifier la taille de l'objet
    }

    void Update()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime; // avancer devant
        if (target != null && currentHitsAmount == 0) // si il y a une cible et qu'il n'a rien touche
        {
            SeekTarget();
        }        
    }

    void OnTriggerEnter(Collider collision) // le collider est un trigger pour passer a travers les ennemis
    {
        
        if (collision.gameObject.GetComponent<UniversalEnemyScript>()) // verifier l'identite de l'objet touche
        {
            var hitScript = collision.gameObject.GetComponent<UniversalEnemyScript>();
            hitScript.currentHealthPoint -= currentDamage; // infliger des degats a l'objet touche
            hitScript.colorOfTakenDamage = damageDisplayColor;
        }
        currentHitsAmount++; // compter le nombre de collision
        if (explodes)
        {
            float finalExplosionSize = size * explosionRadius * currentSizeMultiplier;
            sphereCollider.radius = finalExplosionSize; // l'explosion c'est juste faire grossir la hitbox de base
            currentSpeed = 0;           
            StartCoroutine("TimedDestructionInitiation");
            var instantiated = Instantiate(visualExplosion, transform.position, Quaternion.identity);
            instantiated.transform.localScale = new Vector3 (finalExplosionSize, finalExplosionSize, finalExplosionSize) * 2;
        }
        else if (currentHitsAmount >= hitsBeforeDestroy) // detruire l'objet si il est a court de collision       
        {
            InitiateDestruction();
        }   
    }

    void SeekTarget()
    {
        transform.LookAt(target.transform.position); // s'oriente vers la position de la cible
    }

    IEnumerator LifeSpanTimer()
    {
        yield return new WaitForSeconds(lifeSpan);
        InitiateDestruction();
    }

    public void NewProjectileStats(float damage, float speed, float SizeMultiplier)
    {
        currentDamage = damage;
        currentSpeed = speed;
        currentSizeMultiplier = SizeMultiplier;
    }

    void InitiateDestruction() // detruire
    {
        Destroy(gameObject);
    }
    public void StartTimedDestructionInitiation() // la coroutine ne fonctionnait pas avec getComponent
    {
        StartCoroutine(TimedDestructionInitiation());
    }
    IEnumerator TimedDestructionInitiation() // detruire apres 0.1 seconde
    {
        yield return new WaitForSeconds(0.1f);
        InitiateDestruction();
    }

    /*private void OnDrawGizmos()
    {
        // Set the color
        Gizmos.color = new Color(1f, 0f, 0f); // Red 

        // Draw the sphere
        //Gizmos.DrawSphere(transform.position, range);

        // Draw wire sphere outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size * explosionRadius * currentSizeMultiplier);
    }*/
}
