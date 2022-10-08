using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image image;
    public void UpdateBar(float fillAmount)
    {
        // Image.fillAmount = fillAmount
        image.DOFillAmount(fillAmount, 0.5f);
        if (fillAmount > 0.6)
        {
            image.DOColor(Color.green, 0.5f);
        }
        else if (fillAmount > 0.4)
        {
            image.DOColor(Color.yellow, 0.5f);
        }
        else
        {
            image.DOColor(Color.red, 0.5f);
        }
    }
}
