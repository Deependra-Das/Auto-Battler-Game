using UnityEngine;

namespace AutoBattler.Event
{
    public enum EventNameEnum
    {
        GameplayStateChanged,
        UnitDragged,
        ToggleDiscardDropZone,
        InventoryUnitCardDiscarded,
        ReorderInventoryLayout,
        HighlightInventoryPanel,
        HighlightDiscardPanel,
        UnitAddedOnField,
        UnitRemovedFromField,
        TeamBuffUpdated,
        StageStarted,
        StageFailed,
        StageClearedFull,
        StageClearedPartial,
        RoundStarted,
        RoundOver,
        BuyLevelXP,
        XPChanged,
        LevelChanged,
        SceneLoaded,
        SelectedStageChanged,
        GameplayPaused,
        GameplayResumed,
    }
}