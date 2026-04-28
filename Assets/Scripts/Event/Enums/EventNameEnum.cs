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
        RoundWon,
        RoundLose,
        StageCleared,
        BuyLevelXP,
        XPChanged,
        LevelChanged,
    }
}