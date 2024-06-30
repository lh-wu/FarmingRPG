using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }


    /// <summary>
    /// 根据时间设置时钟UI的文本
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="gameSeason"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameDayOfWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinute"></param>
    /// <param name="gameSecond"></param>
    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
                                                    int gameHour, int gameMinute, int gameSecond)
    {

        gameMinute = gameMinute - (gameMinute % 10);
        string ampm = (gameHour >= 12) ? "pm" : "am";
        string minute;

        if (gameHour >= 13) { gameHour -= 12; }
        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        string time = gameHour.ToString() + " : " + minute + ampm;

        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + ". " + gameDay.ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("Year " + gameYear);

    }
}
