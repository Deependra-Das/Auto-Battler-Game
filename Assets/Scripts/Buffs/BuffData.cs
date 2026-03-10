using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct BuffData
{
    public BuffNameEnum buffName;
    public BuffTypeEnum buffType;
    public Image buffImage;
    public BuffParticipantTier[] buffParticipantTierList;
    public string buffDescription;
}
