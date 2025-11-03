using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    [Header("Visual")]
    public GameObject visualModel;
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    [Header("Pickup")]
    public float pickupRadius = 2f;
    public GameObject pickupVFX;
    public AudioClip pickupSFX;

    [Header("Auto Pickup")]
    public bool autoPickup = false;
    public float autoPickupDelay = 0.5f;

    private Vector3 startPosition;
    private float bobTimer;
    private bool isPickedUp = false;
    private float autoPickupTimer = 0f;

    void Start()
    {
        startPosition = transform.position;

        // Setup du collider
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = pickupRadius;
    }

    void Update()
    {
        if (isPickedUp) return;

        // Animation de rotation
        if (visualModel != null)
        {
            visualModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Animation de bobbing (haut/bas)
        bobTimer += Time.deltaTime * bobSpeed;
        float newY = startPosition.y + Mathf.Sin(bobTimer) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerStay(Collider other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            if (autoPickup)
            {
                autoPickupTimer += Time.deltaTime;
                if (autoPickupTimer >= autoPickupDelay)
                {
                    TryPickup(other.gameObject);
                }
            }
            else
            {
                // Pour input manuel si tu veux (Input.GetKeyDown(KeyCode.E))
                TryPickup(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            autoPickupTimer = 0f;
        }
    }

    void TryPickup(GameObject player)
    {
        PlayerShooter shooter = player.GetComponent<PlayerShooter>();

        if (shooter != null && weaponData != null)
        {
            if (shooter.EquipWeapon(weaponData))
            {
                OnPickedUp();
            }
            else
            {
                Debug.Log("Cannot equip weapon: max weapons reached");
            }
        }
    }

    void OnPickedUp()
    {
        isPickedUp = true;

        // VFX
        if (pickupVFX != null)
        {
            Instantiate(pickupVFX, transform.position, Quaternion.identity);
        }

        // SFX
        if (pickupSFX != null)
        {
            AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
        }

        // Détruire l'objet
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}