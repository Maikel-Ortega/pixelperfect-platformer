using UnityEngine;
using System.Collections;

public enum CameraStates { GOTOTARGET, WAIT, SIMPLEFOLLOW }

public class Mario3Camera : MonoBehaviour 
{

	public Rect freezone;

	public Transform target;
	public Vector3 velocity;
	public Vector3 targetPosition;
	public CameraStates state = CameraStates.WAIT;

	// Use this for initialization
	void Start () 
	{
		target.GetComponent<PlayerLogic>().OnPlayerLanded+= HandleOnPlayerLanded;
	}

	void HandleOnPlayerLanded (PlayerLogic obj)
	{
		targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
		Debug.Log(targetPosition);
		state = CameraStates.GOTOTARGET;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(state)
		{
		case CameraStates.GOTOTARGET:
			UpdateGoToTarget();
			break;
		case CameraStates.WAIT:
			UpdateWait();
			break;
		case CameraStates.SIMPLEFOLLOW:
			UpdateSimpleFollow();
			break;
		}
	}


	void UpdateGoToTarget()
	{
		this.velocity -= this.velocity*Time.deltaTime*5f;
		targetPosition = new Vector3(target.position.x, target.position.y, this.transform.position.z);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*5f);
		if(Vector3.Distance(transform.position, targetPosition)< 0.2f)
		{
			state = CameraStates.SIMPLEFOLLOW;
		}
	}

	void UpdateSimpleFollow()
	{
		targetPosition = new Vector3(target.position.x, this.transform.position.y, this.transform.position.z);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*15f);
	}

	void UpdateWait()
	{
		if(!CharacterOnFreezone())
		{
			Vector3 pVelocity = target.GetComponent<PlayerLogic>().velocity;
			this.velocity = pVelocity;

			targetPosition = new Vector3(target.position.x, target.position.y, this.transform.position.z);

			this.velocity += (targetPosition - transform.position)*Time.deltaTime*0.5f;

		}
		else
		{
			this.velocity -= this.velocity*Time.deltaTime*5f;
		}
		transform.Translate(velocity);

	}

	void LateUpdate()
	{
	}

	bool CharacterOnFreezone()
	{
		freezone.center= new Vector3(transform.position.x, transform.position.y, target.position.z);
		return freezone.Contains((Vector2)target.position);
	}

	void OnDrawGizmos()
	{
		if(CharacterOnFreezone())
		{
			Gizmos.color = Color.green;
		}
		else
		{
			Gizmos.color = Color.red;
		}
		Gizmos.DrawWireSphere(freezone.center,1f);
		Gizmos.DrawWireCube(freezone.center, new Vector3(freezone.width, freezone.height));

		Gizmos.DrawWireSphere(targetPosition,0.1f);
	}
}
