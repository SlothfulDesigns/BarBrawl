using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    //player positions
	public float speed, jumpspeed, punchForce;
    private bool grounded;

	private enum Arm
	{
		Left = 0,
		Right = 1
	}

	// Use this for initialization
	void Start()
	{

	}
	
    //update stuff that shouldn't be synced to fps, like controls, here...
	void Update()
	{
        HandleActions();
        HandleMovement();
    }

    //...and physics and such here
    void FixedUpdate()
    {
        grounded = IsGrounded();

        //disable gravity when on ground so that we don't tumble around
        //like drunkards. Oh, wait a sec..
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = !grounded;
        rigidbody.drag = !grounded ? 0 : 10;
    }

    //check the distance from collider bounds to ground below
    //to see if we're grounded or in mid flight for some reason
    bool IsGrounded()
    {
        Collider collider = GetComponent<Collider>();
        float distance = collider.bounds.extents.y;

        return Physics.Raycast(transform.position, -Vector3.up, distance + 0.0f);
    }

    void HandleMovement()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        //handle rotation first so we can skip handling other movement if the player is currently jumping/grounded
        Vector3 inputRotation = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        if (inputRotation != Vector3.zero)
            rigidbody.rotation = Quaternion.LookRotation(inputRotation);

        //skip movement if grounded to disable air steering
        if (!grounded) return;

        //handle moving around
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(inputHorizontal, 0.0f, inputVertical);

        rigidbody.velocity += movement;
    }

    void HandleActions()
    {
        //disable when in flight, no doublejumping allowed here
        if (Input.GetButtonDown("Jump") && grounded)
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jumpspeed;
            grounded = false;
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

		//get the fist subcomponent of the selected arm
		//get the direction the player is facing
        var direction = transform.forward;

		//get the closest object in that direction
        Rigidbody target = GetNearestTarget();

        if(target != null)
        {
		//add force to fist, towards the targeted object
        target.velocity += direction.normalized * punchForce;

		//profit!
        Debug.Log(string.Format("{0} arm punch!", arm));
        }
	}

    private Rigidbody GetNearestTarget()
    {
        Rigidbody target = null;
        var bumper = transform.Find("radarlol").GetComponent<SphereCollider>();
        Collider[] collider = Physics.OverlapSphere(bumper.transform.position, bumper.radius);
        foreach(var collision in collider)
        {
            //ignore self
            if (collision.name == this.name) continue;

            //ignore non-gravitational objects, ie. map and static stuff
            var rigidbody = collision.attachedRigidbody;
            if(rigidbody != null && rigidbody.useGravity)
            {
                target = collision.attachedRigidbody;
            }
        }
        return target;
    }
}