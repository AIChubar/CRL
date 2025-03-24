using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSymbol", menuName = "Symbol")]

public class Symbol  : ScriptableObject, IEquatable<Symbol>
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
        return ch == other.ch || isWild;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Symbol)obj);
    }

    public override int GetHashCode()
    {
        return (ch != null ? ch.GetHashCode() : 0);
    }
}