using UnityEngine;

namespace AutoBattler.Event
{
    public enum EventNameEnum
    {
        GameplayStateChanged,
        ToggleInventoryDropZone,
        ToggleDiscardDropZone,
        InventoryUnitCardDiscarded,
        ReorderInventoryLayout,
        UnitAddedOnField,
        UnitRemovedFromField,
        FieldUnitsUpdated,
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