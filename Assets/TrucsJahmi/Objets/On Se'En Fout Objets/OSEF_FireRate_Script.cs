using UnityEngine;

public class OSEF_FireRate_Script : MonoBehaviour
{
    public float multiplicativeFireRateBonus;
    public bool hasBeenApplied;
    public GameObject player;
    void Start()
    {
        player = GetComponentInParent<ItemHandlerScript>().player;
        ApplyItemEffect();
    }

    void ApplyItemEffect()
    {
        if (!hasBeenApplied)
        {
            var playerStatScript = player.GetComponent<PlayerStatsScript>();
            playerStatScript.currentAttackRateMultiplier -= multiplicativeFireRateBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
