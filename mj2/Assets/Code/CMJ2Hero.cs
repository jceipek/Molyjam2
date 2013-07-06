using UnityEngine;
using System.Collections;

public class CMJ2Hero : MonoBehaviour 
{

    public float m_targetVelocity = 4f;
    public float m_initialJumpVelocity = 6f;
    public float m_accel = 0.05f;

    public Vector3 m_debugVel;

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
    }
	
	IEnumerator Start () 
    {
    	yield return new WaitForSeconds(1f);
    	changeState(CMJ2HeroState.RIGHT);
        yield return new WaitForSeconds(1.5f);
        changeState(CMJ2HeroState.JUMP);
    }

    void jump ()
    {
    	Vector3 curvel = m_rb.velocity;
        m_rb.velocity = new Vector3 (curvel.x, m_initialJumpVelocity);
        m_state = m_prevState;
    }

    void walk (float dir)
    {
        Vector3 curvel = m_rb.velocity;
        float targetvel = dir * m_targetVelocity;

        m_rb.velocity = new Vector3 (Mathf.MoveTowards(curvel.x, targetvel, m_accel * Time.deltaTime), curvel.y);

        m_debugVel = m_rb.velocity;///
    }
	
	void FixedUpdate () 
    {
	
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
                jump();
                break;
        }

	}
}
