using UnityEngine;
using System.Collections;

public class UniversalEnemyScript : MonoBehaviour
{
    [Header("Objets generaux")]
    public GameObject target;
    public GameObject instantiator;
    public GameObject model3D;
    private Enemy3DModelScript scriptModel3D;

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
    private bool canAttack;

    [Header("Mouvements")]
    public float movementSpeed;
    public bool isWalking;
    public Rigidbody rb;
    public float fallTimer;

    [Header("Capacites speciales")]
    public float minEnemySizeMultiplier;
    public float maxEnemySizeMultiplier;
    public float enemySizeMultiplier;

    public bool canMerge;
    public float maxMergeSize;

    [Header("Afficher degats subits")]
    public GameObject billboard;
    public Color colorOfTakenDamage;

    void Start()
    {
        StartCoroutine("SecondTimer");
        scriptModel3D = model3D.GetComponent<Enemy3DModelScript>();
        rb.useGravity = true;
        isWalking = true;

        // Taille aléatoire
        if (maxEnemySizeMultiplier != minEnemySizeMultiplier)
            enemySizeMultiplier = Random.Range(minEnemySizeMultiplier, maxEnemySizeMultiplier);

        if (enemySizeMultiplier != 1)
            transform.localScale *= enemySizeMultiplier;

        // HP initiaux
        baseHealthPoint *= transform.localScale.x;
        currentHealthPoint = baseHealthPoint;
        scaledBaseHealthPoint = baseHealthPoint;
        oldScaledBaseHealthPoint = scaledBaseHealthPoint;

        currentHealthPercentage = 100;
        oldHealthPercentage = 100;
        oldHP = currentHealthPoint;

        ScaleStatsWithSize();
    }

    void Update() // plus sympa a voir le update()
    {
        HandleAttackTimer();
        HandleDamageFeedback();
        HandleMovement();
        HandleDeath();
    }

    // -----------------------------
    //     SYSTEMES PRINCIPAUX
    // -----------------------------

    void HandleAttackTimer()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                attackTimer = 0;
                canAttack = true;
            }
        }
    }

    void HandleDamageFeedback()
    {
        if (currentHealthPoint >= oldHP) return;

        currentHealthPercentage = currentHealthPoint * 100 / scaledBaseHealthPoint;

        // Affichage des dégâts
        var inst = Instantiate(billboard, transform.position, Quaternion.identity);
        var instScript = inst.GetComponent<BillboardSpriteScript>();
        instScript.billboardText.text = ((int)(oldHP - currentHealthPoint)).ToString();
        instScript.instantiatorSizeOffset = transform.localScale.z / 2;
        instScript.color = colorOfTakenDamage;

        scriptModel3D.TookDamage(oldHealthPercentage - currentHealthPercentage);

        oldHealthPercentage = currentHealthPercentage;
        oldHP = currentHealthPoint;
    }

    void HandleMovement()
    {
        var targetDir = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));

        if (isWalking)
        {
            Walk();
            GroundDetection();
        }
        else
        {
            // grimpe
            rb.useGravity = false;
            transform.position += transform.up * movementSpeed * Time.deltaTime;
            fallTimer = 1;
        }

        if (fallTimer > 0)
            fallTimer -= 10 * Time.deltaTime;
        else
            rb.useGravity = true;
    }

    void HandleDeath()
    {
        if (currentHealthPoint <= 0 || transform.position.y < -5)
            Destruction();
    }

    // -----------------------------
    //     COLLISIONS
    // -----------------------------

    void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if (layer == climbableLayer)
        {
            var point = collision.GetContact(0).point;
            transform.position -= new Vector3(point.x - transform.position.x, 0, point.z - transform.position.z) * movementSpeed * Time.deltaTime;
            isWalking = false;
        }

        if (canMerge && layer == enemyLayer && transform.localScale.x < maxMergeSize)
        {
            var other = collision.gameObject.transform;
            if (other.localScale.x < transform.localScale.x)
            {
                if (transform.localScale.x < 1)
                    transform.localScale += other.localScale / 2;
                else
                    transform.localScale += other.localScale / transform.localScale.x;

                collision.gameObject.GetComponent<UniversalEnemyScript>().Destruction();
                ScaleStatsWithSize();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == climbableLayer)
            isWalking = true;
    }

    // -----------------------------
    //     FONCTIONS UTILITAIRES
    // -----------------------------

    void Walk()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    float CalculateDistanceFromTarget()
    {
        return (target.transform.position - transform.position).sqrMagnitude;
    }

    void GroundDetection()
    {
        if (Physics.Raycast(transform.position, -transform.up, 0.65f))
        {
            fallTimer = 1;
            rb.useGravity = false;
        }
    }

    void ScaleStatsWithSize()
    {
        scaledBaseHealthPoint = baseHealthPoint * transform.localScale.x * transform.localScale.x;

        currentHealthPoint = (currentHealthPoint * scaledBaseHealthPoint) / oldScaledBaseHealthPoint;
        oldScaledBaseHealthPoint = scaledBaseHealthPoint;

        scaledBaseDamage = baseDamage * transform.localScale.x * transform.localScale.x;
        damageToDeal = scaledBaseDamage;

        oldHP = currentHealthPoint;

        attackReach = transform.localScale.x * 0.75f;
    }

    public void Destruction()
    {
        instantiator.GetComponent<EnemySpawnerScript>().enemiesExisting.Remove(gameObject);
        Destroy(gameObject);
    }

    void Attack()
    {
        if (!canAttack) return;

        if (damageToDeal <= 0)
            damageToDeal = 1;

        target.GetComponent<PlayerStatsScript>().TakeDamage(damageToDeal);
        canAttack = false;
    }

    IEnumerator SecondTimer()
    {
        if (CalculateDistanceFromTarget() <= attackReach * attackReach)
            Attack();

        yield return new WaitForSeconds(0.1f);
        StartCoroutine("SecondTimer");
    }
}
