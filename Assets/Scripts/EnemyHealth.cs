using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Drop / FX")]
    public GameObject dropPrefab;
    public GameObject deathVFX;
    public float vfxDestroyDelay = 2f;

    [Header("Drop Settings")]
    public Vector3 dropOffset = Vector3.up * 0.5f;
    public bool randomizeDropRotation = true;

    private Rigidbody rb;
    private bool isDead = false;

    // Event pour notifier l'AI de la mort
    public event Action OnDeath;
    public event Action<float> OnDamageTaken;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Déclencher l'événement de dégât
        OnDamageTaken?.Invoke(damage);

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notifier l'AI
        OnDeath?.Invoke();

        // Spawn l'objet de drop
        if (dropPrefab != null)
        {
            Quaternion dropRotation = randomizeDropRotation
                ? Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0)
                : Quaternion.identity;

            Instantiate(dropPrefab, transform.position + dropOffset, dropRotation);
        }

        // FX de mort
        if (deathVFX != null)
        {
            GameObject vfx = Instantiate(deathVFX, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDestroyDelay);
        }

        // Détruire l'ennemi
        Destroy(gameObject);
    }

    // Getters pour l'état de santé
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
}