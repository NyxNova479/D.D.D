using UnityEngine;

public class OSEF_Size_Script : MonoBehaviour
{
    public float multiplicativeSizeBonus;
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
            playerStatScript.currentAttackSizeMultiplier += multiplicativeSizeBonus;
            playerStatScript.UpdateStats();
            hasBeenApplied = true;
        }
    }
}
