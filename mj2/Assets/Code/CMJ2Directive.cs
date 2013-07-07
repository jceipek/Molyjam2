using UnityEngine;
using System.Collections;

public class CMJ2Directive : MonoBehaviour {

    public enum CMJ2DirectiveType {
    	NONE = 0,
        UP,
        CHG_DIR
    };
    public CMJ2DirectiveType m_directive;
    
    public Color m_completeTint = new Color (1f, 1f, 1f, 0.25f);

	public void complete ()
	{
		print("Complete");
		
		CTintMesh tm = GetComponent<CTintMesh>();
		if (tm)
		{
			tm.m_mainTopLeft = m_completeTint;
			tm.tintCell();
		}
	}

	void Start () 
	{
	
	}

}
