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
    
    public /**/ bool m_onGround = false;
    public /**/ int m_wayUp = 0;
    
    public enum CMJ2HeroState {
        IDLE = 0,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        JUMP,
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
    }
    
    void changeState (CMJ2HeroState state)
    {
    	
    	if (m_state != state)
    	{
	    	m_prevState = m_state;
    		m_state = state;
    	   	if (m_state == CMJ2HeroState.UP ||
    	   		m_state == CMJ2HeroState.DOWN)
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
    	yield return new WaitForSeconds(1f);
    	changeState(CMJ2HeroState.RIGHT);
        //yield return new WaitForSeconds(1.5f);
        //changeState(CMJ2HeroState.JUMP);
    }

    void execJump ()
    {
    	Vector3 curvel = m_rb.velocity;
        m_rb.velocity = new Vector3 (curvel.x, m_initialJumpVelocity);
        m_state = m_prevState;
        m_lastJumpTime = Time.time;
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
    
    void execClimb (float vdir)
    {
    	float xdistfromcenter = (m_xform.position.x + 1000f) % 1f;
    	if (xdistfromcenter > 0.5f)
    		xdistfromcenter -= 1f;
    	//print(m_xform.position.x + " " + xdistfromcenter);
    	m_xform.position += new Vector3 (
    		Mathf.Abs(xdistfromcenter) < m_ladderSnapSpeed ? -xdistfromcenter :
    			(Mathf.Sign(xdistfromcenter) * -m_ladderSnapSpeed), 
    			m_climbSpeed * vdir * Time.deltaTime);    	
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
    
    void jump (bool bumped)
    {
    	if (m_onGround && Time.time > m_lastJumpTime + 0.1f)
    	{
    		int hitlayer = -1;
 			RaycastHit hitinfo;

    		if (!bumped)
    		{
  		    	if (Physics.Raycast(m_xform.position, Vector3.up, out hitinfo, m_centerToTopDist))
  					hitlayer = hitinfo.collider.gameObject.layer;

 		    	if (hitlayer != CMJ2Manager.LAYER_GROUND &&
					hitlayer != CMJ2Manager.LAYER_SPIKE)
			    	changeState(CMJ2HeroState.JUMP);
    		}
    		else
    		{
		    	if (Physics.Raycast(m_xform.position, Vector3.up, out hitinfo, m_centerToTopDist) ||
		    		Physics.Raycast(m_hiRaycastPoint.position, new Vector3 (m_dir * m_hiRaycastNorm.x, m_hiRaycastNorm.y), out hitinfo, m_hiRaycastDist))
		    	{
		    		hitlayer = hitinfo.collider.gameObject.layer;
		    		Debug.DrawLine(m_hiRaycastPoint.position, hitinfo.point, Color.white, 1f);
		    	}
		    
		    	if (hitlayer != CMJ2Manager.LAYER_GROUND &&
					hitlayer != CMJ2Manager.LAYER_SPIKE)
			    	changeState(CMJ2HeroState.JUMP);
			    else
			    	// Bump backwards
			    	changeState(m_dir > 0f ? CMJ2HeroState.LEFT : CMJ2HeroState.RIGHT);
    		}
    	}
	    //else
	    //	print("Not jumping " + m_onGround + " " + (Time.time - m_lastJumpTime));
    }
    
    void setDir (float dir)
    {
    	if (dir == m_dir)
    		return;
    		
    	m_dir = dir;
    	m_sprite.localRotation = m_dir >= 0f ? Quaternion.identity : m_quatFlipped;
    }
	
	/*void OnTriggerStay (Collider col)
	{
		print("MyTriggerStay " + col.name);
		
		changeState(CMJ2HeroState.UP);
	}*/
	
	void OnTriggerStayExt (CCollisionDelegate.CTwoColliders cols)
	{
		//print("Trigger " + cols.mine.name + " " + cols.other.name);
		
		int lay = cols.other.gameObject.layer;
		bool isjump = lay == CMJ2Manager.LAYER_GROUND ||
					  lay == CMJ2Manager.LAYER_SPIKE;
					  		
		switch (cols.mine.name)
		{
			case "leftFloor":
				if (m_state == CMJ2HeroState.LEFT)
				{
					if (isjump)
						jump(true);
				}
				break;
			case "rightFloor":
				if (m_state == CMJ2HeroState.RIGHT)
				{
					if (isjump)
						jump(true);
				}
				break;
		}
	}

	void FixedUpdate () 
    {
    	if (m_inGate == null)
		{
	    	RaycastHit hitinfo;
	    	int hitlayer = -1;
	    	if (Physics.Raycast(m_underRaycastPoint.position + new Vector3 (0f, 0f, -4f), Vector3.forward, out hitinfo, 5f, CMJ2Manager.MASK_ALL_GROUND))
	    		hitlayer = hitinfo.collider.gameObject.layer;
	    	m_onGround = hitlayer == CMJ2Manager.LAYER_GROUND;
	
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
					if (m_state != CMJ2HeroState.UP &&
						m_state != CMJ2HeroState.DOWN)
						changeState(CMJ2HeroState.UP);
				}
			}
			else
			{
				if (!m_onGround && 
					(m_state == CMJ2HeroState.UP ||
					 m_state == CMJ2HeroState.DOWN))
					 changeState(m_dir > 0f ? CMJ2HeroState.RIGHT : CMJ2HeroState.LEFT);
			}
	    	
	    	float vely = m_rb.velocity.y;
	    	int newwayup = vely > 1f ? 1 : vely < -1f ? -1 : 0;
	    	
	    	//// DEBUG
	    	if (m_wayUp > 0 && newwayup < 0)
	    		Debug.DrawLine(m_lowRaycastPoint.position + new Vector3 (-1f, -0.5f), m_lowRaycastPoint.position + new Vector3 (1, -0.5f), Color.blue, 10f);
	    	if (newwayup != 0)
		    	m_wayUp = newwayup;
		}
		
        switch (m_state)
        {
            case CMJ2HeroState.IDLE:
                break;

            case CMJ2HeroState.RIGHT:
            	setDir(1f);
                execWalk();
                break;
            case CMJ2HeroState.LEFT:
            	setDir(-1f);
                execWalk();
                break;
                
            case CMJ2HeroState.UP:
            	execClimb(1f);
            	break;
            case CMJ2HeroState.DOWN:
            	execClimb(-1f);
            	break;

            case CMJ2HeroState.JUMP:
                execJump();
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
