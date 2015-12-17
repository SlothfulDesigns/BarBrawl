using UnityEngine;
using System.Collections;

// Orthographic camera that zooms out when players get further apart from each other
// Copied and adapted from Tank Tutorial
// Comments TBA

public class CameraControl : MonoBehaviour {

	public float m_DampTime = 0.2f; // Approximate time it takes for camera to follow
	public float m_ScreenEdgeBuffer = 4f; // Ensure that players stay on the screen rather than edge via buffer
	public float m_MinSize = 6.5f; // Minimum size (or zoom level) for the camera
	/*[HideInInspector]*/ public Transform[] m_Targets; // Array of all of the players
	
	
	private Camera m_Camera; // Reference to camera to be able to change size
	private float m_ZoomSpeed; // Damp zooming
	private Vector3 m_MoveVelocity; // Damp moving      
	private Vector3 m_DesiredPosition; // Position that camera is trying to reach = middle of the players

	/*
	 * These will need some adaptation, perhaps the m_DesiredPosition should be center of the level instead,
	 * and the camera should zoom based on that.
	 */

	private void Awake()
	{
		m_Camera = GetComponentInChildren<Camera>();
	}

	private void FixedUpdate() // Calls functions for moving & zooming
	{
		Move();
		Zoom();
	}

	private void Move()
	{
		FindAveragePosition();
		
		transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
	}

	private void FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;
		
		for (int i = 0; i < m_Targets.Length; i++)
			// Loops through targets (players), sets zoom point & average position accordingly
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			// Only zooms if target is active, might be unnecessary for our purposes
			
			averagePos += m_Targets[i].position;
			numTargets++;
		}
		
		if (numTargets > 0)
			averagePos /= numTargets;
		// Calculates middle point

		averagePos.y = transform.position.y;
		// We don't want to screw with the y axis
		
		m_DesiredPosition = averagePos;
	}

	private void Zoom() // Zoom function
	{
		float requiredSize = FindRequiredSize();
		m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
	}

	
	private float FindRequiredSize()
	{
		// Goes through players, sets zoom level to accommodate for the furthest one
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
		
		float size = 0f;
		
		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
			
			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
			
			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
			
			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
			// Absolutes so we don't get negative values
		}
		
		size += m_ScreenEdgeBuffer;
		// Add screen edge buffer so players definitely fit to screen
		
		size = Mathf.Max(size, m_MinSize);
		// Find minimum size so we don't zoom in too much
		
		return size;
	}

	public void SetStartPositionAndSize() // Sets camera to the start position
	{
		FindAveragePosition();
		
		transform.position = m_DesiredPosition;
		
		m_Camera.orthographicSize = FindRequiredSize();
	}
}
