using UnityEngine;
using System.Collections;
public class AllyDinoScript : MonoBehaviour
{
    [Header("faim")]
    public float hungerPercentage;
    public int succefulAttacksCount;
    public float hungerRestoredOnHit;
    [Header("degats")]
    public float baseDamage;
    public float currentDamage;
    public float currentAttackRate;
    public float currentAttackRateTimer;
    [Header("mouvement")]
    public float currentSpeed;
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
    public int targetLayer;
    [Header("saut")]
    public Vector3 startJumpPos;
    public Vector3 endJumpPos;
    public float jumpSpeed;
    public float jumpHeight;
    public bool isJumping;
    bool isJumpingBis; // c'est un bool qui se met a jour 1 apres isJumping pour faire que certaines fonctions ne se lancent qu'1 frame
    public float targetDistance;
    public float currentJumpSpeed;
    public Rigidbody rb;

    bool destroyBool = true;

    public Color damageDisplayColor;
    void Start()
    {
        StartCoroutine("SecondTimer");
    }

    void Update()
    {
        
        if (hungerPercentage > 100) // on empeche au pourcentage de depasser 100
        {
            hungerPercentage = 100;
        }
        if (currentAttackRateTimer < currentAttackRate) // un timer simple
        {
            currentAttackRateTimer += Time.deltaTime;
        }
        else // quand le timer est a 100%
        {
            if (!isJumping) 
            {
                JumpAttack();
                currentAttackRateTimer = 0;
            }


        }
        if (isJumping)
        {
            
            //float baseY = Mathf.Lerp(startJumpPos.y, endJumpPos.y, 0.8f);
            if (currentJumpSpeed > 0.5f)
            {
                currentJumpSpeed -= 10f * Time.deltaTime;
            }
            else
            {
                AOEAttack();
                currentJumpSpeed = 0;
                isJumping = false;
            }

            //targetDistance
            transform.position += transform.forward * currentSpeed * currentJumpSpeed * Time.deltaTime;
            //rb.useGravity = false;
            /*
            float z0 = startJumpPos.z;
            float z1 = endJumpPos.z;
            float dist = z1 - z0;

            float nextZ = Mathf.MoveTowards(transform.position.z, z1, currentSpeed * Time.deltaTime);
            float baseY = Mathf.Lerp(startJumpPos.y, endJumpPos.y, (nextZ - z0) / dist);
            float arc = currentSpeed * (nextZ - z0) * (nextZ - z1) / (-0.25f * dist * dist);
            
            float x0 = startJumpPos.x;
            float x1 = endJumpPos.x;
            float nextX = Mathf.MoveTowards(transform.position.x, x1, currentSpeed * Time.deltaTime);

            Vector3 nPos = new Vector3(nextX, baseY + arc, nextZ);

            transform.position = nPos;
            if (arc > 4.5f)
            {
                hasReachedHeight = true;
            }
            if (hasReachedHeight && nPos.y == endJumpPos.y)
            {
                isJumping = false;
            }*/
            isJumpingBis = true;
        }
        else
        {
            if (isJumpingBis)
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // le disau se redresse en gardant sa rotation y
                isJumpingBis = false;
            }
            
            //rb.useGravity = true;
        }
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {

        }
    }
    void AOEAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + (transform.forward * 0.5f), transform.localScale.x * 1.2f, 1 << targetLayer); // on cherche tout les gameobjects dans un rayon et on les met dans l'array "hitColliders"
        // on multiplie radius au carre pour eviter d'utiliser "magnitude" plus bas qui bien plus couteux en ressources

        foreach (var hitCollider in hitColliders) // pour chaques gameobjects trouves
        {
            currentHitsAmount++;
            var hitScript = hitCollider.gameObject.GetComponent<UniversalEnemyScript>();
            hitScript.currentHealthPoint -= currentDamage; // infliger des degats a l'objet touche
            hitScript.colorOfTakenDamage = damageDisplayColor; //Color.green;
            /*
            var hitScript = hitCollider.gameObject.GetComponent<UniversalEnemyScript>();
        hitScript.currentHealthPoint -= currentDamage; // infliger des degats a l'objet touche
        hitScript.colorOfTakenDamage = damageDisplayColor;*/
            hungerPercentage += hungerRestoredOnHit;
            succefulAttacksCount++;
        }
        if (succefulAttacksCount >= 3)
        {
            succefulAttacksCount = 0;
            currentDamage *= 1.3f;
            size += 0.1f / size;
        }
    }
    void OnTriggerEnter(Collider collision) // le collider est un trigger pour passer a travers les ennemis
    {

        if (collision.gameObject.GetComponent<UniversalEnemyScript>()) // verifier l'identite de l'objet touche
        {
            currentJumpSpeed = 0f;
        }

    }

    void JumpAttack()
    {
        
        GameObject closestTarget = FindClosestObject(transform.position, 60, targetLayer);
        if (closestTarget != null)
        {
            Vector3 finalJumpPoisition = closestTarget.transform.position;
        
            //finalJumpPoisition = closestTarget.transform.position //- (new Vector3(0, closestTarget.transform.localScale.y + 0.1f, 0) / 2); //la position final = la ou la cible touche le sol presumement;
            JumpFromThisToThis(transform.position, finalJumpPoisition, 5, 10);
        }
        else
        {
            JumpFromThisToThis(transform.position, instantiator.transform.position + new Vector3(Random.Range(0f, 5f), 3, Random.Range(0f, 5f)), 5, 10);
        }
        transform.LookAt(endJumpPos);
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
                                                                                            // on cherche la distance la plus petite
            if (smallestDistance > currentDistance) // si la distance actuelle est plus petite que la precedente
            {

                smallestDistance = currentDistance; // la distance actuelle devient la distance la plus courte
                closestObject = hitCollider.transform.gameObject; // le resultat final = l'objet actuel
            }

        } // apres avoir compare toutes les distances de tout les gameobject trouves on se retrouve avec la distance la plus petite

        if (closestObject != gameObject) // si le resultat n'est pas le gameobject etabli par defaut
        {
            targetDistance = Mathf.Sqrt(smallestDistance);
            return (closestObject); // le resultat de la fonction = closestObject;
        }
        else
        {
            targetDistance = 0;
            return (null); // le resultat de la fonction est rien
        }
    }

    void JumpFromThisToThis(Vector3 startPosition, Vector3 endPosition, float height, float speed)
    {

        startJumpPos = startPosition;
        endJumpPos = endPosition;
        currentSpeed = speed;
        jumpHeight = height;
        isJumping = true;
        currentJumpSpeed = 5;
        /*float totalHeightJump = startPosition.y - endPosition.y + heightOverEndPos;
        float startDistanceFromEnd = (endPosition - startPosition).magnitude;
        transform.position -= new Vector3(0, 1, 0) * Time.deltaTime;
        //transform.position += new Vector3(0, jumpSpeed, 0) * Time.deltaTime;
        //jumpGravity += (1 / totalHeightJump) - 1;
        */
    }

    public void NewProjectileStats(float damage, float speed, float SizeMultiplier)
    {
        currentDamage = damage;
        currentSpeed = speed;
        currentSizeMultiplier = SizeMultiplier;
    }

    IEnumerator SecondTimer()
    {
        hungerPercentage -= 1;
        transform.localScale = ((hungerPercentage / 20) + 0.2f) * new Vector3(size, size, size); // on scale avec la faim
        if (hungerPercentage <= 0)
        {
            StartCoroutine("TimedDestructionInitiation");
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("SecondTimer");
    }

    void InitiateDestruction() // detruire
    {
        
        Destroy(gameObject);
    }

    IEnumerator TimedDestructionInitiation() // detruire apres 0.1 seconde
    {
        if (destroyBool && hasProjectileLimit)
        {
            instantiator.GetComponent<UniversalWeaponScript>().currentAmountOfProjectile--;
            destroyBool = false;
        }
        yield return new WaitForSeconds(0.1f);
        InitiateDestruction();
    }
    private void OnDrawGizmos()
    {
        // Set the color
        //Gizmos.color = new Color(1f, 0f, 0f); // Red 

        // Draw the sphere
        //Gizmos.DrawSphere(transform.position, range);

        // Draw wire sphere outline
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * 0.5f), transform.localScale.x * 1.2f);
    }
}
