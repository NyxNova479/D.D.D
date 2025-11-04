using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    [Header("Ground Detection")]
    public float groundCheckDistance = 2f;
    public LayerMask groundLayer;

    private Transform player;
    private float attackTimer;
    private Rigidbody rb;
    private bool isDead = false;

    public EnemyHealth health;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Configuration du Rigidbody
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                           RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Trouver le joueur
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Récupérer le composant de santé
        if (health == null)
            health = GetComponent<EnemyHealth>();

        // S'abonner à l'événement de mort
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        // Se désabonner pour éviter les erreurs
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        isDead = true;
        // Arrêter tout mouvement
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        // Vérifications de sécurité
        if (player == null || health == null || rb == null || isDead)
            return;

        // Protection anti-chute dans le sol
        PreventFallingThroughGround();

        // Calculer la direction vers le joueur (sur le plan XZ)
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        // Déplacement ou attaque
        if (distance > attackRange)
        {
            MoveTowards(direction);
        }
        else
        {
            // Arrêter le mouvement en attaque
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // Rotation vers le joueur
        RotateTowards(direction);
    }

    void Update()
    {
        // Timer d'attaque et logique d'attaque
        if (player == null || health == null || isDead)
            return;

        attackTimer += Time.deltaTime;

        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        if (distance <= attackRange)
        {
            AttackPlayer();
        }
    }

    void MoveTowards(Vector3 direction)
    {
        // Déplacement horizontal uniquement (pas de modification de Y)
        Vector3 moveVelocity = new Vector3(
            direction.x * moveSpeed,
            rb.linearVelocity.y, // Garder la vélocité verticale pour la gravité
            direction.z * moveSpeed
        );

        rb.linearVelocity = moveVelocity;
    }

    void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            // Garder seulement la rotation Y
            transform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
        }
    }

    void AttackPlayer()
    {
        if (attackTimer >= attackCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Mathf.Round(damage));
                
            }

            attackTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiser la portée d'attaque dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void PreventFallingThroughGround()
    {
        // Raycast vers le bas pour détecter le sol
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            float groundY = hit.point.y;

            // Si l'ennemi est en dessous du sol, le remonter
            if (transform.position.y < groundY)
            {
                Vector3 correctedPos = transform.position;
                correctedPos.y = groundY;
                transform.position = correctedPos;

                // Réinitialiser la vélocité verticale
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
    }
}