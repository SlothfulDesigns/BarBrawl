using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//player positions
	public float x, y, z;
	public float rotation;
	public float speed;

	private enum Arm
	{
		Left = 0,
		Right = 1
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		float inputH = Input.GetAxis("Horizontal");
		float inputV = Input.GetAxis("Vertical");
		Vector3 movement = new Vector3(inputV, 0.0f, inputH);
		GetComponent<Rigidbody>().velocity = speed * movement;
	}

	void Grab()
	{
		//get direction the player is facing
		//get the closest player/item/thing in that direction
		//grab it!
		//proft
	}

	void Punch(Arm arm)
	{
		//get the fist subcomponent of the selected arm
		//get the direction the player is facing
		//get the closest object in that direction
		//add force to fist, towards the targeted object
		//profit!
	}
}