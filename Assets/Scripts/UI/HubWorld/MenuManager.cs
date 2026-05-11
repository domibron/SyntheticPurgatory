using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UIMenuItem
{
    public string name;
    public GameObject gameObject;
}

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    UIMenuItem[] menuItems;

    [SerializeField]
    bool openMenuOnStart = true;

    [SerializeField, FormerlySerializedAs("defualtMenu")]
    string defaultMenu = "";

    void Start()
    {
        if (openMenuOnStart)
            OpenMenu(defaultMenu);
    }


    public void OpenMenu(string name)
    {
        if (!IsNameValid(name)) return; // dont screw up the menus by hiding all of them.

        foreach (UIMenuItem item in menuItems)
        {
            if (item.name == name)
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void OpenMenu(int index)
    {
        if (menuItems.Length <= 0) return;

        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].gameObject.SetActive(i == index);
        }
    }



    public bool IsNameValid(string name)
    {
        foreach (UIMenuItem item in menuItems)
        {
            if (item.name == name) return true;
        }

        return false;
    }

    public bool IsMenuOpen(string name)
    {
        foreach (UIMenuItem item in menuItems)
        {
            if (item.name == name) return item.gameObject.activeSelf;
        }

        return false;
    }
}
