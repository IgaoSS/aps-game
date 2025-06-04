using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;

public class EnterMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerNameMenu;
    [SerializeField] private Text roomNameMenu;
    private int playersInRoom;

    public void CreateRoom()
    {

        NetworkController.Instance.SetNickname(playerNameMenu.text);
        NetworkController.Instance.CreateRoom(roomNameMenu.text);
    }

    public void EnterRoom()
    {
        NetworkController.Instance.SetNickname(playerNameMenu.text);
        NetworkController.Instance.EnterRoom(roomNameMenu.text);
    }
}
