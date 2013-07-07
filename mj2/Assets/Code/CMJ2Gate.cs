using UnityEngine;
using System.Collections;

public class CMJ2Gate : MonoBehaviour 
{
	
	public GameObject m_leftDoor;
	public GameObject m_rightDoor;
	public float m_doorOpenDist = 0.5f;
	public float m_doorOpenSpeed = 0.25f;
	public float m_doorCloseDelay = 1f;
	
	[HideInInspector]
	public Transform m_xform;
	
	public bool m_open = false;

	void Awake ()
	{
		m_xform = transform;
	}

	public void open (CMJ2Hero hero)
	{
		if (m_open)
			return;
			
		m_open = true;
		StartCoroutine(execOpen(hero));
	}
	
	IEnumerator execOpen (CMJ2Hero hero)
	{
		Vector3 posl = m_leftDoor.transform.position;
		Vector3 posr = m_rightDoor.transform.position;
				
		iTween.MoveTo(m_leftDoor, iTween.Hash(
			"position", posl - new Vector3 (m_doorOpenDist, 0f),
			"time", m_doorOpenSpeed,
			"easetype", iTween.EaseType.easeOutQuad));
		iTween.MoveTo(m_rightDoor, iTween.Hash(
			"position", posr + new Vector3 (m_doorOpenDist, 0f),
			"time", m_doorOpenSpeed,
			"easetype", iTween.EaseType.easeOutQuad));
		
		yield return new WaitForSeconds (m_doorCloseDelay);

		iTween.MoveTo(m_leftDoor, iTween.Hash(
			"position", posl,
			"time", m_doorOpenSpeed,
			"easetype", iTween.EaseType.easeInQuad));
		iTween.MoveTo(m_rightDoor, iTween.Hash(
			"position", posr,
			"time", m_doorOpenSpeed,
			"easetype", iTween.EaseType.easeInQuad));
			
		yield return new WaitForSeconds (m_doorOpenSpeed);

		hero.doorClosed();
	}
	
}
