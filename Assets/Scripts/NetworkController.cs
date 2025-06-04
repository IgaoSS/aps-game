using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexão estabelecida");
    }

    public void CreateRoom(string nameRoom)
    {
        PhotonNetwork.CreateRoom(nameRoom);
    }

    public void EnterRoom(string nameRoom)
    {
        PhotonNetwork.JoinRoom(nameRoom);
    }

    public void SetNickname(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    public string GetListPlayers()
    {
        var list = "";
        foreach(var player in PhotonNetwork.PlayerList)
        {
            list += player.NickName + "\n";
        }
        return list;
    }

    public bool OwnerRoom()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void StartGame(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
