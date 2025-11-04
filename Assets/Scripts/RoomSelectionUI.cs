using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoomSelectionUI : MonoBehaviour
{
    public Button[] roomButtons;
    public RoomManager roomManager;
    public ScreenFader screenfader;

    //public InputActionAsset inputActions;

    void OnEnable()
    {
        List<RoomData> options = roomManager.GetRandomRooms(3);

        for (int i = 0; i < roomButtons.Length; i++)
        {
            int index = i;
            roomButtons[i].GetComponentInChildren<TMPro.TMP_Text>().text = options[i].roomPrefab.name;
            roomButtons[i].onClick.RemoveAllListeners();
            roomButtons[i].onClick.AddListener(() =>
            {
                ScreenFader fader = screenfader;   
                void OnBlack()
                {                 
                    Cursor.lockState = CursorLockMode.Locked;
                    roomManager.SpawnRoom(options[index].roomPrefab);
                    gameObject.SetActive(false);                 
                    fader.OnFadeToBlack -= OnBlack;
                }              
                fader.OnFadeToBlack += OnBlack;
                // inputActions.FindActionMap("Player").Enable();
                Time.timeScale = 1f;
                fader.StartFade();
            });
        }

    }
}
