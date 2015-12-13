using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//player positions
	public float x, y, z;
	public float rotation;
	public float speed;

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
}
