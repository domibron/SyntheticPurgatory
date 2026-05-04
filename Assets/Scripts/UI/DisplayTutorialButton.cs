using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Display text and input action key onto a TMP Text component.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class DisplayTutorialButton : MonoBehaviour
{
    TMP_Text textComp;

    [SerializeField]
    private string actionName;

    private string originalText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComp = GetComponent<TMP_Text>();
        originalText = textComp.text;
    }

    // Update is called once per frame
    void Update()
    {
        //print(InputSystem.actions.FindAction(actionName).GetBindingDisplayString(InputBinding.MaskByGroup("Player")));
        textComp.text = originalText + "\n" + InputSystem.actions.FindAction(actionName).GetBindingDisplayString();

    }
}
