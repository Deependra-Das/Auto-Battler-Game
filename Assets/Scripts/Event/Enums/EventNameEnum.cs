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
        StageFailed,
        StageCleared,
        StageOver,
        RoundStarted,
        RoundOver,
        BuyLevelXP,
        XPChanged,
        LevelChanged,
        SceneLoaded,
        SelectedStageChanged,
        RoundOverSaveSnapshot
    }
}