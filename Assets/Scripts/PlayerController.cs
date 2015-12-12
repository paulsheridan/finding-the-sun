using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float maxSpeed = 10f;
	public float airSpeed = 9f;
	public float climbSpeed = 4f;

	Animator anim;

	//private float groundNormal;
	//private float groundDistance;
	private float leftDistance;
	private float rightDistance;

	private bool facingRight = true;
	private bool grounded = false;
	private float groundRadius = 0.2f;
	public Transform groundCheck;
	public LayerMask whatIsGround;

	public bool onHill;
	
	public float jumpForce = 660;
	public float maxJumpTime = 0.3f;
	private float timeOfJump;
	public float extraJumpForce = 35;

	private float moveX;

	private bool climbing = false;

	public float TimeInJump
	{
		get
		{
			return Time.time - timeOfJump;
		}
	}

	void Start () 
	{
		anim = GetComponent<Animator> ();
	}

	void Update()
	{
		//checks to see if the player is or isn't on the ground
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool ("Ground", grounded);

		//moves the character in the X direction, accounts for ground slope (hopefully)
		//RaycastHit2D downRay = Physics2D.Raycast (transform.position, -Vector2.up, Mathf.Infinity, whatIsGround);
		//groundNormal = Vector3.Dot (Vector2.right, downRay.normal);
		//groundDistance =  Mathf.Abs(downRay.point.y - transform.position.y);

		RaycastHit2D leftRay = Physics2D.Raycast (transform.position, -Vector2.right, Mathf.Infinity, whatIsGround);
		leftDistance =  Mathf.Abs(leftRay.point.x - transform.position.x);

		RaycastHit2D rightRay = Physics2D.Raycast (transform.position, Vector2.right, Mathf.Infinity, whatIsGround);
		rightDistance =  Mathf.Abs(rightRay.point.x - transform.position.x);


		//checks to see if the character should jump
		if ((grounded) && Input.GetButtonDown ("Jump")) 
			Jump ();

		//instantiates a wall jump
		if ((climbing) && Input.GetButtonDown ("Jump"))
			WallJump ();

		if ((climbing) && Input.GetButtonDown ("Crouch"))
			WallDrop ();

		//tells the animator what the current vertical speed is
		anim.SetFloat ("vSpeed", GetComponent<Rigidbody2D>().velocity.y);

		//tells the animator what the current horizontal speed is
		if (Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) > 0) 
			anim.SetFloat ("Speed", Mathf.Abs (moveX));
		else
			anim.SetFloat ("Speed", 0);
	}

	void FixedUpdate() 
	{
		//takes the input and makes a horizontal variable
		moveX = Input.GetAxis ("Horizontal");

		//adds jump if the jump button is held down
		if(Input.GetButton ("Jump") && TimeInJump < maxJumpTime)
			AddJump ();
		if (!climbing) 
		{
			if (grounded) 
				GetComponent<Rigidbody2D>().velocity = new Vector2 (moveX * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			else
				GetComponent<Rigidbody2D>().velocity = new Vector2 (moveX * airSpeed, GetComponent<Rigidbody2D>().velocity.y);
		}
		
		//checks to see if the character needs to be flipped
		if (!climbing)
		{
			if (moveX < 0 && !facingRight)
			Flip ();
		else if (moveX > 0 && facingRight)
			Flip ();
		}
	}

	//flips the character based on movement direction
	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	//makes the character jump
	void Jump ()
	{
		anim.SetBool ("Ground", false);
		timeOfJump = Time.time;
		GetComponent<Rigidbody2D>().AddForce (new Vector2(0, (jumpForce - (GetComponent<Rigidbody2D>().velocity.y * 36))));
	}

	//adds more force to the jump if the jump button is held
	void AddJump()
	{
		GetComponent<Rigidbody2D>().AddForce (new Vector2(0, extraJumpForce));
	}

	//checks to see if the character is touching a climbable wall and executes the code that runs when the character is wall climbing
	void OnCollisionStay2D (Collision2D coll) 
	{
		if (coll.gameObject.tag == "WallCheck") 
		{
			anim.SetBool ("onWall", true);
			climbing = true;
			Physics2D.gravity = new Vector2 (0, 0);
			float moveY = Input.GetAxis ("Vertical");
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, moveY * climbSpeed);
			if (leftDistance > rightDistance && !facingRight)
				Flip ();
			else if (rightDistance > leftDistance && facingRight)
				Flip ();
		}
	}

	//checks to see if the character has left a climbable wall and executes the code beneat it
	void OnCollisionExit2D (Collision2D coll)
	{
		if (coll.gameObject.tag == "WallCheck") 
		{
			LeaveWall ();
		}
	}

	//jumps from walls
	void WallJump ()
	{
		timeOfJump = Time.time;
		if (rightDistance < 0.25)
		{
			transform.position = new Vector3 (transform.position.x - 0.01f, transform.position.y + 0.01f, transform.position.z);
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (-100, 660 - (GetComponent<Rigidbody2D>().velocity.y * 40)));
			LeaveWall ();
		}
		
		if (leftDistance < 0.25)
		{
			transform.position = new Vector3 (transform.position.x + 0.1f, transform.position.y + 0.1f, transform.position.z);
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (100, 660 - (GetComponent<Rigidbody2D>().velocity.y * 40)));
			LeaveWall ();
		}
	}

	//drops from a wall when the player hits b or shift
	void WallDrop ()
	{
		if (rightDistance < 0.25)
		{
			transform.position = new Vector3 (transform.position.x - 0.01f, transform.position.y + 0.01f, transform.position.z);
		}
		
		if (leftDistance < 0.25)
		{
			transform.position = new Vector3 (transform.position.x + 0.01f, transform.position.y + 0.01f, transform.position.z);
		}
	}

	//makes sure that physics and animation states are back to normal whenever player leaves a wall
	void LeaveWall ()
	{
		climbing = false;
		anim.SetBool ("onWall", false);
		Physics2D.gravity = new Vector2 (0, -50f);
	}
}
