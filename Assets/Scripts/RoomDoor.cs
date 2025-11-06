using UnityEngine;
using UnityEngine.InputSystem;

public class RoomDoor : MonoBehaviour
{
    public GameObject uiCanvas;
    public GameObject uiCanvasE;
    private bool playerInRange;

    public InputActionAsset inputActions;


    //public InputActionAsset inputActions;

    void Update()
    {
        if (playerInRange)
        {
            uiCanvasE.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) { uiCanvas.SetActive(!uiCanvas.activeSelf); }
            
            
        }
        else { uiCanvas.SetActive(false); uiCanvasE.SetActive(false); }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
