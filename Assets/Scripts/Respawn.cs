using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour 
{
	public Transform SpawnPoint;
	public GameObject player;
	
	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player")
		{
			player.transform.position = SpawnPoint.position;
		}
	}
}
