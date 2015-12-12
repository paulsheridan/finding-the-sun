using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour 
{
	public Transform SpawnPoint;

	void  OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
			SpawnPoint.position = new Vector2(transform.position.x, transform.position.y + 1);
		}
	}
}