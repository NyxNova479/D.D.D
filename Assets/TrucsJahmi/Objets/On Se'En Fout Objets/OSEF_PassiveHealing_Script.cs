using UnityEngine;

public class OSEF_PassiveHealing_Script : MonoBehaviour
{
    public float multiplicativePassiveHealingBonus;
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
            playerStatScript.passiveHealingMultiplier += multiplicativePassiveHealingBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
