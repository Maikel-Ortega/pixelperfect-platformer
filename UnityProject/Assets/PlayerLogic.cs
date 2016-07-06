using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour 
{
	public float jumpForce 					= 10f;
	public float gravity 					= 4f;
	public float drag 						= 0.2f;
	public float acceleration 				= 0.5f;
	public float maxSpeedX 					= 0.2f;
	public float maxSpeedY 					= 0.2f;
	public float  jumpChargeTime			= 0.1f;

	public Vector3 velocity;
	public BoxCollider myCollider;
	public bool grounded;
	public LayerMask collisionLayermask;
	public Animator animator;
	public SpriteRenderer spriteRenderer;

	private float jumpChargeTimer 			= 0f;
	private int lastDir 					= 1;
	private bool lastFramePressedJump 		= false;
	private Vector3 lastVelocity;

	public event System.Action<PlayerLogic> OnPlayerLanded;

	void Update () 
	{
		CheckMovement();
		CheckCollisions();
		CheckAnimations();
	}

	void LateUpdate()
	{
		Move();
		lastVelocity = velocity;
	}

	void Move()
	{
		transform.Translate(velocity);
	}

	void CheckMovement()
	{
		float inpx = Input.GetAxis("Horizontal");
		if(Mathf.Abs(inpx) > 0 &&  Mathf.Sign(velocity.x) != Mathf.Sign(inpx))
		{
			velocity.x = 0;
		}

		Vector3 deltaVelocity = Vector3.zero;

		if(!grounded)
		{
			deltaVelocity.y = gravity*Time.deltaTime;
		}

		if(inpx != 0 && Mathf.Sign((int)inpx) != Mathf.Sign(lastDir))
		{
			lastDir *= -1;
			spriteRenderer.flipX = (lastDir == -1);
		}

		deltaVelocity.x = inpx*acceleration * Time.deltaTime;
		velocity = velocity + deltaVelocity;

		if(Input.GetKeyDown(KeyCode.Z))
		{
			lastFramePressedJump = true;
		}

		if(lastFramePressedJump && grounded)
		{
			if(velocity.y <= 0 )
			{
				animator.Play("jump");
				velocity.y = jumpForce;
			}
		}

		if(!Input.GetKey(KeyCode.Z) && velocity.y > 0)
		{
			if( velocity.y > jumpForce*0.3f)
			{
				Debug.Log("SHORT JUMP");
				velocity.y = jumpForce*0.3f;
			}				
		}

		if(inpx == 0 && grounded)
		{
			velocity.x *= drag;
		}

		velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX, maxSpeedX);
		velocity.y = Mathf.Clamp(velocity.y, -maxSpeedY, maxSpeedY);

		if(!Input.GetKey(KeyCode.Z))
		{
			lastFramePressedJump = false;
		}
	}

	void CheckCollisions()
	{				
		//Up-Down collisions
		int diry = velocity.y > 0 ? 1 : -1;

		Vector3 origin = transform.position - Vector3.right * myCollider.size.x/2 + diry*Vector3.up *0.7f* myCollider.size.y/2f;
		Vector3 end = transform.position + Vector3.right * myCollider.size.x/2 + diry*Vector3.up *0.7f* myCollider.size.y/2f;

		float maxMov = 0.2f+ velocity.magnitude;
		int n = 10;
		bool collision = false;
		for(int i = 1; i < n; i++)
		{
			Vector3 rayOrigin = Vector3.Lerp(origin,end, (float)i/(float)n);
			RaycastHit hit;

			if(Physics.Raycast(rayOrigin, diry*Vector3.up, out hit,maxMov, collisionLayermask.value))
			{
				transform.position = new Vector3(transform.position.x,  hit.point.y - diry* myCollider.size.y/2, transform.position.z);
				collision = true;

				Debug.DrawRay(rayOrigin,diry*Vector3.up*maxMov,Color.red);
			}
			else
			{
				Debug.DrawRay(rayOrigin,diry*Vector3.up*maxMov,Color.green);
			}
		}

		if(collision)
		{
			if(diry<=0)
			{	
				if(grounded == false && OnPlayerLanded != null)
				{
					OnPlayerLanded(this);
				}
				grounded = true;
			}
			velocity = new Vector3(velocity.x,0f,velocity.z);
		}
		else
		{
			grounded = false;
		}

		//Left-Right collisions
		int dirx = velocity.x > 0 ? 1: -1;
		origin = transform.position + dirx*Vector3.right * 0.7f*myCollider.size.x/2 + Vector3.down *0.9f* myCollider.size.y/2f;
		
		end = origin + Vector3.up * myCollider.size.y;

		collision = false;
		for(int i = 1; i < n; i++)
		{
			Vector3 rayOrigin = Vector3.Lerp(origin,end, (float)i/(float)n);
			RaycastHit hit;

			if(Physics.Raycast(rayOrigin, dirx*Vector3.right, out hit,maxMov))
			{
				transform.position = new Vector3(hit.point.x - dirx* myCollider.size.x/2, transform.position.y, transform.position.z);
				collision = true;
				Debug.DrawRay(rayOrigin,dirx*Vector3.right*maxMov,Color.red);
			}
			else
			{
				Debug.DrawRay(rayOrigin,dirx*Vector3.right*maxMov,Color.green);
			}
		}
		if(collision)
		{
			velocity = new Vector3(0f,velocity.y,velocity.z);
		}
	}


	void CheckAnimations()
	{
		if(Input.GetAxis("Horizontal") != 0 && grounded )
		{
			animator.Play("walk");
		}
		else if (!grounded && velocity.y < 0)
		{
			animator.Play("fall");
		}
		else if (grounded )
		{
			animator.Play("idle");
		}
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position + velocity*2f);

	}
}
