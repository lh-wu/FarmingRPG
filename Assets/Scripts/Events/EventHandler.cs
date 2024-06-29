using System;
using System.Collections.Generic;

public delegate void MovementDelegate(float inputX,float inputY, bool isWalking,bool isRunning, bool isIdle,bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection);


public static class EventHandler
{
    public static event MovementDelegate MovementEvent;

    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdateEvent;

    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection)
    {
        MovementEvent?.Invoke(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect,
    usingToolDirection, liftingToolDirection,pickingDirection, swingingToolDirection,idleDirection);
    }

    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,List<InventoryItem> inventoryItems)
    {
        InventoryUpdateEvent?.Invoke(inventoryLocation, inventoryItems);
    }

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

}
