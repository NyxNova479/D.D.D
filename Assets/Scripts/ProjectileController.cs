using UnityEngine;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour
{
    [Header("Stats")]
    public float lifetime = 5f;

    [Header("Comportements")]
    public bool canBounce;
    public int maxBounces;
    public float bounceRange;
    public int pierceCount;

    public bool canExplode;
    public float explosionRadius;
    public float explosionDamageMultiplier;

    public bool applyKnockback;
    public float knockbackForce;

    [Header("Visuals / FX")]
    public GameObject onHitVFX;
    public GameObject onExplosionVFX;
    public AudioClip hitSFX;

    [HideInInspector] public LayerMask blockingLayer;

    private Vector3 velocity;
    private float damage;
    private float timeAlive;
    private List<GameObject> hitEnemies = new List<GameObject>();
    private int bounceCount = 0;

    public void Initialize(WeaponData data, Vector3 targetPos)
    {
        damage = data.damage;
        transform.localScale *= data.projectileSize;
        velocity = (targetPos - transform.position).normalized * data.projectileSpeed;

        canBounce = data.canBounce;
        maxBounces = data.maxBounces;
        bounceRange = data.bounceRange;
        canExplode = data.canExplode;
        explosionRadius = data.explosionRadius;
        explosionDamageMultiplier = data.explosionDamageMultiplier;
        pierceCount = data.pierceCount;
        applyKnockback = data.applyKnockback;
        knockbackForce = data.knockbackForce;

        onHitVFX = data.onHitVFX;
        onExplosionVFX = data.onExplosionVFX;
        hitSFX = data.hitSFX;
    }

    void Update()
    {
        float moveStep = velocity.magnitude * Time.deltaTime;
        Ray ray = new Ray(transform.position, velocity.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, moveStep);

        // Tri par distance
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            // Si c'est un ennemi
            if (hit.collider.CompareTag("Enemy") && !hitEnemies.Contains(hit.collider.gameObject))
            {
                HandleHit(hit.collider);
                break;
            }
            // Si sol ou props
            else if (((1 << hit.collider.gameObject.layer) & blockingLayer) != 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;

        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
            Destroy(gameObject);
    }

    void HandleHit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);

        // Dégâts
        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null) enemyHealth.TakeDamage(damage);

        // Knockback
        if (applyKnockback)
        {
            var kb = other.GetComponent<EnemyKnockback>();
            if (kb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                kb.ApplyKnockback(dir, knockbackForce);
            }
        }

        // FX
        if (onHitVFX) Instantiate(onHitVFX, transform.position, Quaternion.identity);
        if (hitSFX) AudioSource.PlayClipAtPoint(hitSFX, transform.position);

        // Explosion
        if (canExplode) Explode();

        // Bounce
        bool bounced = false;
        if (canBounce && bounceCount < maxBounces)
        {
            GameObject next = FindNextEnemy(other.gameObject);
            if (next != null)
            {
                velocity = (next.transform.position - transform.position).normalized * velocity.magnitude;
                bounceCount++;
                bounced = true;
            }
        }

        if (!bounced)
        {
            if (hitEnemies.Count <= pierceCount) return;
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        if (onExplosionVFX) Instantiate(onExplosionVFX, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null) enemy.TakeDamage(damage * explosionDamageMultiplier);
            }
        }

        Destroy(gameObject);
    }

    GameObject FindNextEnemy(GameObject current)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (e == current || hitEnemies.Contains(e)) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < closestDist && d <= bounceRange)
            {
                closestDist = d;
                closest = e;
            }
        }

        return closest;
    }
}
