using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    //player positions
	public float speed, jumpspeed, punchForce, throwForce;
    public int baseDamage;
    public int health, armor, weaponDamage;
    private bool grounded, carryingEquipment;
    private GameObject grabbedObject;

    AudioClip punchAudio;
    AudioClip footstepsAudio;
    AudioSource audioSource;

	private enum Arm
	{
		Left = 0,
		Right = 1
	}

	// Use this for initialization
	private void Start()
	{
        audioSource = GetComponent<AudioSource>();
        punchAudio = Resources.Load<AudioClip>("punch_heavy_stereo");
        footstepsAudio = Resources.Load<AudioClip>("shitty_footsteps");
    }
	
    //update stuff that shouldn't be synced to fps, like controls, here...
	private void Update()
	{
        HandleActions();
        HandleMovement();
    }

    //...and physics and such here
    private void FixedUpdate()
    {
        grounded = IsGrounded();

        //disable gravity when on ground so that we don't tumble around
        //like drunkards. Oh, wait a sec..
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = !grounded;
        rigidbody.drag = !grounded ? 0 : 10;
    }

    #region Update loop utilities

    private void HandleMovement()
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

        if(grabbedObject != null)
        {
            grabbedObject.transform.position = transform.position + transform.forward;
        }

        if (!audioSource.isPlaying && movement != Vector3.zero)
            audioSource.PlayOneShot(footstepsAudio);
    }

    private void HandleActions()
    {
        //disable when in flight, no doublejumping allowed here
        if (Input.GetButtonDown("Jump") && grounded)
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jumpspeed;
            grounded = false;
            Debug.Log("Jump!");
        }

        if (Input.GetButtonDown("Punch")) Punch();
        if (Input.GetButtonDown("Grab")) Grab();
        if (Input.GetButtonDown("Throw")) Throw();

    }

    #endregion

    #region Actions

    private void Throw()
    {
        if(grabbedObject != null)
        {
            Debug.Log("Throw!");
            RemovePowerups();
            grabbedObject.GetComponent<Rigidbody>().velocity += (transform.forward + (transform.up * 1.5f)) * throwForce;
            grabbedObject = null;
        }
    }

	private void Grab()
	{
        //get the closest player/item/thing
        GameObject target = GetNearestTarget();

        if (target == null) return; //nothing to grab

        if (grabbedObject == null)
        {
            Debug.Log("Grab!");
            //grab it!
            grabbedObject = target;

            if (!carryingEquipment)
            {
                AddPowerups();
                carryingEquipment = true;
            }
        }
        else
        {
            Debug.Log("Drop!");
            //let go of the object
            RemovePowerups();
            carryingEquipment = false;
            grabbedObject = null;
        }
	}

	private void Punch()
	{
		//get the direction the player is facing
        var direction = transform.forward;

		//get the closest object in that direction
        GameObject target = GetNearestTarget();

        if(target != null && target.GetComponent<Rigidbody>() != null)
        {
            //add force to fist, towards the targeted object
            target.GetComponent<Rigidbody>().velocity += direction.normalized * punchForce;
            audioSource.PlayOneShot(punchAudio);
            Debug.Log(string.Format("Punch!"));
        }
	}

    private void Use()
    {
        var stats = grabbedObject.GetComponent<ItemController>();
        if (stats == null || !stats.equippable) return; //doesn't have powerups

        if (!stats.used)
        {
            if (stats.healthOnUse > 0) this.health += stats.healthOnUse;
            if (stats.armorOnUse > 0) this.armor += stats.armorOnUse;

            stats.used = true;

            Debug.Log("Used item: +" + stats.healthOnUse + "health, +" + stats.armorOnUse + " armor");
        }
        else
        {
            Debug.Log("Item has no charges left");
        }
    }

    #endregion

    #region Action utilities

    //check the distance from collider bounds to ground below
    //to see if we're grounded or in mid flight for some reason
    private bool IsGrounded()
    {
        Collider collider = GetComponent<Collider>();
        float distance = collider.bounds.extents.y;

        return Physics.Raycast(transform.position, -Vector3.up, distance + 0.0f);
    }

    private GameObject GetNearestTarget()
    {
        GameObject target = null;
        var bumper = transform.Find("radarlol").GetComponent<SphereCollider>();
        Collider[] collider = Physics.OverlapSphere(bumper.transform.position, bumper.radius);

        foreach(var collision in collider)
        {
            //ignore self
            if (collision.name == this.name) continue;

            //ignore non-gravitational objects, ie. map and static stuff
            var rigidbody = collision.attachedRigidbody;
            if(rigidbody != null && rigidbody.useGravity && rigidbody.name != this.name)
            {
                target = collision.gameObject;
            }
        }
        return target;
    }

    private void AddPowerups()
    {
        var stats = grabbedObject.GetComponent<ItemController>();
        if (stats == null || !stats.equippable) return; //doesn't have powerups

        if (stats.damagePowerup > 0) this.weaponDamage = stats.damagePowerup;
        if (stats.armorPowerup > 0) this.armor = stats.armorPowerup;
    }

    private void RemovePowerups()
    {
        this.weaponDamage = 0;
        this.armor = 0;
    }

    #endregion
}