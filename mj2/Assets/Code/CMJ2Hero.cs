using UnityEngine;
using System.Collections;

public class CMJ2Hero : MonoBehaviour 
{

    public float m_targetVelocity = 4f;
    public float m_initialJumpVelocity = 6f;
    public float m_accel = 0.05f;

    public Vector3 m_debugVel;
    
    public /**/ bool m_onGround = false;
    public /**/ int m_wayUp = 0;
    
    public enum CMJ2HeroState {
        IDLE = 0,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        JUMP
    };
    public CMJ2HeroState m_state;
    CMJ2HeroState m_prevState = CMJ2HeroState.IDLE;

	public Transform m_lowRaycastPoint;
	public float m_onGroundDist = 0.505f;

    protected Transform m_xform;
    protected Rigidbody m_rb;

    void Awake ()
    {
        m_xform = transform;
        m_rb = rigidbody;
    }
    
    void changeState (CMJ2HeroState state)
    {
    	m_prevState = m_state;
    	m_state = state;
    	print(m_prevState + "->" + m_state + " " + Time.time);
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
    }

    void walk (float dir)
    {
    	RaycastHit hitinfo;
    	int hitlayer = -1;
    	if (Physics.Raycast(m_lowRaycastPoint.position, new Vector3 (dir * 0.7071f, -0.7071f), out hitinfo, 1f))
    		hitlayer = hitinfo.collider.gameObject.layer;
    		
    	if (hitlayer != CMJ2Manager.LAYER_GROUND)
    		jump();
    		    	
        Vector3 curvel = m_rb.velocity;
        float targetvel = dir * m_targetVelocity;

        m_rb.velocity = new Vector3 (Mathf.MoveTowards(curvel.x, targetvel, m_accel * Time.deltaTime), curvel.y);

        m_debugVel = m_rb.velocity;///
    }
    
    void jump ()
    {
    	if (m_onGround && m_wayUp < 1)
	    	changeState(CMJ2HeroState.JUMP);
	    else
	    	print("Not jumping " + m_onGround + " " + m_wayUp);
    }
	
	void OnTriggerEnterExt (CCollisionDelegate.CTwoColliders cols)
	{
		print("Trigger " + cols.mine.name + " " + cols.other.name);
		
		int lay = cols.other.gameObject.layer;
		bool isjump = lay == CMJ2Manager.LAYER_GROUND ||
					  lay == CMJ2Manager.LAYER_SPIKE;
					  
		print(lay + " " + m_state);
		
		switch (cols.mine.name)
		{
			case "leftFloor":
				if (m_state == CMJ2HeroState.LEFT)
				{
					if (isjump)
						jump();
				}
				break;
			case "rightFloor":
				if (m_state == CMJ2HeroState.RIGHT)
				{
					if (isjump)
						jump();
				}
				break;
		}
	}

	void FixedUpdate () 
    {
    	RaycastHit hitinfo;
    	int hitlayer = -1;
    	if (Physics.Raycast(m_lowRaycastPoint.position, Vector3.down, out hitinfo, m_onGroundDist))
    		hitlayer = hitinfo.collider.gameObject.layer;
    	
    	float vely = m_rb.velocity.y;
    	int newwayup = vely > 1f ? 1 : vely < -1f ? -1 : 0;
    	if (m_wayUp > 0 && newwayup < 0)
    		Debug.DrawLine(m_lowRaycastPoint.position + new Vector3 (-1f, -0.5f), m_lowRaycastPoint.position + new Vector3 (1, -0.5f), Color.blue, 10f);
    	if (newwayup != 0)
	    	m_wayUp = newwayup;
    		
    	m_onGround = hitlayer == CMJ2Manager.LAYER_GROUND;
    	//if (m_onGround)
    	//	Debug.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + Vector3.forward, Color.blue, 10f);
    		
        switch (m_state)
        {
            case CMJ2HeroState.IDLE:
                break;

            case CMJ2HeroState.RIGHT:
                walk(1f);
                break;
            case CMJ2HeroState.LEFT:
                walk(-1f);
                break;

            case CMJ2HeroState.JUMP:
                execJump();
                break;
        }

	}
	
	void OnDrawGizmos ()
	{

		if (m_lowRaycastPoint)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + new Vector3 (0.7071f, -0.7071f));
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + new Vector3 (-0.7071f, -0.7071f));
			
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(m_lowRaycastPoint.position, m_lowRaycastPoint.position + new Vector3 (0, -m_onGroundDist));
		}
	}
}
