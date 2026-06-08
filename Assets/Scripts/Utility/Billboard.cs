using System;
using UnityEngine;



/// <summary>
/// Makes the object this is attached to, point to the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
	[SerializeField]
	private bool invertDirection = false;
	[SerializeField]
	private bool lockUpwards = false;

	Transform camTransform;

	// Start is called before the first frame update
	void Start()
	{
		SetTargetCamera();
	}

	// Update is called once per frame
	void Update()
	{
		if (camTransform == null)
		{
			SetTargetCamera();
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

		if (lockUpwards) transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	}

	public void SetTargetCamera()
	{
		try
		{
			camTransform = Camera.main.transform;
		}
		catch (NullReferenceException)
		{
			//Debug.LogError("Main camera was not detected!", this);
		}
	}
}
