using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShooterGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector2.zero, Quaternion.identity);
    }
}
    