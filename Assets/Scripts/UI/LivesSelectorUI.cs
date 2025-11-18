using UnityEngine;
using UnityEngine.UI;

public class LivesSelectorUI : MonoBehaviour
{
    [SerializeField]
    private Image firstHeart;
    [SerializeField]
    private Image secondHeart;
    [SerializeField]
    private Image thirdHeart;

    [SerializeField]
    private Sprite activeSprite;
    [SerializeField]
    private Sprite inactiveSprite;


    public void SetFirstHeart(bool state)
    {
        if (state)
        {
            firstHeart.sprite = activeSprite;
        }
        else
        {
            firstHeart.sprite = inactiveSprite;
        }
    }

    public void SetSecondHeart(bool state)
    {
        if (state)
        {
            secondHeart.sprite = activeSprite;
        }
        else
        {
            secondHeart.sprite = inactiveSprite;
        }
    }

    public void SetThirdHeart(bool state)
    {
        if (state)
        {
            thirdHeart.sprite = activeSprite;
        }
        else
        {
            thirdHeart.sprite = inactiveSprite;
        }
    }


    public void UpdateMaxLives(int amount)
    {
        GameManager.Instance.SetMaxLives(amount);
    }
}
