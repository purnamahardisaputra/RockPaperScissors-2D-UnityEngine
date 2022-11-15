using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CardNetPlayer : MonoBehaviourPun
{
    public static List<CardNetPlayer> NetPlayers = new List<CardNetPlayer>(2);
    private Card[] cards;

    public void Set(CardPlayer player)
    {
        player.NickName.text = photonView.Owner.NickName;
        cards = player.GetComponentsInChildren<Card>();

        foreach (var card in cards)
        {
            var button = card.GetComponent<Button>();
            button.onClick.AddListener(() => RemoteClickButton(card.AttackValue));

            if (photonView.IsMine == false)
            {
                button.interactable = false;
            }
        }
    }

    private void Awake()
    {
        // NetPlayers.Clear();
    }

    private void RemoteClickButton(Attack value)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RemoteClickButtonRPC", RpcTarget.Others, value);
        }
    }

    [PunRPC]
    private void RemoteClickButtonRPC(int value)
    {
        foreach (var card in cards)
        {
            if (card.AttackValue == (Attack)value)
            {
                var button = card.GetComponent<Button>();
                button.onClick.Invoke();
                break;
            }
        }
    }
    private void OnEnable()
    {
        NetPlayers.Add(this);
    }

    private void OnDisable()
    {
        foreach (var card in cards)
        {
            // if (card == null)
            // {
            //     continue;
            // }
            var button = card.GetComponent<Button>();
            button.onClick.RemoveListener(() => RemoteClickButton(card.AttackValue));
        }
        NetPlayers.Remove(this);
    }
}
