using UnityEngine;

public static class SaveValues
{
    private const string SessionsKey = "RateGame_Sessions";
    private const string EventsKey = "RateGame_Events";
    private const string StartTimeKey = "RateGame_StartTime";
    private const string OpenTimeKey = "RateGame_OpenTime";
    private const string FirstShowKey = "RateGame_FirstShow";
    private const string NeverShowKey = "RateGame_NeverShow";

    public static void IncreaseNumberOfSessions()
    {
        int sessions = PlayerPrefs.GetInt(SessionsKey, 0);
        PlayerPrefs.SetInt(SessionsKey, sessions + 1);
    }

    public static int GetNumberOfSessions()
    {
        return PlayerPrefs.GetInt(SessionsKey, 0);
    }

    public static void IncreaseNumberOfCustomEvents()
    {
        int eventsCount = PlayerPrefs.GetInt(EventsKey, 0);
        PlayerPrefs.SetInt(EventsKey, eventsCount + 1);
    }

    public static int GetNumberOfCustomEvents()
    {
        return PlayerPrefs.GetInt(EventsKey, 0);
    }

    public static void SetOpenTime(bool overwrite = true)
    {
        if (overwrite || !PlayerPrefs.HasKey(OpenTimeKey))
        {
            PlayerPrefs.SetFloat(OpenTimeKey, Time.realtimeSinceStartup);
        }
    }

    public static float GetTimeSinceOpen()
    {
        float openTime = PlayerPrefs.GetFloat(OpenTimeKey, Time.realtimeSinceStartup);
        return Time.realtimeSinceStartup - openTime;
    }

    public static void AddTimeFromStart(float time)
    {
        float total = PlayerPrefs.GetFloat(StartTimeKey, 0);
        PlayerPrefs.SetFloat(StartTimeKey, total + time);
    }

    public static float GetTimeSinceStart()
    {
        return PlayerPrefs.GetFloat(StartTimeKey, 0);
    }

    public static void MarkAsSeen()
    {
        PlayerPrefs.SetInt(FirstShowKey, 1);
    }

    public static int IsFirstShow()
    {
        return PlayerPrefs.GetInt(FirstShowKey, 0);
    }

    public static void NeverShowPopup()
    {
        PlayerPrefs.SetInt(NeverShowKey, 1);
    }

    public static bool ShouldNeverShow()
    {
        return PlayerPrefs.GetInt(NeverShowKey, 0) == 1;
    }
}
