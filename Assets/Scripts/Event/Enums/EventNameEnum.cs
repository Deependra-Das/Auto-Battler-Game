using UnityEngine;

namespace AutoBattler.Event
{
    public enum EventNameEnum
    {
        GameplayStateChanged,
        UnitDragged,
        InventoryUnitCardDragged,
        HighlightInventoryPanel,
        HighlightDiscardPanel,
        UnitAddedOnField,
        UnitRemovedFromField,
        TeamBuffUpdated,
        StageStarted,
        RoundStarted,
        RoundEnded,
        StageCleared,
        BuyLevelXP,
        XPChanged,
        LevelChanged,
    }
}