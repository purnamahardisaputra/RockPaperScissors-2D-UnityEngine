using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPlayer : MonoBehaviour
{
    public Transform atkPoskRef;
    public Card choosenCard;
    private Tweener animationTwener;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public float Health;
    public PlayerStats playerStats = new PlayerStats
    {
        MaxHealth = 100,
        RestoreHealth = 5,
        DamageHealth = 10
    };
    public HealthBar healthBar;
    public AudioSource audioSource;
    public AudioClip damageClip;
    public TMP_Text NickName { get => nameText; }
    public bool IsReady = false;

    private void Start()
    {
        Health = playerStats.MaxHealth;
    }
    public void SetStats(PlayerStats stats, bool RestoreFullHealth = false)
    {
        this.playerStats = stats;
        if (RestoreFullHealth)
        {
            Health = playerStats.MaxHealth;
        }
        UpdateHealthBar();
    }
    public Attack? AttackValue
    {
        get => choosenCard == null ? null : choosenCard.AttackValue;
        // get
        // {
        //     if (chosenCard == null)
        //         return null;
        //     else
        //         return chosenCard.AttackValue;
        // }
    }

    public void Reset()
    {
        if (choosenCard != null)
        {
            choosenCard.Reset();
        }
        choosenCard = null;
    }
    public void SetChosenCard(Card newCard)
    {
        if (choosenCard != null)
        {
            choosenCard.transform.DOKill();
            choosenCard.Reset();
        }

        choosenCard = newCard;
        choosenCard.transform.DOScale(choosenCard.transform.localScale * 1.2f, 0.2f);
    }

    public void UpdateHealthBar()
    {
        healthBar.UpdateBar(Health / playerStats.MaxHealth);
        healthText.text = Health + "/" + playerStats.MaxHealth;
    }

    public void ChangeHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, playerStats.MaxHealth);
        UpdateHealthBar();
    }

    public void AnimateAttack()
    {
        Tweener tweener = choosenCard.transform
            .DOMove(atkPoskRef.position, 0.5f);
    }

    public void AnimateDamage()
    {
        audioSource.PlayOneShot(damageClip);
        var image = choosenCard.GetComponent<Image>();
        animationTwener = image
            .DOColor(Color.red, 0.1f)
            .SetLoops(3, LoopType.Yoyo)
            .SetDelay(0.2f);
    }

    public void AnimateDraw()
    {
        animationTwener = choosenCard.transform
            .DOMove(choosenCard.OriginalPosition, 1)
            .SetEase(Ease.InBack)
            .SetDelay(0.2f);
    }

    public bool IsAnimating()
    {
        return animationTwener.IsActive();
    }

    public void IsClickable(bool value)
    {
        Card[] cards = GetComponentsInChildren<Card>();
        foreach (var card in cards)
        {
            card.SetClickable(value);
        }
    }
}
