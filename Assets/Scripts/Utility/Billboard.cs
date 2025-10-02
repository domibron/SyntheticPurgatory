using System;
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
/// Makes the object this is attached to, point to the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
	public bool invertDirection = false;

	Transform camTransform;

	// Start is called before the first frame update
	void Start()
	{
		try
		{
			camTransform = Camera.main.transform;
		}
		catch (NullReferenceException)
		{
			Debug.LogError("Main camera was not detected!", this);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (camTransform == null)
		{
			return;
		}

		if (!invertDirection)
		{
			transform.LookAt(camTransform.position);
		}
		else
		{
			transform.LookAt(transform.position - (camTransform.position - transform.position));
		}
	}
}
