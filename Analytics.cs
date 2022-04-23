
using UnityEngine;
using Firebase.Analytics;

public class Analytics : MonoBehaviour
{
    public static Analytics Instance;


 public void Awake()
    {
        if (Instance==null)
            Instance=this;

        //DontDestroyOnLoad(gameObject);
    }

    public void ItemPurchaseEventLog(string ItemName, int ItemPrice)
    {
        //GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Coins", ItemPrice, ItemName, "");
        FirebaseAnalytics.LogEvent("ItemPurchased_Name_"+ItemName);
    }
    public void PauseEventLog()
    {

        FirebaseAnalytics.LogEvent("Game_Paused");
    }
    public void BackToMenuFromPauseEventLog()
    {

        FirebaseAnalytics.LogEvent("BackToMenuFromPause");
    }
    public void BackToMenuFromMissionResultEventLog()
    {

        FirebaseAnalytics.LogEvent("BackToMenuFromMission");
    }
    public void NextFromMissionResultEventLog()
    {

        FirebaseAnalytics.LogEvent("NextMissionFromGameOver");
    }
    public void LevelCompletedEventLog(int LevelNumber, bool IsMissionSuccessful)
    {


        FirebaseAnalytics.LogEvent("LevelComplete_"+LevelNumber.ToString()+
          (IsMissionSuccessful ? "_Successfull" : "_Failed"));
    }
    public void LevelStartedEventLog(int LevelNumber, string GameMode)
    {

        FirebaseAnalytics.LogEvent("LevelStart_"+GameMode+"_Number_"+LevelNumber.ToString());
    }
    public void OpenRobotSelection()
    {

        FirebaseAnalytics.LogEvent("Player_Selection");

    }
    public void LogEvent(string Message)
    {

        FirebaseAnalytics.LogEvent(Message);
    }
    public void LogResourceEvent(bool isAdd, float Ammount, string ItemType)
    {
        if (isAdd)
        {

            FirebaseAnalytics.LogEvent("Coins_Earned");
        }
        else
        {

            FirebaseAnalytics.LogEvent("Coins_Used");
        }
    }

}
