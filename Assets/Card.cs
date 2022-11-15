using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Attack AttackValue;
    public CardPlayer player;
    public Vector2 OriginalPosition;
    Vector2 OriginalScale;
    Color originalColor;

    bool isClickable = true;
    private void Start()
    {
        OriginalPosition = this.transform.position;
        OriginalScale = this.transform.localScale;
        originalColor = GetComponent<Image>().color;
    }

    public void onClick()
    {
        Debug.Log("Player Klik " + AttackValue);
        if (isClickable)
        {
            OriginalPosition = this.transform.position;
            player.SetChosenCard(this);
        }
    }

    public void Reset()
    {
        transform.position = OriginalPosition;
        transform.localScale = OriginalScale;
        GetComponent<Image>().color = originalColor;
    }

    public void SetClickable(bool value)
    {
        isClickable = value;
    }

    // private void OnDestroy()
    // {
    //     var button = GetComponentInChildren<Button>();
    //     button.onClick.RemoveAllListeners();
    // }

}
