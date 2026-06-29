using System.Collections.Generic;
using UnityEngine;

public class UnitIconService
{
    private readonly Dictionary<UnitElementEnum, Sprite> _elementIconDictionary;
    private readonly Dictionary<UnitFactionEnum, Sprite> _factionIconDictionary;
    private readonly Dictionary<UnitTypeEnum, Sprite> _unitTypeIconDictionary;

    public UnitIconService(UnitIconScriptableObjectScript icon_SO)
    {
        _elementIconDictionary = new Dictionary<UnitElementEnum, Sprite>();
        _factionIconDictionary = new Dictionary<UnitFactionEnum, Sprite>();
        _unitTypeIconDictionary = new Dictionary<UnitTypeEnum, Sprite>();

        if (icon_SO != null)
        {
            if (icon_SO.elementIconList != null)
            {
                PopulateElementIcons(icon_SO.elementIconList);
            }
            if (icon_SO.factionIconList != null)
            {
                PopulateFactionIcons(icon_SO.factionIconList);
            }
            if (icon_SO.unitTypeIconList != null)
            {
                PopulateUnitTypeIcons(icon_SO.unitTypeIconList);
            }
        }
    }

    private void PopulateElementIcons(List<UnitElementIconEntry> elementIconList)
    {
        foreach (var entry in elementIconList)
        {
            _elementIconDictionary[entry.element] = entry.elementIcon;
        }
    }

    private void PopulateFactionIcons(List<UnitFactionIconEntry> factionIconList)
    {
        foreach (var entry in factionIconList)
        {
            _factionIconDictionary[entry.faction] = entry.factionIcon;
        }
    }

    private void PopulateUnitTypeIcons(List<UnitTypeIconEntry> unitTypeIconList)
    {
        foreach (var entry in unitTypeIconList)
        {
            _unitTypeIconDictionary[entry.unitType] = entry.unitTypeIcon;
        }
    }

    public Sprite GetElementIcon(UnitElementEnum element)
    {
        return _elementIconDictionary[element];
    }

    public Sprite GetFactionIcon(UnitFactionEnum faction)
    {
        return _factionIconDictionary[faction];
    }

    public Sprite GetUnitTypeIcon(UnitTypeEnum unitType)
    {
        return _unitTypeIconDictionary[unitType];
    }
}
