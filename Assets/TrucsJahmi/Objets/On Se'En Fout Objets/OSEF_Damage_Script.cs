using UnityEngine;

public class OSEF_Damage_Script : MonoBehaviour
{
    public float multiplicativeDamageBonus;
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
            playerStatScript.currentAttackDamageMultiplier += multiplicativeDamageBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
