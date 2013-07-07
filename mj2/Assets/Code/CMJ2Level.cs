using UnityEngine;
using System.Collections;

public class CMJ2Level : MonoBehaviour 
{
	
	public GUIText m_scoreText;
	
    public static CMJ2Level g;
    
    public int m_directivesTotal = 3;
    public int m_directivesComplete = 0;

	void Awake () 
    {
        g = this;
    }
    
    void Start ()
    {
    	CMJ2Manager.g.directives(0, m_directivesTotal);
    }
    
	public void directiveComplete (CMJ2Directive cdir)
	{
		m_directivesComplete++;
		CMJ2Manager.g.directives(m_directivesComplete, m_directivesTotal);
	}

	/*void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			
		}
	}*/

}
