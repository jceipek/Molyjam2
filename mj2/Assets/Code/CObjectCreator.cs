using UnityEngine;
using System.Collections;

public class CObjectCreator : MonoBehaviour 
{
	
	public const float SQR_DIST = 20f*20f;
	public float m_creationRadius = 1f;
	public float m_delay = 0f;
	public float m_frequency = 1f;
	public float m_frequencyRandom = 0.5f;

	public float m_throwAngle = 0f;
	public float m_throwForce = 0f;
	public Collider m_ignoreCollider;

	Vector3 m_throwVector;

	public GameObject m_objectToCreate;

	public CAudioEffectSource m_effectOnCreate;
	
	float m_nextCreationTime = -1;
	Transform m_xform;

	// Use this for initialization
	void Start () 
	{
		m_xform = transform;
		m_nextCreationTime = Time.time + m_delay + (Random.value - 0.5f) * m_frequencyRandom;
		m_throwVector = Quaternion.AngleAxis(m_throwAngle, Vector3.forward) * new Vector3 (m_throwForce, 0);
	}
	
	public void spawn ()
	{
		//if ((CMushroomBoard.g.m_camFocusPos - m_xform.position).sqrMagnitude > SQR_DIST) 
		//	return;
		Vector3 pos = m_xform.position + (Vector3) (Random.insideUnitCircle * m_creationRadius);

		// TODO pool?
		GameObject go = Instantiate(m_objectToCreate, pos, Quaternion.identity) as GameObject;
		Rigidbody rb = go.rigidbody;
		if (rb && m_throwForce > 0f)
			rb.velocity = m_xform.rotation * m_throwVector;

		if (m_ignoreCollider)
		{
			Collider col = go.collider;
			if (col)
				Physics.IgnoreCollision(col, m_ignoreCollider);
		}

		if (m_effectOnCreate != null)
			m_effectOnCreate.play(1f, pos);
	}

	void FixedUpdate () 
	{
		if (Time.time > m_nextCreationTime)
		{
			spawn();			
			m_nextCreationTime = Time.time + m_frequency + (Random.value - 0.5f) * m_frequencyRandom;
		}
	}
	
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, m_creationRadius);
	}
	
}
