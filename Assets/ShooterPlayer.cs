using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ShooterPlayer : MonoBehaviourPun
{
    [SerializeField] float speed;
    [SerializeField] TMP_Text playerName;
    [SerializeField] int health = 10;
    private void Start()
    {
        playerName.text = photonView.Owner.NickName + $"({health})";
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
            return;

        Vector2 moveDir = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate(moveDir * Time.deltaTime * speed);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("TakeDamage", RpcTarget.Others, 1);
        }
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        health -= amount;
        playerName.text = photonView.Owner.NickName + $"({health})";
        GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
    }
}
