using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

/// <summary>
/// Removes the game object this is attached to after a period of time.
/// </summary>
public class RemoveAfterTime : MonoBehaviour
{
    /// <summary>
    /// How long to wait before destroying the game object this is attached to.
    /// </summary>
    public float timeToWaitBeforeRemoving = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToWaitBeforeRemoving);
    }
}
