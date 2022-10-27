using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CardGameManager : MonoBehaviour, IOnEventCallback
{
    public GameObject netPlayerPrefab;
    public CardPlayer P1;
    public CardPlayer P2;
    public float restoreValue = 5;
    public float damageValue = 10;
    public GameState State, NextState = GameState.NetPlayersInitialization;
    private CardPlayer damagedPlayer;
    private CardPlayer winner;
    public TMP_Text ping;
    public TMP_Text WinnerText;
    public GameObject gameOverPanel;
    public List<int> syncReadyPlayers = new List<int>(2);
    public bool Online = true;
    public enum GameState
    {
        SyncState,
        NetPlayersInitialization,
        ChooseAttack,
        Attacks,
        Damages,
        Draw,
        GameOver
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        if (Online)
        {
            PhotonNetwork.Instantiate(netPlayerPrefab.name, Vector3.zero, Quaternion.identity);
            StartCoroutine(PingCoroutine());
            State = GameState.NetPlayersInitialization;
            NextState = GameState.NetPlayersInitialization;
        }
        else
        {
            State = GameState.ChooseAttack;
        }
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.SyncState:
                if (syncReadyPlayers.Count == 2)
                {
                    syncReadyPlayers.Clear();
                    State = NextState;
                }
                break;
            case GameState.NetPlayersInitialization:
                if (CardNetPlayer.NetPlayers.Count == 2)
                {
                    foreach (var netPlayer in CardNetPlayer.NetPlayers)
                    {
                        if (netPlayer.photonView.IsMine)
                        {
                            netPlayer.Set(P1);
                        }
                        else
                        {
                            netPlayer.Set(P2);
                        }
                    }
                    ChangeState(GameState.ChooseAttack);
                }
                break;
            case GameState.ChooseAttack:
                if (P1.AttackValue != null && P2.AttackValue != null)
                {
                    P1.AnimateAttack();
                    P2.AnimateAttack();
                    P1.IsClickable(false);
                    P2.IsClickable(false);
                    ChangeState(GameState.Attacks);
                }
                break;
            case GameState.Attacks:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    damagedPlayer = GetDamagedPlayer();
                    if (damagedPlayer != null)
                    {
                        damagedPlayer.AnimateDamage();
                        ChangeState(GameState.Damages);
                    }
                    else
                    {
                        P1.AnimateDraw();
                        P2.AnimateDraw();
                        ChangeState(GameState.Draw);
                    }
                }
                break;
            case GameState.Damages:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    //calculate health
                    if (damagedPlayer == P1)
                    {
                        P1.ChangeHealth(-damageValue);
                        P2.ChangeHealth(restoreValue);
                    }
                    else
                    {
                        P1.ChangeHealth(restoreValue);
                        P2.ChangeHealth(-damageValue);
                    }

                    var winner = getWinner();

                    if (winner == null)
                    {
                        P1.IsClickable(true);
                        P2.IsClickable(true);
                        ResetPlayers();
                        State = GameState.ChooseAttack;
                    }
                    else
                    {
                        gameOverPanel.SetActive(true);
                        WinnerText.text = winner == P1 ? $"{P1.NickName.text} wins" : $"{P2.NickName.text} wins";
                        ResetPlayers();
                        ChangeState(GameState.GameOver);
                    }
                }
                break;
            case GameState.Draw:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    ResetPlayers();
                    P1.IsClickable(true);
                    P2.IsClickable(true);
                    ChangeState(GameState.ChooseAttack);
                }
                break;
        }


    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void ChangeState(GameState newState)
    {

        if (Online == false)
        {
            State = newState;
            return;
        }
        if (this.NextState == newState)
            return;

        // kirim message bahwa kita sudah siap
        var actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        var raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(1, actorNum, raiseEventOptions, SendOptions.SendReliable);
        this.State = GameState.SyncState;
        this.NextState = newState;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            var actorNum = (int)photonEvent.CustomData;
            if (syncReadyPlayers.Contains(actorNum) == false)
                syncReadyPlayers.Add(actorNum);
        }
    }
    IEnumerator PingCoroutine()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            ping.text = "Ping : " + PhotonNetwork.GetPing() + "ms";
            if (PhotonNetwork.GetPing() >= 100)
            {
                ping.color = Color.red;
            }
            else if (PhotonNetwork.GetPing() > 80)
            {
                ping.color = Color.yellow;
            }
            else
            {
                ping.color = Color.green;
            }
            yield return wait;
        }
    }

    private void ResetPlayers()
    {
        damagedPlayer = null;
        P1.Reset();
        P2.Reset();
    }

    private CardPlayer GetDamagedPlayer()
    {
        Attack? PlayerAtk1 = P1.AttackValue;
        Attack? PlayerAtk2 = P2.AttackValue;

        if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Paper)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Scissor)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Rock)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Scissor)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Rock)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Paper)
        {
            return P2;
        }

        return null;
    }

    private CardPlayer getWinner()
    {
        if (P1.Health == 0)
        {
            return P2;
        }
        else if (P2.Health == 0)
        {
            return P1;
        }
        else
        {
            return null;
        }
    }
    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Quit Application");
    }

}
