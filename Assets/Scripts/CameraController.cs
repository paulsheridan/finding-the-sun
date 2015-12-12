using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	private Transform target;
	public float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;
	public float cameraHeight = 2f;

	void Awake ()
	{
		target = GameObject.FindWithTag ("Player").transform;
	}

	void FindPlayer ()
	{
		target = GameObject.FindWithTag ("Player").transform;
	}


	void LateUpdate () 
	{
		if (target == null)
		{
			FindPlayer ();
		}
		Vector3 targetPosition = target.TransformPoint(new Vector3(0, cameraHeight, -10));
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	}
}