using UnityEngine;

[System.Serializable]
public class NPCScheduleEvent
{
    public int hour;
    public int minute;
    public int priority;
    public int day;
    public Weather weather;
    public Season season;
    public SceneName toSceneName;
    public GridCoordinate toGridCoordinate;
    public Direction npcFacingDirectionAtDestination = Direction.None;
    public AnimationClip animationAtDestination;

    /// <summary>
    /// ���㷽���� (hour * 100) + minute
    /// </summary>
    public int Time
    {
        get { return (hour * 100) + minute; }
    }

    public NPCScheduleEvent(int hour,int minute, int priority,int day,Weather weather, Season season, SceneName toSceneName,GridCoordinate toGridCoordinate,AnimationClip animationAtDestination)
    {
        this.hour = hour;
        this.minute = minute;
        this.priority = priority;
        this.day = day;
        this.weather = weather;
        this.season = season;
        this.toSceneName = toSceneName;
        this.toGridCoordinate = toGridCoordinate;
        this.animationAtDestination = animationAtDestination;
    }

    public NPCScheduleEvent() {; }

    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day}, Weather: {weather}, Season: {season}";
    }

}
