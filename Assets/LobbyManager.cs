using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button StartGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject RoomListObject;
    [SerializeField] GameObject PlayerListObject;
    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] PlayerItem playerItemPrefab;
    List<PlayerItem> playerItemList = new List<PlayerItem>();
    List<RoomItem> roomItemList = new List<RoomItem>();
    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        feedbackText.text = "Connecting to Lobby...";

        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
        else
        {
            PhotonNetwork.JoinLobby();
            roomPanel.SetActive(false);
        }
    }
    public void ClickCreateRoom()
    {
        feedbackText.text = "";
        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Room Name Min 3 Characters";
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(newRoomInputField.text, roomOptions);
    }

    public void ClickStartGame(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }

    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room : " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Created Room : " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room : " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Joined Room : " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);
        //update player list
        UpdatePlayerList();

        //atur button start game
        SetStartGameBUtton();

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        SetStartGameBUtton();
    }

    public void SetStartGameBUtton()
    {
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        StartGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 2;
    }

    private void UpdatePlayerList()
    {
        // destroy dulu semua player item yang sudah ada
        foreach (var item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        // PhotonNetwork.PlayerList (Alternative)
        // foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) (Alternative)
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, PlayerListObject.transform);
            newPlayerItem.Set(player);
            playerItemList.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.transform.SetAsFirstSibling();
            }

        }

        // start game hanya bisa diklik ketika jumlah pemain tertentu
        // jadi atur juga disini
        SetStartGameBUtton();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + " " + message);
        feedbackText.text = returnCode + " " + message;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        Debug.Log("Room List Updated");

        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();

        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);

        // sort yang open dibuat pertama;
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen)
                roomInfoList.Add(roomInfo);
        }

        // kemudian close
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen == false)
                roomInfoList.Add(roomInfo);
        }

        foreach (var roomInfo in roomInfoList)
        {
            if (roomInfo.MaxPlayers == 0 || roomInfo.IsVisible == false)
                continue;

            RoomItem newRoomItem = Instantiate(roomItemPrefab, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            this.roomItemList.Add(newRoomItem);
        }
    }
}
