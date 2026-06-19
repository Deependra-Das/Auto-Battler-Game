using System.Collections.Generic;
using UnityEngine;

public class UnitColorService
{
    private readonly Dictionary<UnitElementEnum, Color> _colors;
    private readonly Color _healingColor;
    private readonly Color _healthDamageColor;
    private readonly Color _shieldDamageColor;

    public UnitColorService(UnitColorScriptableObjectScript config)
    {
        _colors = new Dictionary<UnitElementEnum, Color>();

        foreach (var entry in config.unitElementColorEntryList)
        {
            _colors[entry.element] = entry.color;
        }

        _healingColor = config.healingColor;
        _healthDamageColor = config.healthDamageColor;
        _shieldDamageColor = config.shieldDamageColor;
    }

    public Color GetElementColor(UnitElementEnum type)
    {
        return _colors.TryGetValue(type, out var color) ? color : Color.white;
    }

    public Color GetHealingColor()
    {
        return _healingColor;
    }

    public Color GeHealthDamageColor()
    {
        return _healthDamageColor;
    }

    public Color GetShieldDamageColor()
    {
        return _shieldDamageColor;
    }
}
