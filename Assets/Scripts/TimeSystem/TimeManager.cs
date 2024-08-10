using System.Collections.Generic;
using System;
using UnityEngine;

public class TimeManager : SingletonMonobehavior<TimeManager>, ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    private bool gameClockPaused = false;
    private float gameTick = 0f;

    private string _ISavealeUniqueID;
    public string ISaveableUniqueID { get { return _ISavealeUniqueID; } set { _ISavealeUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void OnEnable()
    {
        ISaveableRegister();
        // 在切换场景的时候，使得时间暂停
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void Update()
    {
        if(!gameClockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        ++gameSecond;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            ++gameMinute;
            if (gameMinute > 59)
            {
                gameMinute = 0;
                ++gameHour;
                if (gameHour > 23)
                {
                    gameHour = 0;
                    ++gameDay;
                    if (gameDay > 30)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        ++gs;
                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = Season.Spring;
                            ++gameYear;
                            if (gameYear > 9999) { gameYear = 1; }
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        else
                        {
                            gameSeason = (Season)gs;
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            //Debug.Log("GameYear:" + gameYear + " GameSeason:" + gameSeason + " GameDayOfWeek:" + gameDayOfWeek + " GameHour:" + gameHour + " GameMinute:" + gameMinute);
        }
        // 如果有秒级的时间，则启用下面被注释的部分
        // EventHandler.CallAdvanceGameSecondEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * 30) + gameDay;
        int dayOfWeek = totalDays % 7;
        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 0:
                return "Sun";
            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);
        return gameTime;
    }


    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; ++i)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; ++i)
        {
            UpdateGameSecond();
        }
    }


    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // 清空保存器
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        // 新建sceneSave
        SceneSave sceneSave = new SceneSave();
        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.stringDictionary = new Dictionary<string, string>();
        // 保存各项时间信息到sceneSave
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());
        // 将sceneSave内容保存到保存器中
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                if (sceneSave.intDictionary != null)
                {
                    if(sceneSave.intDictionary.TryGetValue("gameYear",out int saveGameYear))
                    {
                        gameYear = saveGameYear;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int saveGameDay))
                    {
                        gameDay = saveGameDay;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int saveGameHour))
                    {
                        gameHour = saveGameHour;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int saveGameMinute))
                    {
                        gameMinute = saveGameMinute;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int saveGameSecond))
                    {
                        gameSecond = saveGameSecond;
                    }
                }

                if (sceneSave.stringDictionary != null)
                {
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string saveGameDayOfWeek))
                    {
                        gameDayOfWeek = saveGameDayOfWeek;
                    }
                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string saveGameSeason))
                    {
                        if(Enum.TryParse<Season>(saveGameSeason,out Season season))
                        {
                            gameSeason = season;
                        }
                    }
                }

                gameTick = 0f;
                EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }
        }
    }


    public void ISaveableStoreScene(string sceneName) { }
    public void ISaveableRestoreScene(string sceneName) { }

}
