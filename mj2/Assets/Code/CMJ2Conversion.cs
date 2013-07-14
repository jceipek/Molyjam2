using UnityEngine;
using System;

public struct Cell : IEquatable<Cell>
{
	private int m_x;
	private int m_y;
	public Cell	(int x, int y)
	{
		m_x = x;
		m_y = y;
	}

	public int X { get { return m_x; } }
    public int Y { get { return m_y; } }

	public override int GetHashCode()
    {
        return m_x.GetHashCode() ^ m_y.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((Cell)obj);
    }

    public bool Equals(Cell other)
    {
        return other.X.Equals(m_x) && other.Y.Equals(m_y);
    }

    public static Cell operator +(Cell a, Cell b)
    {
        return new Cell(a.X + b.X, a.Y + b.Y);
    }

    public static Cell operator -(Cell a, Cell b)
    {
        return new Cell(a.X - b.X, a.Y - b.Y);
    }

    // unary minus
    public static Cell operator -(Cell a)
    {
        return new Cell(-a.X, -a.Y);
    }
}

public class CMJ2Conversion : MonoBehaviour
{

}