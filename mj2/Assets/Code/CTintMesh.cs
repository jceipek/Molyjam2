using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Tint/Tint Mesh")]
public class CTintMesh : MonoBehaviour 
{
	public bool m_globalPosition = false;

	//[HideInInspector]
	public bool m_gradient = false;
	//[HideInInspector]
	public Color m_mainTopLeft = Color.gray;
	//[HideInInspector]
	public Color m_topRight = Color.gray;
	//[HideInInspector]
	public Color m_bottomLeft = Color.gray;
	//[HideInInspector]
	public Color m_bottomRight = Color.gray;
	
	//[HideInInspector]
	public Vector2 m_gradientOffset = Vector2.zero;

	// Use this for initialization
	void Start () 
	{
		tintCell();
	}

	public void tintCell ()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		if (!mf)
			return;

		Mesh mesh = mf.sharedMesh;
		int numv = mesh.vertexCount;
		Vector3[] vt = mesh.vertices;

		Color[] cs = new Color [numv];
		if (!m_gradient)
		{
			for (int i = 0; i < numv; ++i)
				cs[i] = m_mainTopLeft;
			mesh.colors = cs;
			return;
		}

		Vector2[] vt_rel = new Vector2 [numv]; 
		float left = float.MaxValue;
		float right = float.MinValue;
		float top = float.MinValue;
		float bottom = float.MaxValue;

		Transform xf = transform;

		for (int i = 0; i < numv; ++i)
		{
			Vector2 v = vt_rel[i] = (Vector2) (m_globalPosition ?  xf.TransformPoint(vt[i]) : vt[i]);

			if (v.x < left)
				left = v.x;
			if (v.x > right)
				right = v.x;
			if (v.y < bottom)
				bottom = v.y;
			if (v.y > top)
				top = v.y;
		}
		for (int i = 0; i < numv; ++i)
		{
			Vector2 v = vt_rel[i];
			Vector2 lerp = new Vector2 (Mathf.Clamp01((v.x - left + m_gradientOffset.x) / (right - left)),
			                            Mathf.Clamp01((v.y - bottom + m_gradientOffset.y) / (top - bottom)));
			cs[i] = Color.Lerp(Color.Lerp(m_bottomLeft, m_bottomRight, lerp.x), Color.Lerp(m_mainTopLeft, m_topRight, lerp.x), lerp.y);
		}
		mesh.colors = cs;
	}

#if UNITY_EDITOR
	protected virtual void Update ()
	{
		// Only run in editing mode
		if (Application.isPlaying)
			return;

		tintCell();
	}
#endif
}
