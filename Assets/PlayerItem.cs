using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;

    public void Set(Photon.Realtime.Player player)
    {
        playerName.text = player.NickName;
        if (player == PhotonNetwork.MasterClient)
        {
            playerName.text += "<b> (Master) </b>";
            playerName.color = Color.red;
        }
        if (player.IsLocal && !player.IsMasterClient)
        {
            playerName.text += "<b> (You) </b>";
            playerName.color = Color.yellow;
        }
    }
}
