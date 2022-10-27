using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Player1 P1;
    public Player1 P2;
    public GameState State = GameState.ChooseAttack;
    private Player1 damagedPlayer;
    private Player1 winner;
    public TMP_Text WinnerText;

    public GameObject gameOverPanel;

    public enum GameState
    {
        ChooseAttack,
        Attacks,
        Damages,
        Draw,
        GameOver
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.ChooseAttack:
                if (P1.AttackValue != null && P2.AttackValue != null)
                {
                    P1.AnimateAttack();
                    P2.AnimateAttack();
                    P1.IsClickable(false);
                    P2.IsClickable(false);
                    State = GameState.Attacks;
                }
                break;
            case GameState.Attacks:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    damagedPlayer = GetDamagedPlayer();
                    if (damagedPlayer != null)
                    {
                        damagedPlayer.AnimateDamage();
                        State = GameState.Damages;
                    }
                    else
                    {
                        P1.AnimateDraw();
                        P2.AnimateDraw();
                        State = GameState.Draw;
                    }
                }
                break;
            case GameState.Damages:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    //calculate health
                    if (damagedPlayer == P1)
                    {
                        P1.ChangeHealth(-10);
                        P2.ChangeHealth(5);
                    }
                    else
                    {
                        P1.ChangeHealth(5);
                        P2.ChangeHealth(-10);
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
                        WinnerText.text = winner == P1 ? "Player 1 Wins" : "Player 2 Wins";
                        ResetPlayers();
                        State = GameState.GameOver;
                    }
                }
                break;
            case GameState.Draw:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    ResetPlayers();
                    P1.IsClickable(true);
                    P2.IsClickable(true);
                    State = GameState.ChooseAttack;
                }
                break;
        }

    }

    private void ResetPlayers()
    {
        damagedPlayer = null;
        P1.Reset();
        P2.Reset();
    }

    private Player1 GetDamagedPlayer()
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

    private Player1 getWinner()
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
