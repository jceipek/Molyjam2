using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CCellSpriteDynMultiQuad : CCellSprite 
{	

	[HideInInspector]
	public int m_quadsHoriz = 1;
	[HideInInspector]
	public int m_quadsVert = 1;

	[HideInInspector]
	public bool m_optimizeRectangles = true;

	[HideInInspector]
	public bool[] m_sqOnOff;
	int m_checksum;
	int m_prevChecksum = -1;

	bool[] m_sqAvail;

	List<Vector2> m_rawVertices;

	protected override void setMesh (bool mesh_changed)
	{
		m_numCells = new Vector2 ((float)m_atlasSize.x / m_cellSize.x,
		                          (float)m_atlasSize.y / m_cellSize.y);

		if (m_quadsHoriz != Mathf.RoundToInt(m_cellSpan.x) ||
		    m_quadsVert != Mathf.RoundToInt(m_cellSpan.y))
		{
			m_quadsHoriz = Mathf.RoundToInt(m_cellSpan.x);
			m_quadsVert = Mathf.RoundToInt(m_cellSpan.y);
			mesh_changed = true;
		}

		//m_mesh.colors = null;
		//m_mesh.uv = null;
		
		if (mesh_changed)
		{
			if (m_sqOnOff == null || m_sqOnOff.Length != m_quadsVert * m_quadsHoriz)
			{
				m_sqOnOff = new bool[m_quadsVert * m_quadsHoriz];
				setAllSquares(true);
			}
			else if (updateChecksum() != m_prevChecksum)
				apply();
		}

		if (m_uv == null)
		{
			m_uv = new Vector2 [m_mesh.vertexCount];
			setCell(m_currentCell);		
		}
	}

	public void setSquareOnOff (int x, int y, bool onoff)
	{
		m_sqOnOff[y * m_quadsHoriz + x] = onoff;
		if (updateChecksum() != m_prevChecksum)
			apply();
	}

	public void setOptimized (bool opt)
	{
		m_optimizeRectangles = opt;
		m_prevChecksum = -1;
		apply();
	}

	int updateChecksum ()
	{
		m_checksum = 0;
		for (int y = 0; y < m_quadsVert; ++y)
			for (int x = 0; x < m_quadsHoriz; ++x)
				if (m_sqOnOff[y * m_quadsHoriz + x])
					++m_checksum;
		return m_checksum;
	}

	public void setAllSquares (bool onoff)
	{
		for (int y = 0; y < m_quadsVert; ++y)
			for (int x = 0; x < m_quadsHoriz; ++x)
				m_sqOnOff[y * m_quadsHoriz + x] = onoff;

		m_prevChecksum = updateChecksum();
		apply();
	}

	public override void setCell (int cell_input)
	{
		if (m_mesh != null)
		{
			Vector3 off = m_centered ? 
				m_offset - new Vector3 (m_scale.x * m_cellSpan.x * 0.5f, m_scale.y * m_cellSpan.y * 0.5f): 
					m_offset; // Top left

			Vector2 cell = new Vector2 (cell_input % m_numCells.y,
			                            m_numCells.y - Mathf.Floor(cell_input / m_numCells.y) - m_cellSpan.y);
			Vector2 pixelh = Vector2.one * (0.5f / m_atlasSize.x);

			int numv = m_mesh.vertexCount;
			for (int i = 0; i < numv; ++i)
			{
				Vector2 rawv = new Vector2 ((m_mesh.vertices[i].x - off.x) / (m_scale.x * m_cellSpan.x),
				                            (m_mesh.vertices[i].y - off.y) / (m_scale.y * m_cellSpan.y));
				m_uv[i] = new Vector2 (
					Mathf.Lerp(cell.x / m_numCells.x + pixelh.x, (cell.x + m_cellSpan.x) / m_numCells.x - pixelh.x, rawv.x),
					Mathf.Lerp(cell.y / m_numCells.y + pixelh.y, (cell.y + m_cellSpan.y) / m_numCells.y - pixelh.y, rawv.y));
			}
			m_mesh.uv = m_uv;
		}
		
		m_currentCell = cell_input;
	}

	class CDynRect {
		public CDynRect (int tx, int ty, int w, int h) {
			topx = tx; 
			topy = ty;
			width = w;
			height = h;
		}
		public int topx;
		public int topy;
		public int width;
		public int height;

		public override string ToString()
		{
			return "x:"+ topx +",y:"+topy+"  w:"+width+",h:"+height;
		}
	}
	
	bool isRectAvail (int xx, int yy, int dirx, int diry, int width, int height)
	{
		for (int j = 0, yyi = yy; j < height; yyi += diry, ++j)
			for (int i = 0, xxi = xx; i < width; xxi += dirx, ++i)
		{
				//print(xxi + " " + yyi + " " + width + " " + height);
				if (!m_sqAvail[yyi * m_quadsHoriz + xxi])
					return false;
		}
		return true;
	}

	CDynRect findBiggestRectAtCorner (int xx, int yy, int dirx, int diry)
	{
		if (!m_sqAvail[yy * m_quadsHoriz + xx])
			return null;

		int maxwidth = dirx > 0 ? (m_quadsHoriz - xx) : (xx + 1);
		int maxheight = diry > 0 ? (m_quadsVert - yy) : (yy + 1);

		int totalsq_found = 0;
		int maxwidth_found = 0;
		int maxheight_found = 0;
		for (int height = maxheight; height >= 1; --height)
		{
			if (height * maxwidth < totalsq_found)
				break;
			for (int width = maxwidth; width >= 1; --width)
			{
				if (height * width < totalsq_found)
					break;

				if (isRectAvail(xx, yy, dirx, diry, width, height))
				{
					totalsq_found = width * height;
					maxheight_found = height;
					maxwidth_found = width;
				}
			}
		}
		if (totalsq_found > 0)
			return new CDynRect (dirx > 0 ? xx : (xx - maxwidth_found + 1),
			                     diry > 0 ? yy : (yy - maxheight_found + 1),
			                     maxwidth_found,
			                     maxheight_found);
		return null;
	}

	CDynRect scanCornersAndFindBiggestRect ()
	{
		CDynRect curbiggest = null;
		for (int j = 0; j < m_quadsVert; ++j)
		{
			for (int i = 0; i < m_quadsHoriz; ++i)
			{
				if (!m_sqAvail[j * m_quadsHoriz + i])
					continue;

				if (!m_optimizeRectangles)
					return new CDynRect (i, j, 1, 1);

				bool left_bl = i == 0 || !m_sqAvail[j * m_quadsHoriz + i-1];
				bool right_bl = i == (m_quadsHoriz - 1) || !m_sqAvail[j * m_quadsHoriz + i+1];
				bool leftright_bl = left_bl || right_bl;
				bool top_bl = j == 0 || !m_sqAvail[(j-1) * m_quadsHoriz + i];
				bool bot_bl = j == (m_quadsVert - 1) || !m_sqAvail[(j+1) * m_quadsHoriz + i];
				bool topbot_bl = top_bl || bot_bl;
				if (leftright_bl && topbot_bl)
				{
					CDynRect biggest = findBiggestRectAtCorner(i, j, left_bl ? 1 : -1, top_bl ? 1 : -1);
					if (curbiggest == null ||
					    biggest.width * biggest.height > curbiggest.width * curbiggest.height)
					{
						curbiggest = biggest;
						//print(curbiggest + " " + i + " " + j);
					}
				}
			}
		}
		return curbiggest;
	}

	public void apply ()
	{
		m_prevChecksum = m_checksum;
		m_uv = null;

		Vector3 off = m_centered ? 
			m_offset - new Vector3 (m_scale.x * m_cellSpan.x * 0.5f, m_scale.y * m_cellSpan.y * 0.5f): 
				m_offset; // Top left
		fillWithRects(off);
	}

	int fillWithRects (Vector3 off)
	{
		m_sqAvail = new bool[m_quadsVert * m_quadsHoriz];
		//print(m_quadsHoriz + " " + m_quadsVert + " " + m_sqAvail.Length + " " + m_sqOnOff.Length);
		m_mesh.triangles = null;
		m_mesh.vertices = null;
		m_mesh.normals = null;
		m_mesh.uv = null;

		for (int j = 0; j < m_quadsVert; ++j)
			for (int i = 0; i < m_quadsHoriz; ++i)
				m_sqAvail[j * m_quadsHoriz + i] = m_sqOnOff[j * m_quadsHoriz + i];

		CDynRect rect;

		m_rawVertices = new List<Vector2> ();
		List<int> tris = new List<int> ();

		Vector3 eu = m_xform.eulerAngles;
		float angx = (eu.x + 3600) % 360;
		float angy = (eu.y + 3600) % 360;
		bool flipped = (angx > 90f && angx < 270f) ^ 
			(angy > 90f && angy < 270f);

		while ((rect = scanCornersAndFindBiggestRect()) != null)
		{
			for (int j = 0; j < rect.height; ++j)
			{
				for (int i = 0; i < rect.width; ++i)
				{
					//print(rect.topx + "+" + i + "," + rect.topy + "+" + j);
					m_sqAvail[(rect.topy + j) * m_quadsHoriz + rect.topx + i] = false;
				}
			}

			float x = (float)rect.topx;
			float y = (float)rect.topy;
			Vector2 vt = new Vector2 (x / m_quadsHoriz, y / m_quadsVert);
			int idxTL = m_rawVertices.IndexOf(vt);
			if (idxTL < 0)
			{
				idxTL = m_rawVertices.Count;
				m_rawVertices.Add(vt);
			}

			x = (float)(rect.topx + rect.width);
			y = (float)rect.topy;
			vt = new Vector2 (x / m_quadsHoriz, y / m_quadsVert);
			int idxTR = m_rawVertices.IndexOf(vt);
			if (idxTR < 0)
			{
				idxTR = m_rawVertices.Count;
				m_rawVertices.Add(vt);
			}

			x = (float)(rect.topx + rect.width);
			y = (float)(rect.topy + rect.height);
			vt = new Vector2 (x / m_quadsHoriz, y / m_quadsVert);
			int idxBR = m_rawVertices.IndexOf(vt);
			if (idxBR < 0)
			{
				idxBR = m_rawVertices.Count;
				m_rawVertices.Add(vt);
			}

			x = (float)rect.topx;
			y = (float)(rect.topy + rect.height);
			vt = new Vector2 (x / m_quadsHoriz, y / m_quadsVert);
			int idxBL = m_rawVertices.IndexOf(vt);
			if (idxBL < 0)
			{
				idxBL = m_rawVertices.Count;
				m_rawVertices.Add(vt);
			}

			if (!flipped)
			{
				tris.Add(idxTL);
				tris.Add(idxTR);
				tris.Add(idxBR);

				tris.Add(idxTL);
				tris.Add(idxBR);
				tris.Add(idxBL);
			}
			else
			{
				tris.Add(idxTL);
				tris.Add(idxBR);
				tris.Add(idxTR);

				tris.Add(idxTL);
				tris.Add(idxBL);
				tris.Add(idxBR);
			}
		}
		int numv = m_rawVertices.Count;

		Vector3[] norms = new Vector3 [numv];
		Vector3[] vertices = new Vector3 [numv];
		Color[] cs = new Color [numv];
		for (int i = 0; i < numv; ++i)
		{
			vertices[i] = off + new Vector3 (
				Mathf.Lerp(-OVERLAY_EPSILON, m_scale.x * m_cellSpan.x + OVERLAY_EPSILON, m_rawVertices[i].x),
				Mathf.Lerp(-OVERLAY_EPSILON, m_scale.y * m_cellSpan.y + OVERLAY_EPSILON, 1f - m_rawVertices[i].y));
			norms[i] = Vector3.back;
			cs[i] = Color.grey;
		}
		m_mesh.vertices = vertices;
		m_mesh.triangles = tris.ToArray();
		m_mesh.normals = norms;
		m_mesh.colors = cs;

		return numv;
	}
	
}

