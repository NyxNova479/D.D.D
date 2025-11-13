using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomDoor : MonoBehaviour
{
    public GameObject uiCanvas;
    public GameObject uiCanvasE;
    private bool playerInRange;

    public PauseMenu psscript;

    public InputActionAsset inputActions;

    private bool d = false ;

    //public InputActionAsset inputActions;


    private void Start()
    {
        d = false;
    }

    void Update()
    {
        // caca();

        if (playerInRange)
        {
            uiCanvasE.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) { uiCanvas.SetActive(true); uiCanvasE.SetActive(false); psscript.DoorUi(); d = true; }


        }
        else { uiCanvasE.SetActive(false);}

        if (uiCanvas.activeSelf == true){uiCanvasE.SetActive(false);}   
    }

    void caca()
    {
        if (uiCanvas.activeSelf == true)
        {
            d = false;
        }

        else if (d == true) 
        { 
            d = false;
            psscript.Resume();
        }

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
