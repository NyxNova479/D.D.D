using UnityEngine;

public class OSEF_PV_Script : MonoBehaviour
{
    public float multiplicativeHPBonus;
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
            playerStatScript.healthPointMultiplier += multiplicativeHPBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
