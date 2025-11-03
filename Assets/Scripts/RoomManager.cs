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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

    public void SpawnRoom(GameObject roomPrefab)
    {
        if (currentRoom != null)
            Destroy(currentRoom);

        currentRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        currentRoom.SetActive(true);

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
    }

    public void ResetRoomCount()
    {
        currentRoomIndex = 0;
        UpdateRoomText();
        //Debug.Log("Compteur de salles réinitialisé à 0.");
    }

    void UpdateRoomText()
    {
        if (roomCounterText != null)
        {
            roomCounterText.text = $"Room : {currentRoomIndex}";
        }
    }
}
