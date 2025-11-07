using UnityEngine;

public class MenuCameraLook : MonoBehaviour
{
    [Header("Paramètres")]
    public float rotationAmount = 10f;
    public float smoothSpeed = 5f;

    private Vector3 _initialRotation;

    void Start()
    {
        _initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Position de la souris en pourcentage de l’écran
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        // Calcul des rotations cibles
        float rotX = -mouseY * rotationAmount;
        float rotY = mouseX * rotationAmount;

        // Création de la rotation finale
        Quaternion targetRotation = Quaternion.Euler(
            _initialRotation.x + rotX,
            _initialRotation.y + rotY,
            _initialRotation.z
        );

        // Lerp (transition FLUIDE SMOOOOTH OPERATORR
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
