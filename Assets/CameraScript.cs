using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject cameraCore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            //if (hitCollider.transform.gameObject.layer == 8) // si le collider est sur la couche "Enemy"
            transform.position += transform.forward;
        }
        if (hitColliders.Length == 0)
        {
            transform.position = -transform.forward * 10;
        }
    }
}
