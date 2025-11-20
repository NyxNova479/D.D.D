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
    public InputActionAsset inputActions;
    public PauseMenu psscript;

    void OnEnable()
    {
        // Empêche certaines actions pendant la sélection
        psscript.PourLesPortes();

        // Récupère 3 salles aléatoires
        List<RoomData> options = roomManager.GetRandomRooms(3);

        for (int i = 0; i < roomButtons.Length; i++)
        {
            int index = i;

            // affiche le nom de la salle dans le bouton
            TMP_Text buttonText = roomButtons[i].GetComponentInChildren<TMP_Text>();
            buttonText.text = options[i].roomName; // Pour avoir le nom de la salle

            // Nettoie les anciens listeners
            roomButtons[i].onClick.RemoveAllListeners();

            // Ajoute le nouveau listener
            roomButtons[i].onClick.AddListener(() =>
            {
                ScreenFader fader = screenfader;

                void OnBlack()
                {
                    // Appelle la version mise à jour de SpawnRoom avec RoomData complet
                    roomManager.SpawnRoom(options[index].roomPrefab, options[index]);

                    // Cache l'UI de sélection
                    gameObject.SetActive(false);

                    // Se désabonne pour éviter les accumulations
                    fader.OnFadeToBlack -= OnBlack;
                }

                void OnComplete()
                {
                    //Debug.Log("Le fondu est terminé !");
                    // Ici tu peux remettre les contrôles, etc.
                    // Exemple :
                    psscript.Resume();

                    // Se désabonne aussi
                    fader.OnFadeComplete -= OnComplete;
                }

                // Abonnement temporaire à l'événement de fondu
                fader.OnFadeToBlack += OnBlack;
                fader.OnFadeComplete += OnComplete;

                // Reprend le jeu et démarre la transition
                fader.StartFade();
               // fader.OnFadeComplete;
                
            });
        }
    }
}
