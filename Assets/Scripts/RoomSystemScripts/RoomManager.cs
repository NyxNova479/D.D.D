using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public Transform player;
    public List<RoomData> allRooms;
    public GameObject currentRoom;

    public TMP_Text roomCounterText;
    public int currentRoomIndex { get; private set; } = 0;

    public RoomData currentRoomData { get; private set; } // Salle actuelle

    [Header("Starting room")]
    public GameObject startingRoom; 
    public RoomData startingRoomData;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {

        if (startingRoom != null && startingRoomData != null)
        {
            currentRoom = startingRoom;
            currentRoomData = startingRoomData;
            currentRoomIndex = 1; // On commence à 1 car c'est la première salle
            UpdateRoomText();
        }
    }

    public List<RoomData> GetRandomRooms(int count)
    {
        List<RoomData> selected = new List<RoomData>();
        List<RoomData> copy = new List<RoomData>(allRooms);

        while (selected.Count < count && copy.Count > 0)
        {
            float totalWeight = copy.Sum(r => r.spawnProbability);
            float rand = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (RoomData room in copy)
            {
                cumulative += room.spawnProbability;
                if (rand <= cumulative)
                {
                    selected.Add(room);
                    copy.Remove(room);
                    break;
                }
            }
        }

        return selected;
    }

    public void SpawnRoom(GameObject roomPrefab, RoomData roomData)
    {
        // Détruit l'ancienne salle SAUF si c'est la première fois et que c'est la salle de départ
        if (currentRoom != null && currentRoom != startingRoom)
        {
            Destroy(currentRoom);
        }
        else if (currentRoom == startingRoom)
        {
            // Désactive la salle de départ
            startingRoom.SetActive(false);
        }

        currentRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        currentRoom.SetActive(true);

        currentRoomData = roomData;

        Transform entry = currentRoom.transform.Find("EntryPoint");

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.position = entry.position;
            controller.enabled = true;
        }
        else
        {
            player.position = entry.position;
        }

        currentRoomIndex++;
        UpdateRoomText();

        // jahmi
        UpdatePlayer();
    }

    public void ResetRoomCount()
    {
        currentRoomIndex = 0;

        if (startingRoom != null)
        {
            if (currentRoom != null && currentRoom != startingRoom)
            {
                Destroy(currentRoom);
            }

            startingRoom.SetActive(true);
            currentRoom = startingRoom;
            currentRoomData = startingRoomData;
            currentRoomIndex = 1;
        }

        UpdateRoomText();
    }

    void UpdateRoomText()
    {
        if (roomCounterText != null)
        {
            string roomName = currentRoomData != null ? currentRoomData.roomName : "Unknown";
            roomCounterText.text = $"Room {currentRoomIndex} : {roomName}";
        }
    }

    //le truc de jahmi
    void UpdatePlayer()
    {
        var playerWeaponHandlerScript = player.GetComponentInChildren<WeaponHandlerScript>();
        if (playerWeaponHandlerScript != null)
        {
            playerWeaponHandlerScript.ResetWeapons();
        }
    }
}