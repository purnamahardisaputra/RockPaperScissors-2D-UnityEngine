using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Button button;
    LobbyManager manager;
    RoomInfo roomInfo;
    public void Set(LobbyManager manager, RoomInfo roomInfo)
    {
        this.manager = manager;
        this.roomInfo = roomInfo;
        roomNameText.text = roomInfo.Name + $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        if (roomInfo.IsOpen == false)
            button.interactable = false;
    }

    public void ClickRoomButton()
    {
        manager.JoinRoom(this.roomInfo.Name);
    }
}
