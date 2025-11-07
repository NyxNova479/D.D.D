using UnityEngine;

public class BOBMNUCLAIRscripttest : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        //UpdateGui();

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        Debug.Log("Le joueur est mort.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
