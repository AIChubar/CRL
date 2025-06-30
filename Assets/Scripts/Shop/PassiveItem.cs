using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItem", menuName = "Item/PassiveItem")]
public class PassiveItem : Item
{    
    public List<StatModifier> statModifiers;

    public override string GetDescription()
    {
        if (statModifiers == null || statModifiers.Count == 0)
            return "No effects.";

        var descriptions = new List<string>();
        foreach (var mod in statModifiers)
        {
            string valueStr = mod.StatModType switch
            {
                StatModType.Flat => $"{mod.Value:+0.00;-0.00;+0.00}",
                StatModType.PercentAdd => $"{mod.Value:+0.00%;-0.00%;+0.00%}",
                StatModType.PercentMult => $"{mod.Value:+0.00%;-0.00%;+0.00%} Mult",
                _ => $"{mod.Value:0.00}"
            };

            if (mod.StatType == StatType.ColumnBuff)
            {
                string columnInfo = mod.IsColumnSpecific ? $" (Col {mod.ColumnIndex + 1})" : "";
                descriptions.Add($"Change Column {valueStr}{columnInfo}");
            }
            else
            {
                descriptions.Add($"{mod.StatType} {valueStr}");
            }
        }

        return string.Join("\n", descriptions);
    }

}