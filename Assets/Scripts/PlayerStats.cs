using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour 
{

	public Transform SpawnPoint;
	//private PlayerController playerController;

	public float health = 3f;
	public float maxHealth = 3f;
	public float enemyDamage = 1f;
	public float healthPickup = 1f;
	public float damageForce = 40f;

	private float damageDelayPeriod = 2f;
	private float lastHitTime;
	private Animator anim;

	void Start ()
	{
		anim = GetComponent<Animator> ();
	}


	void OnCollision2D (Collision2D col)
	{
		if (col.gameObject.tag == "Enemy")
		{
			if (Time.time > lastHitTime + damageDelayPeriod) 
			{
				if(health > 0f)
				{
					TakeDamage(col.transform); 
					lastHitTime = Time.time;
				}
				else
				{
					RespawnPlayer ();
				}
			}
		}
	}

	void TakeDamage (Transform enemy)
	{
		//playerController.jump = false;
		health -= enemyDamage;
		Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;
		GetComponent<Rigidbody2D>().AddForce (hurtVector * damageForce);
		UpdateUI ();
	}

	void UpdateUI ()
	{

	}


	void AddHealth ()
	{
		health += healthPickup;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	void RespawnPlayer ()
	{
		//whatever animations, sounds and particles we want tied to dying.
		//i assume this will mean disabling player for a short time too
		health = maxHealth;
		transform.position = SpawnPoint.position;
		anim.SetTrigger ("Die");
	}
}
