using UnityEngine;

public class OSEF_Speed_Script : MonoBehaviour
{
    public float multiplicativeSpeedBonus;
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
            playerStatScript.movementSpeedMultiplier += multiplicativeSpeedBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
