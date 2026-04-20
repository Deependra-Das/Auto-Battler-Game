using UnityEngine;

namespace AutoBattler.Event
{
    public enum EventNameEnum
    {
        PreparationStart,
        CombatStart,
        GameOver,
        RoundOver,
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
    }
}