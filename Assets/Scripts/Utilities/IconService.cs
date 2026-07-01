using System.Collections.Generic;
using UnityEngine;

public class IconService
{
    private readonly Dictionary<UnitElementEnum, Sprite> _elementIconDictionary;
    private readonly Dictionary<UnitFactionEnum, Sprite> _factionIconDictionary;
    private readonly Dictionary<UnitTypeEnum, Sprite> _unitTypeIconDictionary;
    private readonly Dictionary<BuffNameEnum, Sprite> _buffIconDictionary;

    public IconService(IconScriptableObjectScript icon_SO)
    {
        _elementIconDictionary = new Dictionary<UnitElementEnum, Sprite>();
        _factionIconDictionary = new Dictionary<UnitFactionEnum, Sprite>();
        _unitTypeIconDictionary = new Dictionary<UnitTypeEnum, Sprite>();
        _buffIconDictionary = new Dictionary<BuffNameEnum, Sprite>();

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
            if (icon_SO.buffIconList != null)
            {
                PopulateBuffIcons(icon_SO.buffIconList);
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

    private void PopulateBuffIcons(List<BuffIconEntry> buffIconList)
    {
        foreach (var entry in buffIconList)
        {
            _buffIconDictionary[entry.buffName] = entry.buffIcon;
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

    public Sprite GetBuffIcon(BuffNameEnum buffName)
    {
        return _buffIconDictionary[buffName];
    }
}
