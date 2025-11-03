using UnityEngine;
using System.Collections.Generic;

public class PlayerShooter : MonoBehaviour
{
    [Header("Weapons")]
    public List<WeaponData> equippedWeapons = new List<WeaponData>();
    public int maxWeapons = 6;

    [Header("Targeting")]
    public LayerMask enemyLayer;
    public LayerMask blockingLayer;
    public float targetingUpdateRate = 0.1f; // Optimisation

    [Header("Visual Feedback")]
    public bool showTargetingDebug = false;

    private Dictionary<WeaponData, float> weaponTimers = new Dictionary<WeaponData, float>();
    private List<GameObject> cachedEnemies = new List<GameObject>();
    private float targetCacheTimer = 0f;

    void Start()
    {
        // Initialiser les timers pour chaque arme
        foreach (var weapon in equippedWeapons)
        {
            if (weapon != null && !weaponTimers.ContainsKey(weapon))
                weaponTimers[weapon] = 0f;
        }
    }

    void Update()
    {
        // Mise à jour du cache d'ennemis pour optimisation
        targetCacheTimer += Time.deltaTime;
        if (targetCacheTimer >= targetingUpdateRate)
        {
            UpdateEnemyCache();
            targetCacheTimer = 0f;
        }

        // Chaque arme tire indépendamment
        foreach (var weapon in equippedWeapons)
        {
            if (weapon == null || weapon.projectilePrefab == null) continue;

            if (!weaponTimers.ContainsKey(weapon))
                weaponTimers[weapon] = 0f;

            weaponTimers[weapon] += Time.deltaTime;
            float shootInterval = 1f / weapon.attackSpeed;

            if (weaponTimers[weapon] >= shootInterval)
            {
                GameObject target = FindBestTarget(weapon);
                if (target != null)
                {
                    ShootAt(weapon, target);
                }
                weaponTimers[weapon] = 0f;
            }
        }
    }

    void UpdateEnemyCache()
    {
        cachedEnemies.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Filtrer les ennemis morts
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null && !health.IsDead())
                {
                    cachedEnemies.Add(enemy);
                }
            }
        }
    }

    GameObject FindBestTarget(WeaponData weapon)
    {
        if (cachedEnemies.Count == 0 || weapon == null || weapon.firePoint == null)
            return null;

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in cachedEnemies)
        {
            if (enemy == null) continue;

            Transform targetPoint = enemy.transform.Find("TargetPoint");
            Vector3 targetPos = (targetPoint != null) ? targetPoint.position : enemy.transform.position + Vector3.up * 0.5f;

            float distance = Vector3.Distance(weapon.firePoint.position, targetPos);

            if (distance > weapon.range) continue;

            Vector3 direction = (targetPos - weapon.firePoint.position).normalized;

            if (Physics.Raycast(weapon.firePoint.position, direction, out RaycastHit hit, distance, blockingLayer | enemyLayer))
            {
                if (hit.collider != null && (hit.collider.gameObject == enemy || hit.collider.transform.root.gameObject == enemy))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        return closestEnemy;
    }


    void ShootAt(WeaponData weapon, GameObject target)
    {
        if (weapon.firePoint == null)
        {
            Debug.LogWarning($"Weapon {weapon.weaponName} has no fire point!");
            return;
        }

        // Position de ciblage
        Transform targetPoint = target.transform.Find("TargetPoint");
        Vector3 targetPos = targetPoint ? targetPoint.position : target.transform.position + Vector3.up * 0.5f;

        // Instantier le projectile
        GameObject proj = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);

        var projectile = proj.GetComponent<ProjectileController>();
        if (projectile != null)
        {
            projectile.blockingLayer = blockingLayer;
            projectile.Initialize(weapon, targetPos);
        }
        else
        {
            Debug.LogError($"Projectile prefab for {weapon.weaponName} missing ProjectileController!");
            Destroy(proj);
        }

        // FX de tir (optionnel)
       /* if (weapon.onFireVFX != null)
        {
            Instantiate(weapon.onFireVFX, weapon.firePoint.position, weapon.firePoint.rotation);
        }

        if (weapon.fireSFX != null)
        {
            AudioSource.PlayClipAtPoint(weapon.fireSFX, weapon.firePoint.position, 0.5f);
        }*/
    }

    public bool EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return false;

        if (equippedWeapons.Count >= maxWeapons)
        {
            Debug.Log("Max weapons reached!");
            return false;
        }

        equippedWeapons.Add(newWeapon);
        weaponTimers[newWeapon] = 0f;

        Debug.Log($"Equipped: {newWeapon.weaponName} ({equippedWeapons.Count}/{maxWeapons})");
        return true;
    }

    public void RemoveWeapon(WeaponData weapon)
    {
        if (equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Remove(weapon);
            weaponTimers.Remove(weapon);
        }
    }

    public void RemoveAllWeapons()
    {
        equippedWeapons.Clear();
        weaponTimers.Clear();
    }

    public int GetWeaponCount() => equippedWeapons.Count;
    public bool HasWeapon(WeaponData weapon) => equippedWeapons.Contains(weapon);

    void OnDrawGizmos()
    {
        if (!showTargetingDebug || equippedWeapons.Count == 0) return;

        foreach (var enemy in cachedEnemies)
        {
            if (enemy == null) continue;

            foreach (var weapon in equippedWeapons)
            {
                if (weapon == null || weapon.firePoint == null) continue;

                GameObject target = FindBestTarget(weapon);
                if (target == null) continue;
                if (target != enemy) continue;

                Transform tp = enemy.transform.Find("TargetPoint");
                Vector3 targetPos = tp != null ? tp.position : enemy.transform.position + Vector3.up * 0.5f;
                Gizmos.DrawLine(weapon.firePoint.position, targetPos);
            }
        }

    }
}