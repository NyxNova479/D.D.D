using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PlayerStatsScript : MonoBehaviour
{
    [Header("Health Points")]
    public float baseHealthPoint;
    public float currentMaxHealthPoint;
    public float currentHealthPoint;
    public float healthPointMultiplier;
    [Header("Passive Healing")]
    public float basePassiveHealing;
    public float currentPassiveHealing;
    public float passiveHealingMultiplier;
    [Header("Attack Damage")]
    public float baseAttackDamageMultiplier;
    public float currentAttackDamageMultiplier;
    [Header("Attack Rate")]
    public float baseAttackRateMultiplier;
    public float currentAttackRateMultiplier;
    [Header("Attack Size")]
    public float baseAttackSizeMultiplier;
    public float currentAttackSizeMultiplier;
    [Header("Movement Speed")]
    public float baseMovementSpeed;
    public float currentMovementSpeed;
    public float movementSpeedMultiplier;

    float oldCurrentMovementSpeed;
    [Header("Jump Height")]
    public float baseJumpHeight;
    public float currentJumpHeight;
    public float jumpHeightMultiplier;
    [Header("General")]
    public PauseMenu pausemenu;
    public GameObject weaponHandler;
    [Header("UI")]
    public GameObject DeathScreenCanvas;
    public TMP_Text healthUi;
    public Image healthFill;
    void Start()
    {
        StartCoroutine("SecondTimer");
        StartingSetUpCurrentStats();
        UpdateHealthUI();
        UpdateStats();
    }

    void Update()
    {
        if (oldCurrentMovementSpeed != currentMovementSpeed) // mettre a jour la vitesse si il y a un changement
        {
            oldCurrentMovementSpeed = currentMovementSpeed;
            //gameObject.GetComponent<ThirdPersonController>().MoveSpeed = currentMovementSpeed;
            gameObject.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = currentMovementSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.y < -5)
        {
            Die();
        }
    }

    void StartingSetUpCurrentStats()
    {
        //les stats basiques
        currentMaxHealthPoint = baseHealthPoint;
        currentHealthPoint = currentMaxHealthPoint;
        currentJumpHeight = baseJumpHeight;
        currentMovementSpeed = baseMovementSpeed;
        //les multiplicateurs basiques
        currentAttackDamageMultiplier = baseAttackDamageMultiplier;
        currentAttackRateMultiplier = baseAttackRateMultiplier;
        currentAttackSizeMultiplier = baseAttackSizeMultiplier;
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoint -= damage;
        if (currentHealthPoint < 0) currentHealthPoint = 0;
        UpdateHealthUI();

        if (currentHealthPoint <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (currentHealthPoint < currentMaxHealthPoint)
        {
            if (currentHealthPoint + amount > currentMaxHealthPoint)
            {
                currentHealthPoint += currentMaxHealthPoint - currentHealthPoint;
            }
            else
            {
                currentHealthPoint += amount;
            }
        }
        UpdateHealthUI();
    }

    void PassiveHealing()
    {
        Heal(currentPassiveHealing);
    }

    public void UpdateStats()
    {
        currentMaxHealthPoint = baseHealthPoint * healthPointMultiplier;
        currentPassiveHealing = basePassiveHealing * passiveHealingMultiplier;
        UpdateHealthUI();
        UpdateWeaponsStats();
    }
    void UpdateWeaponsStats()
    {
        weaponHandler.GetComponent<WeaponHandlerScript>().UpdateAllWeaponsStats(currentAttackDamageMultiplier, currentAttackRateMultiplier, currentMovementSpeed);
    }
        void UpdateHealthUI()
    {
        if (healthFill)
            healthFill.fillAmount = currentHealthPoint / currentMaxHealthPoint;

        if (healthUi)
        {
            healthUi.text = (Mathf.Round(currentHealthPoint) + "/" + currentMaxHealthPoint);
        }
    }

    void Die()
    {
        DeathScreenCanvas.SetActive(true);
        pausemenu.PauseGame();
        Debug.Log("Player est mort !");

    }

    IEnumerator SecondTimer()
    {
        yield return new WaitForSeconds(1);
        PassiveHealing();
        StartCoroutine("SecondTimer");
    }
}
