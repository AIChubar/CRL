using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Modifiable character stats.
/// </summary>
[Serializable]
public class CharacterStat
{
    
    public float BaseValue;
    protected  float lastBaseValue = float.MinValue;
 
    protected  readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    
    protected  bool isDirty = true;
    protected  float _value;
    
    /// <summary>
    /// Current modified stat value.
    /// </summary>
    public virtual float Value {
        get {
            if(isDirty || lastBaseValue != BaseValue) {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue(); // This line is missing in your code
                isDirty = false;
            }
            return _value;
        }
    }
    public virtual int ValueInt
    {
        get
        {
            return (int)Math.Ceiling(Value); // Round up to the nearest integer
        }
    }
    
    public float GetValue(bool includeTemporary)
    {
        if (!includeTemporary)
            return CalculateFinalValue(ignoreTemporary: true);

        return Value;
    }

    
    public int GetValueInt(bool includeTemporary)
    {
        return (int)Math.Ceiling(GetValue(includeTemporary));
    }

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
 
    public CharacterStat(float baseValue) : this()
    {
        BaseValue = baseValue;
        _value = baseValue;
    }
    
    /// <summary>
    /// Modifiable character stats.
    /// </summary>
    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }
    
    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        
        return false;
        
    }
    
    public virtual bool RemoveAllModifiersFromStat(StatType stat)
    {
        bool didRemove = false;
 
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].StatType == stat)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }
 
// Add this method to the CharacterStat class
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }
    
    public void RemoveTemporaryModifiers()
    {
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].IsTemporary)
            {
                statModifiers.RemoveAt(i);
                isDirty = true;
            }
        }
    }
    
    protected virtual float CalculateFinalValue(bool ignoreTemporary = false)
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        foreach (var mod in statModifiers)
        {
            if (ignoreTemporary && mod.IsTemporary)
                continue;

            if (mod.StatModType == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.StatModType == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;
                int modIndex = statModifiers.IndexOf(mod);
                if (modIndex + 1 >= statModifiers.Count || statModifiers[modIndex + 1].StatModType != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.StatModType == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }

}
