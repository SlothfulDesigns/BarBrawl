using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    //player positions
	public float speed, jumpspeed;
    private bool falling;

	private enum Arm
	{
		Left = 0,
		Right = 1
	}

	// Use this for initialization
	void Start()
	{
        falling = false;
	}
	
	// Update is called once per frame
	void Update()
	{
        HandleActions();
        HandleMovement();
    }

    void HandleMovement()
    {

        if (GetComponent<Rigidbody>().velocity.y < 1) falling = false;

        //handle rotation first so we can skip handling other movement if the player is currently jumping/falling
        Vector3 inputRotation = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        if (inputRotation != Vector3.zero)
            GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(inputRotation);

        //skip movement if falling
        if (falling) return;

        //handle moving around
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(inputHorizontal, 0.0f, inputVertical);

        GetComponent<Rigidbody>().velocity = speed * movement;

    }
    void HandleActions()
    {
        if (Input.GetButtonDown("Jump") && !falling)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpspeed);
            falling = true;
            Debug.Log("Jump!");
        }

        if (Input.GetButtonDown("PunchLeft")) Punch(Arm.Left);
        if (Input.GetButtonDown("PunchRight")) Punch(Arm.Right);

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

        Debug.Log(string.Format("{0} arm punch!", arm));
		//get the fist subcomponent of the selected arm
		//get the direction the player is facing
		//get the closest object in that direction
		//add force to fist, towards the targeted object
		//profit!
	}
}