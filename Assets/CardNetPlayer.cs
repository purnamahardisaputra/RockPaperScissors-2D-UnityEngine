using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CardNetPlayer : MonoBehaviourPun
{
    public static List<CardNetPlayer> NetPlayers = new List<CardNetPlayer>(2);
    public CardPlayer cardPlayer;
    private Card[] cards;

    public void Set(CardPlayer player)
    {
        cardPlayer = player;
        cards = cardPlayer.GetComponentsInChildren<Card>();
        foreach (var card in cards)
        {
            var button = card.GetComponent<Button>();
            button.onClick.AddListener(() => RemoteClickButton(card.AttackValue));
        }
    }

    private void OnDestroy()
    {
        NetPlayers.Remove(this);
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
        NetPlayers.Remove(this);
    }
}
