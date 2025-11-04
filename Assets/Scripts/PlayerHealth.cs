using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject DeathScreenCanvas;
    public PauseMenu pausemenu;

    public TMP_Text healthUi;

    [Header("UI")]
    public Image healthFill;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthFill)
            healthFill.fillAmount = currentHealth / maxHealth;

        if (healthUi)
        {
            healthUi.text = (Mathf.Round(currentHealth)+"/"+ maxHealth);
        }
    }

    void Die()
    {
        DeathScreenCanvas.SetActive(true);
        pausemenu.PauseGame();
        Debug.Log("Player est mort !");
        
    }
}
