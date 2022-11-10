using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public CardPlayer player;
    public GameManager gameManager;
    public BotStats botStats;
    private float timer = 0;
    int lastSelected = 0;
    Card[] cards;
    public bool IsReady = false;


    public void SetStats(BotStats newStats, bool RestoreFullHealth = false)
    {
        this.botStats = newStats;
        var newPlayerStats = new PlayerStats
        {
            MaxHealth = this.botStats.MaxHealth,
            RestoreHealth = this.botStats.RestoreHealth,
            DamageHealth = this.botStats.DamageHealth
        };
        player.SetStats(newPlayerStats, RestoreFullHealth: true);
    }
    IEnumerator Start()
    {
        cards = player.GetComponentsInChildren<Card>();
        yield return new WaitUntil(() => player.IsReady);
        SetStats(this.botStats);
        this.IsReady = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.State != GameManager.GameState.ChooseAttack)
        {
            timer = 0;
            return;
        }
        if (timer < botStats.choosingInterval)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0;
        ChooseAttack();
        // Debug.Log("Bot Choose " + cards.GetValue(lastSelected));
    }

    public void ChooseAttack()
    {
        var random = Random.Range(1, cards.Length);
        var selection = (lastSelected + random) % cards.Length;
        player.SetChosenCard(cards[selection]);
        lastSelected = selection;
    }
}
