using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSymbol", menuName = "Symbol")]
public class Symbol : ScriptableObject, IEquatable<Symbol>
{
    public string symbolName;
    public float probability;
    public Sprite sprite;
    public string ch;
    public bool isWild;

    public bool Equals(Symbol other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ch == other.ch || isWild || other.isWild;
    }

    public static bool operator ==(Symbol obj1, Symbol obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (obj1 is null || obj2 is null)
            return false;
        return obj1.Equals(obj2);
    }

    public static bool operator !=(Symbol obj1, Symbol obj2)
    {
        return !(obj1 == obj2);
    }

    public override bool Equals(object obj)
    {
        if (obj is Symbol otherSymbol)
        {
            return Equals(otherSymbol);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (ch != null ? ch.GetHashCode() : 0);
    }
}
