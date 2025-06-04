using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text listPlayers;
    [SerializeField] private Button startGame;

    [PunRPC]
    public void UpdateList()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);

        if (PhotonNetwork.PlayerList.Length > 2)
        {
            NetworkController.Instance.LeaveLobby();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Alert");
        }
        else
        {
            listPlayers.text = NetworkController.Instance.GetListPlayers();
            startGame.interactable = NetworkController.Instance.OwnerRoom();
        }
    }
}