// Itay Keren (C) 2012
// Mouse/multitouch helper

#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
#define EMULATE_DOUBLE_FINGER
#endif

using UnityEngine;
using System.Collections;

public class CPointer : MonoBehaviour 
{
	
	public const int MAX_POINTERS = 4;
	const int GUI_LAYER = CMJ2Manager.LAYER_GUI;
	const int GUI_LAYER_MASK = 1 << GUI_LAYER;
	
	public static int numPointers = 0;
	
	public static CPointer g;
	
	public Camera guiCamera;
	float guiZOffset = -1;
	
	Collider _pressedCollider;
	
	public static Vector2[] _pos;
	//public static Vector2[] _prevPos;
	public static Vector3[] _guiPos;
	//public static Vector3[] _guiPrevPos;
	static bool[] _startedNowReal;
	static bool[] _finishedNowReal;
	static bool[] _isTouchingReal;
	static bool[] _ignoreTouch;
	static bool[] _startedNow;			// Cleared by client!
	static bool[] _finishedNow;			// Cleared by client!
	static bool[] _isTouching;			// Cleared by client! If 'finishedNow', it's still isTouching
	
	public static void clear (int t) 
	{
		_startedNow[t] = _finishedNow[t] = _isTouching[t] = false;
	}
	
#if EMULATE_DOUBLE_FINGER
	Vector2 _pseudoFingerOffset;
#endif	
	
	public static bool getTouch (int t, out bool started, out bool ended)
	{
		if (_ignoreTouch[t])
		{
			started = ended = false;
			return false;
		}
		
		bool touching = _isTouching[t];
		started = _startedNow[t];
		ended = _finishedNow[t];
		_startedNow[t] = _finishedNow[t] = _isTouching[t] = false;
		return touching;
	}

	public static bool getTouch (int t, out bool started, out bool ended, out Vector2 pos, out Vector3 gpos)
	{
		pos = _pos[t];
		gpos = _guiPos[t];

		if (_ignoreTouch[t])
		{
			started = ended = false;
			return false;
		}
		
		bool touching = _isTouching[t];
		started = _startedNow[t];
		ended = _finishedNow[t];
		_startedNow[t] = _finishedNow[t] = _isTouching[t] = false;
		
		//prevpos = _prevPos[t];
		//_prevPos[t] = _pos[t];
		
		//if (touching)
		//print("A: " +touching + " " + started + " " + ended + "    " + _isTouchingReal[t]);
		
		return touching;
	}
	
	/*public static bool getTouch (int t, out bool started, out bool ended, out Vector2 pos, out Vector2 prevpos, out Vector3 gpos, out Vector3 gprevpos)
	{
		bool touching = _isTouching[t];
		
		started = _startedNow[t];
		ended = _finishedNow[t];
		_startedNow[t] = _finishedNow[t] = _isTouching[t] = false;
		
		pos = _pos[t];
		prevpos = _prevPos[t];
		_prevPos[t] = _pos[t];
		
		gpos = _guiPos[t];
		gprevpos = _guiPrevPos[t];
		_guiPrevPos[t] = _guiPos[t];
		
		return touching;
	}*/	

	// Use this for initialization
	void Start () 
	{
		if (g != null)
		{
			Destroy(gameObject);
			return;
		}
		
		g = this;
		
		guiZOffset = -guiCamera.transform.position.z - transform.position.z;
			
		numPointers = MAX_POINTERS;
		
		_pos = new Vector2 [numPointers];
		//_prevPos = new Vector2 [numPointers];
		
		_guiPos = new Vector3 [numPointers];
		//_guiPrevPos = new Vector3 [numPointers];
		
		_isTouching = new bool [numPointers];
		_finishedNow = new bool [numPointers];
		_startedNow = new bool [numPointers];
		_ignoreTouch = new bool [numPointers];
		_isTouchingReal = new bool [numPointers];
		_finishedNowReal = new bool [numPointers];
		_startedNowReal = new bool [numPointers];
		
		for (int i = 0; i < numPointers; ++i)
		{
			_startedNowReal[i] = _finishedNowReal[i] = _isTouchingReal[i] = _ignoreTouch[i] = false;
			clear(i);
		}
	}
	
	bool testGui (int id)
	{
		bool started = _startedNowReal[id];
		bool finished = _finishedNowReal[id];
		
		if (started || finished)
		{
			//Debug.DrawLine(_guiPos[id] + new Vector3 (0, 0, -3f), _guiPos[id] + new Vector3 (0, 0, 2f), Color.red, 0.01f);
			//Debug.Break();

			RaycastHit hit;
			if (Physics.Raycast(_guiPos[id] + new Vector3 (0, 0, -3f), Vector3.forward, out hit, 5f, GUI_LAYER_MASK))
			{
				_startedNowReal[id] = _finishedNowReal[id] = _isTouchingReal[id] = false;
				clear(id);
				
				_ignoreTouch[id] = !finished;

			//	print("Hit " + hit.collider.name + " " + _startedNow[id] + " " + _finishedNow[id]);
				CCellSpriteAnimated anim = _pressedCollider == null ? null : _pressedCollider.GetComponent<CCellSpriteAnimated>();
				if ((_pressedCollider != hit.collider || finished) &&
					_pressedCollider != null)
				{
					// Stayed in same collider between hit and release
					if (finished && _pressedCollider == hit.collider)
					{
						CButtonCallback cbc = _pressedCollider.GetComponent<CButtonCallback>();
						if (cbc != null)
							cbc.SendMessage("onClick", _pressedCollider);
						else
						{
							MonoBehaviour mb = _pressedCollider.GetComponent<MonoBehaviour>();
							if (mb != null)
								mb.SendMessage("onClick", _pressedCollider);
						}
					}
					if (anim != null)
						anim.runAnimation("release");
				}
				
				if (started)
				{
					_pressedCollider = hit.collider;
					anim = _pressedCollider == null ? null : _pressedCollider.GetComponent<CCellSpriteAnimated>();
					if (anim != null)
						anim.runAnimation("press");
				}
				else
					_pressedCollider = null;

				return true;
			}
		}
		return false;
	}
	
	// Update is called once per frame
	void Update () 
	{
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID)
		_startedNowReal[0] = Input.GetMouseButtonDown(0);
		if (_startedNowReal[0])
		{
			_ignoreTouch[0] = false;
			_startedNow[0] = true;
		}		
		_finishedNowReal[0] = Input.GetMouseButtonUp(0);
		if (_finishedNowReal[0])
		{
			_ignoreTouch[0] = false;
			_finishedNow[0] = true;
		}
		
		_isTouchingReal[0] = Input.GetMouseButton(0) || _finishedNowReal[0];
		if (_isTouchingReal[0])
		{
			_isTouching[0] = true;
		
			_pos[0] = Input.mousePosition;
			_guiPos[0] = guiCamera.ScreenToWorldPoint(new Vector3 (_pos[0].x, _pos[0].y, guiZOffset));
		
			/*if (_startedNowReal[0])
			{
				_prevPos[0] = _pos[0];
				//_guiPrevPos[0] = _guiPos[0];
			}*/
		}
		//if (_isTouching[0])
		//	print("B: " +_isTouching[0] + " " + _startedNow[0] + " " + _finishedNow[0] + "    " + _isTouchingReal[0]);		
		
#if EMULATE_DOUBLE_FINGER
		_isTouchingReal[1] = _finishedNowReal[1] = _startedNowReal[1] = false;
		
		if (_isTouchingReal[0])
		{
			if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
			{
				_isTouchingReal[1] = _isTouching[1] = true;
				if ((_startedNowReal[1] = _startedNowReal[0]))
					_startedNow[1] = true;
				_finishedNowReal[1] = _finishedNow[1] = true;
				_ignoreTouch[1] = false;
			}
			else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				_isTouchingReal[1] = _isTouching[1] = true;
				if ((_startedNowReal[1] = _startedNowReal[0] || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
					_startedNow[1] = true;
				
				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (_startedNowReal[0] || Input.GetKeyDown(KeyCode.LeftShift))
					{
						_pos[1] = _pos[0];
						_pseudoFingerOffset = Vector2.zero;
					}
					else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftApple))
					{
						_pos[1] = _pos[0] + _pseudoFingerOffset;
					}
					else
					{
						_pseudoFingerOffset = _pos[1] - _pos[0];
					}
				}
				else
				{
					_pos[1] = _pos[0] + new Vector2 (0f, -75f); 
				}
				
				//_worldPos[1] = worldCamera.ScreenToWorldPoint(new Vector3 (_pos[1].x, _pos[1].y, worldZOffset));
				_guiPos[1] = guiCamera.ScreenToWorldPoint(new Vector3 (_pos[1].x, _pos[1].y, guiZOffset));
			}
		}
		
#endif // EMULATE_DOUBLE_FINGER
		
		for (int id = 0; id < 2; ++id)
			if (_startedNowReal[id] || _finishedNowReal[id])
				testGui(id);
		
#elif UNITY_IPHONE || UNITY_ANDROID		
		foreach (Touch touch in Input.touches)
		{
			int id = touch.fingerId;
			if (id >= numPointers)
				continue;
			
			_isTouchingReal[id] = _isTouching[id] = true;
			
			_startedNowReal[id] = touch.phase == TouchPhase.Began;
			_finishedNowReal[id] = touch.phase == TouchPhase.Ended;
			
			Vector2 curpos = touch.position;
			_pos[id] = curpos;
			
			_guiPos[id] = guiCamera.ScreenToWorldPoint(new Vector3 (_pos[id].x, _pos[id].y, guiZOffset));
						
			if (_startedNowReal[id] || _finishedNowReal[id])
			{
				if (_startedNowReal[id])
				{
					_ignoreTouch[id] = false;
					_startedNow[id] = true;
				}
				if (_finishedNowReal[id])
				{
					_ignoreTouch[id] = false;
					_finishedNow[id] = true;
				}
				
				// Clear this pointer
				if (!testGui(id) && _finishedNowReal[id])					
					_startedNowReal[id] = _finishedNowReal[id] = _isTouchingReal[id] = false;
			}
		}
#endif
	}
}
