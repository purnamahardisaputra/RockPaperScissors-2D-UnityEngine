using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : MonoBehaviour
{
    public Transform atkPoskRef;
    public Card choosenCard;
    private Tweener animationTwener;
    public TMP_Text healthText;
    public float Health;
    public HealthBar healthBar;
    public float MaxHealth;
    public AudioSource audioSource;
    public AudioClip damageClip;

    private void Start()
    {
        Health = MaxHealth;
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
            choosenCard.Reset();
        }

        choosenCard = newCard;
        choosenCard.transform.DOScale(choosenCard.transform.localScale * 1.2f, 0.2f);
    }

    public void ChangeHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, 100);
        //healthbar
        healthBar.UpdateBar(Health / MaxHealth);
        //text
        healthText.text = Health + " / " + MaxHealth;
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
