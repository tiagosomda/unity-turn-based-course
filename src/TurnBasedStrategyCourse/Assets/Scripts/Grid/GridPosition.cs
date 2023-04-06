using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return $"x: {x}; z: {z}";
    }

    public static bool operator ==(GridPosition left, GridPosition right)
    {
        return left.x == right.x && left.z == right.z;
    }

    public static bool operator !=(GridPosition left, GridPosition right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public static GridPosition operator +(GridPosition left, GridPosition right)
    {
        return new GridPosition(left.x + right.x, left.z + right.z);
    }

    public static GridPosition operator -(GridPosition left, GridPosition right)
    {
        return new GridPosition(left.x - right.x, left.z - right.z);
    }
}