using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_Text feedbackText;

    private void Start()
    {
        usernameInput.text = PlayerPrefs.GetString(PropertyNames.Player.NickName, "");
    }
    public void ClickConnect()
    {
        feedbackText.text = "";

        if (usernameInput.text.Length < 3)
        {
            feedbackText.text = "Username Min 3 Characters";
            return;
        }

        //simpan username
        PlayerPrefs.SetString(PropertyNames.Player.NickName, usernameInput.text);
        PhotonNetwork.NickName = usernameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;

        // connect to server
        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "Connecting...";
    }

    // dijalankan ketika sudah connect ke server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        feedbackText.text = "Connected to Master";
        StartCoroutine(LoadLevelAfterConnectedAndReady());

    }

    IEnumerator LoadLevelAfterConnectedAndReady()
    {
        while (PhotonNetwork.IsConnectedAndReady == false)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel("Lobby");
    }

}
