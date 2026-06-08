using TMPro;
using UnityEngine;

public class ConfirmationBox : MonoBehaviour
{
    public static ConfirmationBox Instance { get; private set; }

    public delegate void ConfirmationBoxDelegate(bool confirmedAction);
    public event ConfirmationBoxDelegate OnConfirmation;

    [SerializeField]
    GameObject confirmationBoxObject;

    [SerializeField]
    TMP_Text boxTitle;
    [SerializeField]
    TMP_Text boxDesc;
    [SerializeField]
    TMP_Text boxConfirm;
    [SerializeField]
    TMP_Text boxCancel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("There were multiple confirmation boxes!");
            return;
        }

        Instance = this;
        CloseBox();
    }

    public bool TryOpenConfirmationBox(string title, string message, string confirmMessage = "Confirm", string cancelMessage = "Cancel")
    {
        if (confirmationBoxObject.activeSelf) return false;

        OpenConfirmationBox(title, message, confirmMessage, cancelMessage);
        return true;
    }


    public void OpenConfirmationBox(string title, string message, string confirmMessage = "Confirm", string cancelMessage = "Cancel")
    {
        boxTitle.text = title;
        boxDesc.text = message;
        boxConfirm.text = confirmMessage;
        boxCancel.text = cancelMessage;

        confirmationBoxObject.SetActive(true);
    }

    public void ConfirmAction()
    {
        OnConfirmation?.Invoke(true);
        CloseBox();
    }

    public void DenyAction()
    {
        OnConfirmation?.Invoke(false);
        CloseBox();
    }

    public void CloseBox()
    {
        confirmationBoxObject.SetActive(false);
    }
}
