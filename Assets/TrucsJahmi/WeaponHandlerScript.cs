using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeaponHandlerScript : MonoBehaviour
{
    public List<GameObject> weaponsList;

    public void ResetWeapons()
    {
        for (int i = 0; i < weaponsList.Count; i++)
        {
            weaponsList[i].GetComponent<UniversalWeaponScript>().currentAmountOfProjectile = 0;
        }
    }
}
