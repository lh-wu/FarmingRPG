using System;
using UnityEngine;
using System.Collections.Generic;

public delegate void MovementDelegate(float inputX,float inputY, bool isWalking,bool isRunning, bool isIdle,bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection);


public static class EventHandler
{
    #region 玩家移动动画event
    public static event MovementDelegate MovementEvent;
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection)
    {
        MovementEvent?.Invoke(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect,
    usingToolDirection, liftingToolDirection,pickingDirection, swingingToolDirection,idleDirection);
    }
    #endregion

    #region 背包更新event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdateEvent;
    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,List<InventoryItem> inventoryItems)
    {
        InventoryUpdateEvent?.Invoke(inventoryLocation, inventoryItems);
    }
    #endregion

    #region 时间event
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameMinuteEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;
    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameHourEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;
    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameDayEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;
    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameSeasonEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;
    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {
        AdvanceGameYearEvent?.Invoke(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }
    #endregion

    #region 场景切换相关
    public static event Action BeforeSceneUnloadFadeOutEvent;
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        BeforeSceneUnloadFadeOutEvent?.Invoke();
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadFadeInEvent;
    public static void CallAfterSceneLoadFadeInEvent()
    {
        AfterSceneLoadFadeInEvent?.Invoke();
    }
    #endregion

    public static event Action DropSelectedItemEvent;
    public static void CallDropSelectedItemEvent()
    {
        DropSelectedItemEvent?.Invoke();
    }

    #region 收割特效（割草、落叶、木屑等）
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;
    public static void CallHarvestActionEffectEvent(Vector3 effectPosition,HarvestActionEffect harvestActionEffect)
    {
        HarvestActionEffectEvent?.Invoke(effectPosition, harvestActionEffect);
    }
    #endregion

    public static event Action RemoveSelectedItemFromInventoryEvent;
    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        RemoveSelectedItemFromInventoryEvent?.Invoke();
    }

    public static event Action InstantiateCropPrefabsEvent;
    public static void CallInstantiateCropPrefabsEvent()
    {
        InstantiateCropPrefabsEvent?.Invoke();
    }


}
