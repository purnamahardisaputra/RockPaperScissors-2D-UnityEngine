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

    private void Start()
    {
        PhotonNetwork.JoinLobby();
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
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(newRoomInputField.text);
    }

    public void ClickStartGame(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
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
        Debug.Log("Joined Room :" + PhotonNetwork.CurrentRoom.Name);
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

        StartGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 1;
    }

    private void UpdatePlayerList()
    {
        // PhotonNetwork.PlayerList (Alternative)
        // foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) (Alternative)

        foreach (var item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();
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
        Debug.Log("Room List Updated");
        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }
        this.roomItemList.Clear();
        foreach (var roomInfo in roomList)
        {
            RoomItem newRoomItem = Instantiate(roomItemPrefab, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo.Name);
            this.roomItemList.Add(newRoomItem);

        }
    }
}
