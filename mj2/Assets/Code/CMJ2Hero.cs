using UnityEngine;
using System.Collections;

public class CMJ2Hero : MonoBehaviour 
{

    public float m_targetVelocity = 4f;
    public float m_initialJumpVelocity = 6f;
    public float m_accel = 0.05f;
    public float m_climbSpeed = 2f;

    public Vector3 m_debugVel;
    
    float m_lastJumpTime;
    
    public /**/ float m_dir = 1f;
    public /**/ int m_Vdir = 0;
    
    public /**/ bool m_onGround = false;
    public /**/ int m_wayUp = 0;
    
    public PhysicMaterial m_spikedMaterial;
    
    public ParticleSystem m_blood;
    
    public enum CMJ2HeroState {
    	NONE = 0,
        IDLE,
        WALK,
        CLIMB,
        SPIKED,
        GATE
    };
    public CMJ2HeroState m_state;
    CMJ2HeroState m_prevState = CMJ2HeroState.IDLE;

	public Transform m_hiRaycastPoint;
	public float m_hiRaycastAngle = 75f;
	public float m_hiRaycastDist = 3f;
	Vector3 m_hiRaycastNorm;
	public float m_centerToTopDist = 1.5f;
	
	public Transform m_lowRaycastPoint;
	public float m_lowRaycastDist = 1f;
	public float m_lowRaycastAngle = 45f;
	Vector3 m_lowRaycastNorm;
	
	public Transform m_underRaycastPoint;
	
	public float m_exitSpeed = 0.5f;
	public float m_inGateDepth = 1f;
	public float m_onLadderDepth = -1f;
	public float m_ladderSnapSpeed = 0.025f;
	
	public Transform m_sprite;
//{"walk":{"cells":[64,68,70,72],"loop":true,"freq":0.15},"jump":{"cells":[74,76],"loop":false,"freq":0.3},"fall":{"cells":[78],"loop":false,"freq":0.3},"idle":{"cells":[66],"loop":false,"freq":0.3},"climb":{"cells":[128,130,132,134],"loop":true,"freq":0.15},"spiked":{"cells":[96,98,100,102],"loop":false,"freq":0.15}}
	CCellSpriteAnimated m_spriteAnim;
	
	Quaternion m_quatFlipped;
    protected Transform m_xform;
    protected Rigidbody m_rb;
    
    Vector3 m_origLocPos;
    
    CMJ2Gate m_inGate;

    void Awake ()
    {
        m_xform = transform;
        m_rb = rigidbody;
        
        m_hiRaycastNorm = Quaternion.AngleAxis(m_hiRaycastAngle, Vector3.forward) * Vector3.right;
        m_lowRaycastNorm = Quaternion.AngleAxis(m_lowRaycastAngle, Vector3.forward) * Vector3.right;
        m_quatFlipped = Quaternion.Euler(0f, 180f, 0f);
        
        m_origLocPos = m_sprite.localPosition;
        m_spriteAnim = m_sprite.GetComponent<CCellSpriteAnimated>();
    }
    
    public void changeState (CMJ2HeroState state)
    {
    	
    	if (m_state != state)
    	{
	    	m_prevState = m_state;
    		m_state = state;
    		
    		switch (m_state)
    		{
    			case CMJ2HeroState.IDLE:
    				m_spriteAnim.runAnimation("idle");
    			break;
    			case CMJ2HeroState.WALK:
    				m_spriteAnim.runAnimation("walk");
    			break;
    			case CMJ2HeroState.CLIMB:
    				m_spriteAnim.runAnimation("climb");
    			break;
    			case CMJ2HeroState.SPIKED:
    				m_rb.velocity = Vector3.zero;
    				m_spriteAnim.runAnimation("spiked");
    				
    				Collider[] cols = GetComponentsInChildren<Collider>() as Collider[];
    				foreach (Collider col in cols)
    					col.sharedMaterial = m_spikedMaterial;
    				m_rb.useGravity = false;
    				
    				if (m_blood)
	    				m_blood.Play();
    			break;
    		}
    		
    	   	if (m_state == CMJ2HeroState.CLIMB)
    	   	{
    	   		if (!m_rb.isKinematic)
    	   		{
					m_rb.isKinematic = true;
		     		m_sprite.localPosition = new Vector3 (m_sprite.localPosition.x, m_sprite.localPosition.y, m_onLadderDepth);					
    	   		}
    	   	}
    	   	else
    	   	{
    	   		if (m_rb.isKinematic)
    	   		{
					m_rb.isKinematic = false;
		     		m_sprite.localPosition = m_origLocPos;
    	   		}
    	   	}
    	   	
  	    	print(m_prevState + "->" + m_state + " " + Time.time);
    	}
    	
    }
	
	IEnumerator Start () 
    {
    	changeState(CMJ2HeroState.IDLE);
    	yield return new WaitForSeconds(1f);
    	setDir(1f);
    	changeState(CMJ2HeroState.WALK);
        //yield return new WaitForSeconds(1.5f);
        //changeState(CMJ2HeroState.JUMP);
    }

    void execWalk ()
    {
    	if (m_inGate == null)
    	{
	    	RaycastHit hitinfo;
	    	int hitlayer = -1;
	    	if (Physics.Raycast(m_lowRaycastPoint.position, new Vector3 (m_dir * m_lowRaycastNorm.x, m_lowRaycastNorm.y), out hitinfo, m_lowRaycastDist, CMJ2Manager.MASK_ALL_GROUND))
	    		hitlayer = hitinfo.collider.gameObject.layer;
	    			    		
	    	if (hitlayer != CMJ2Manager.LAYER_GROUND)
	    		jump(false);
    	}   	
        Vector3 curvel = m_rb.velocity;
        float targetvel = m_dir * m_targetVelocity;

        m_rb.velocity = new Vector3 (Mathf.MoveTowards(curvel.x, targetvel, m_accel * Time.deltaTime), curvel.y);

        m_debugVel = m_rb.velocity;///
    }
    
    void execClimb ()
    {
    	float xdistfromcenter = (m_xform.position.x + 1000f) % 1f;
    	if (xdistfromcenter > 0.5f)
    		xdistfromcenter -= 1f;
    	//print(m_xform.position.x + " " + xdistfromcenter);
    	m_xform.position += new Vector3 (
    		Mathf.Abs(xdistfromcenter) < m_ladderSnapSpeed ? -xdistfromcenter :
    			(Mathf.Sign(xdistfromcenter) * -m_ladderSnapSpeed), 
    			m_climbSpeed * (float)m_Vdir * Time.deltaTime);    	
    }
    
    void execGate ()
    {
    	m_rb.velocity = new Vector3 (Mathf.Lerp(m_rb.velocity.x, m_dir * m_exitSpeed, 0.5f), m_rb.velocity.y);
     	float dist = Mathf.Abs(m_inGate.m_xform.position.x - m_xform.position.x);
     	if (dist < 0.05f && !Mathf.Approximately(m_sprite.localPosition.z, m_inGateDepth))
     		m_sprite.localPosition = new Vector3 (m_sprite.localPosition.x, m_sprite.localPosition.y, m_inGateDepth);  		 
    }
    
    public void doorClosed ()
    {
    	m_sprite.renderer.enabled = false;
    }
    
    void makeJump ()
    {
    	print("JUMP");
    	Vector3 curvel = m_rb.velocity;
        m_rb.velocity = new Vector3 (curvel.x, m_initialJumpVelocity);
		m_lastJumpTime = Time.time;
		m_spriteAnim.runAnimation("jump");
    }
    
    void jump (bool bumped)
    {
    	if (m_onGround && Time.time > m_lastJumpTime + 0.1f)
    	{
    		int hitlayer = -1;
 			RaycastHit hitinfo;

    		if (!bumped)
    		{
  		    	if (Physics.Raycast(m_xform.position, Vector3.up, out hitinfo, m_centerToTopDist, CMJ2Manager.MASK_ALL_EXCEPT_HERO))
  					hitlayer = hitinfo.collider.gameObject.layer;

 		    	if (hitlayer != CMJ2Manager.LAYER_GROUND &&
					hitlayer != CMJ2Manager.LAYER_SPIKE)
				{
			    	//changeState(CMJ2HeroState.JUMP);
			    	makeJump();
				}
    		}
    		else
    		{
		    	if (Physics.Raycast(m_xform.position, Vector3.up, out hitinfo, m_centerToTopDist, 1 << CMJ2Manager.LAYER_GROUND) ||
		    		Physics.Raycast(m_hiRaycastPoint.position, new Vector3 (m_dir * m_hiRaycastNorm.x, m_hiRaycastNorm.y), out hitinfo, m_hiRaycastDist, 1 << CMJ2Manager.LAYER_GROUND))
		    	{
		    		hitlayer = hitinfo.collider.gameObject.layer;
		    		//Debug.DrawLine(m_hiRaycastPoint.position, hitinfo.point, Color.white, 1f);
		    	}
		    
		    	if (hitlayer != CMJ2Manager.LAYER_GROUND)
				{
			    	//changeState(CMJ2HeroState.JUMP);
			    	makeJump();
				}			    	
			    else
			    {
			    	// Bump backwards
			    	setDir(-m_dir);
			    	changeState(CMJ2HeroState.WALK);
			    }
    		}
    	}
	    //else
	    //	print("Not jumping " + m_onGround + " " + (Time.time - m_lastJumpTime));
    }
    
    void climb ()
    {
    	RaycastHit hitinfo;
    	// Check one cell up
    	int hitlayer = -1;
    	if (Physics.Raycast(m_xform.position + new Vector3 (0f, 1f, -4f), Vector3.forward, out hitinfo, 5f, CMJ2Manager.MASK_ALL_EXCEPT_HERO))
    		hitlayer = hitinfo.collider.gameObject.layer;

	    changeState(CMJ2HeroState.CLIMB);
	    setVDir(hitlayer == CMJ2Manager.LAYER_LADDER ? 1 : -1);
    }
    
    void setDir (float dir)
    {
    	if (dir == m_dir)
    		return;
    	
    	print("Direction " + dir);	
    	m_dir = dir;
    	m_sprite.localRotation = m_dir >= 0f ? Quaternion.identity : m_quatFlipped;
    }

    void setVDir (int vdir)
    {
    	if (vdir == m_Vdir)
    		return;
    	
    	print("VDirection " + vdir);	
    	m_Vdir = vdir;
    }
	
	/*void OnTriggerStay (Collider col)
	{
		print("MyTriggerStay " + col.name);
		
		changeState(CMJ2HeroState.UP);
	}*/
	
	
	void OnTriggerStayExt (CCollisionDelegate.CTwoColliders cols)
	{
		//print("Trigger " + cols.mine.name + " " + cols.other.name + " " + m_onGround);
		
		if (m_state != CMJ2HeroState.WALK ||
			!m_onGround)
			return;
		
		int lay = cols.other.gameObject.layer;
		bool isjump = lay == CMJ2Manager.LAYER_GROUND ||
					  lay == CMJ2Manager.LAYER_SPIKE;
					  		
		switch (cols.mine.name)
		{
			case "leftFloor":
				if (m_dir < 0f)
				{
					if (isjump)
						jump(true);
				}
				break;
			case "rightFloor":
				if (m_dir > 0f)
				{
					if (isjump)
						jump(true);
				}
				break;
		}
	}

	void FixedUpdate () 
    {
    	if (m_state != CMJ2HeroState.GATE && m_state != CMJ2HeroState.SPIKED)
		{
	    	RaycastHit hitinfo;
	    	int hitlayer = -1;
	    	if (Physics.Raycast(m_underRaycastPoint.position + new Vector3 (0f, 0f, -4f), Vector3.forward, out hitinfo, 5f, CMJ2Manager.MASK_ALL_GROUND))
	    		hitlayer = hitinfo.collider.gameObject.layer;
	    	
	    	/*if (hitlayer == CMJ2Manager.LAYER_SPIKE)
	    	{
	    		changeState(CMJ2HeroState.SPIKED);
	    		return;
	    	}*/
	    	
	    	bool wasonground = m_onGround;
	    		
	    	m_onGround = hitlayer == CMJ2Manager.LAYER_GROUND;
	    	
	    	if (m_onGround && !wasonground && 
	    		m_state != CMJ2HeroState.CLIMB &&
	    		m_spriteAnim.currentAnimationName != "walk")
	    	{
	    		print("re-walk");	
	    		m_spriteAnim.runAnimation("walk");
	    	}
	
			// Ladder? *** when on ladder, look for ANY collider to continue climbing
			hitlayer = -1;
			bool onlayer = Physics.Raycast(m_xform.position + new Vector3 (0f, 0f, -4f), Vector3.forward, out hitinfo, 5f, CMJ2Manager.MASK_ALL_EXCEPT_HERO);
			if (onlayer)
				hitlayer = hitinfo.collider.gameObject.layer;
				
			if (onlayer)
			{
				if (hitlayer == CMJ2Manager.LAYER_GATE)
				{
					CMJ2Gate gate = hitinfo.collider.GetComponent<CMJ2Gate>();
					if (gate)
						gate.open(this);
						
					m_inGate = gate;
					setDir(Mathf.Sign(gate.m_xform.position.x - m_xform.position.x));
					changeState(CMJ2HeroState.GATE);
				}
				else if (hitlayer == CMJ2Manager.LAYER_LADDER)
				{
					float posxoncol = hitinfo.transform.InverseTransformPoint(hitinfo.point).x;
					
					if (m_state != CMJ2HeroState.CLIMB)
					{
						if ((m_dir > 0 && posxoncol < -0.01f) ||
							(m_dir < 0 && posxoncol > 0.01f))
							climb();
					}
				}
			}
			else
			{
				// Continue climbing until out of ground completely if ladder is absent
				if (!Physics.Raycast(m_xform.position + new Vector3 (0f, 1f, -4f), Vector3.forward, out hitinfo, 5f, CMJ2Manager.MASK_ALL_GROUND) &&
					!m_onGround && 
					m_state == CMJ2HeroState.CLIMB)
					 changeState(CMJ2HeroState.WALK);
			}
	    	
	    	float vely = m_rb.velocity.y;
	    	int newwayup = vely > 1f ? 1 : vely < -1f ? -1 : 0;
	    	
	    	//// DEBUG
	    	if (m_wayUp >= 0 && newwayup < 0)
	    	{
	    		//Debug.DrawLine(m_lowRaycastPoint.position + new Vector3 (-1f, -0.5f), m_lowRaycastPoint.position + new Vector3 (1, -0.5f), Color.blue, 10f);
	    		
	    		m_spriteAnim.runAnimation("fall");
	    	}
	    	if (newwayup != 0)
		    	m_wayUp = newwayup;
		}
		
        switch (m_state)
        {
            case CMJ2HeroState.IDLE:
                break;

            case CMJ2HeroState.WALK:
                execWalk();
                break;
                
            case CMJ2HeroState.CLIMB:
            	execClimb();
            	break;

            case CMJ2HeroState.GATE:
            	execGate();
            	break;
        }

	}
	
	void OnDrawGizmos ()
	{

		if (m_lowRaycastPoint)
		{
    	    m_lowRaycastNorm = Quaternion.AngleAxis(m_lowRaycastAngle, Vector3.forward) * Vector3.right;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + new Vector3 (m_lowRaycastNorm.x, m_lowRaycastNorm.y) * m_lowRaycastDist);
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + new Vector3 (-m_lowRaycastNorm.x, m_lowRaycastNorm.y) * m_lowRaycastDist);
			
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_underRaycastPoint.position);
		}

		if (m_hiRaycastPoint)
		{
	        m_hiRaycastNorm = Quaternion.AngleAxis(m_hiRaycastAngle, Vector3.forward) * Vector3.right;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(m_hiRaycastPoint.position, m_hiRaycastPoint.position + new Vector3 (m_hiRaycastNorm.x, m_hiRaycastNorm.y) * m_hiRaycastDist);
			Gizmos.DrawLine(m_hiRaycastPoint.position, m_hiRaycastPoint.position + new Vector3 (-m_hiRaycastNorm.x, m_hiRaycastNorm.y) * m_hiRaycastDist);
			
			Gizmos.DrawLine(transform.position, transform.position + new Vector3 (0, m_centerToTopDist));
		}
	}
}
