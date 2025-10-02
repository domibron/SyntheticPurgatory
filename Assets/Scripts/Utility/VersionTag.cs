using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


public class VersionTag : MonoBehaviour
{
    public TMP_Text uiText;

    // Start is called before the first frame update
    void Start()
    {
        uiText.text = "V " + Application.version;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
