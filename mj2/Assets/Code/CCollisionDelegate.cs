using UnityEngine;
using System.Collections;

public class CCollisionDelegate : MonoBehaviour 
{

	public class CTwoColliders {
		public CTwoColliders (Collider colm, Collider colo) {
			mine = colm;
			other = colo;
		}
		public Collider mine;
		public Collider other;
	};
	public MonoBehaviour m_delegateToObject;

	void OnCollisionEnter (Collision col) 
	{
		if (m_delegateToObject)
			m_delegateToObject.SendMessage("OnCollisionEnter", col);
	}
	/*void OnCollisionExit (Collision col) 
	{
		if (m_delegateToObject)
			m_delegateToObject.SendMessage("OnCollisionExit", col);
	}*/
	
	void OnTriggerStay (Collider col) 
	{
		if (m_delegateToObject)
		{
			CTwoColliders cols = new CTwoColliders (collider, col);
			m_delegateToObject.SendMessage("OnTriggerStayExt", cols);
		}
	}
	/*void OnTriggerExit (Collider col) 
	{
		if (m_delegateToObject)
		{
			CTwoColliders cols = new CTwoColliders (collider, col);
			m_delegateToObject.SendMessage("OnTriggerExitExt", cols);
		}
	}*/
}
