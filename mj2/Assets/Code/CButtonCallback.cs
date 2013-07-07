using UnityEngine;
using System.Collections;

public class CButtonCallback : MonoBehaviour 
{

	public MonoBehaviour m_delegateToObject;
	
	// Override to implement
	public virtual void onClick (Collider col) 
	{
		if (m_delegateToObject)
		{
			Debug.Log("Delegating click to " + m_delegateToObject.name);
			m_delegateToObject.SendMessage("onClick", col);
		}
	}
}
