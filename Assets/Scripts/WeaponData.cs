using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string weaponName = "Default Weapon";
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Stats de base")]
    public float damage = 10f;
    public float projectileSize = 1f;
    public float projectileSpeed = 20f;
    public float attackSpeed = 1f; 
    public float range = 15f;

    [Header("Comportements spéciaux")]
    public bool canBounce = false;
    public int maxBounces = 2;
    public float bounceRange = 8f;

    public bool canExplode = false;
    public float explosionRadius = 3f;
    public float explosionDamageMultiplier = 0.75f;

    public int pierceCount = 0;

    public bool applyKnockback = false;
    public float knockbackForce = 10f;

    [Header("Visuals / FX")]
    public GameObject onHitVFX;
    public GameObject onExplosionVFX;
    public AudioClip hitSFX;

    public void UpgradeAttackSpeed(float value) => attackSpeed += value;
    public void UpgradeDamage(float value) => damage += value;
    public void UpgradeProjectileSize(float value) => projectileSize += value;
    public void UpgradeProjectileSpeed(float value) => projectileSpeed += value;
}
