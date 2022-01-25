
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Events;

public class LevelActivies : MonoBehaviour
{
    public enum MissionState { kill, rescue, drink, eat }
    public enum TimeLineState { maintimeline, leveltimeline, smoothcamera, directPlay }
    public UnityEvent onStart, onEnd;
    public int total_enemies;

    public PlayableDirector playableDirector;
    public PlayableAsset playableAsset;
    public Button skip_timlineBtn;
    public GameObject mission_Panel;
    public Button start_missionButton;
    public Text mission_statement;
    public string missin_info;
    public Transform[] enemy_position;
    public Transform player_position;
    public MissionState mission_State;
    public static LevelActivies _instance;
    //public SmoothFollow smooth_camera;
    //public Transform smooth_followcamPOs;
    public TimeLineState timelinestate;
    private void Awake()
    {
        _instance = this;

    }
    void Start()
    {
        
        //print(PlayerPrefs.GetInt("Timeline", 1));

        //if (PlayerPrefs.GetInt("Timeline", 0) == 0)
        //{
        //    playableDirector.Play(playableAsset);
        //}
        //else
        //{

            if (playableAsset && playableDirector && timelinestate == TimeLineState.leveltimeline)
            {
            //print("timeline");
                playableDirector.Play(playableAsset);
            skip_timlineBtn.gameObject.SetActive(true);
            skip_timlineBtn.onClick.RemoveAllListeners();
            skip_timlineBtn.onClick.AddListener(SkipTimeline);
            }
            else
            {
                MissinStatement(missin_info);
                //if (smooth_followcamPOs && smooth_camera)
                //{
                //    Invoke("MissionStartLater", 10f);
                //    smooth_camera.GetComponent<SmoothFollow>().enabled = true;
                //    smooth_camera.target = smooth_followcamPOs;
                //}
                //else
                //{
                //    MissinStatement(missin_info);
                //}
            }
        //}

        start_missionButton.onClick.AddListener(StartUpMissionPanel);
    }
    public void SkipTimeline()
    {
        skip_timlineBtn.gameObject.SetActive(false);
        playableDirector.Stop();
    }
    public void MissionStartLater()
    {
        //smooth_camera.GetComponent<SmoothFollow>().enabled = false;
        MissinStatement(missin_info);
    }
    public void MissionStarter()
    {
        MissinStatement(missin_info);
        //start_missionButton.onClick.RemoveAllListeners();
        start_missionButton.onClick.AddListener(StartUpMissionPanel);
    }
    public void StartUpMissionPanel()
    {
        onStart?.Invoke();
    }
    public void EnemyDead(int index)
    {
        print(index);
        if (index >= total_enemies)
        {
            print("gameover");
            GlobalScripts.enemy_counter = 0;
            Invoke("LevelCompete", 2f);
            //LevelCompete();
        }
        //else
        //{
        //    GlobalScripts.enemy_counter++;
        //}
    }

   public void LevelCompete()
    {
        GamePlayUIManager.Instance.GameOver(true);
    }
    public void EnemyPosition(GameObject enemey)
    {
        if (enemey)
        {
            Transform pos = enemy_position[Random.Range(0, enemy_position.Length)];
            if (pos)
            {
                GameObject enemy = GameObject.Instantiate(enemey, pos.transform.position, Quaternion.identity);
                //enemey.transform.position = pos.transform.position;
                enemy.gameObject.SetActive(true);

                enemy.GetComponent<Enemy>().enabled = enemy.activeInHierarchy;
                //EnemyMode(enemy.GetComponent<Enemy>());
            }

        }


    }
    public void PlayerPosition(GameObject player)
    {
        if (player && player_position)
        {
            player.SetActive(true);

            player.transform.position = player_position.transform.position;
            player.transform.rotation = player_position.transform.rotation;
        }

    }
    //public void EnemyMode(Enemy enemy)
    //{
    //    if (enemy)
    //    {
    //        if (mission_State == MissionState.drink)
    //        {
    //            enemy.type = Enemy.Types.Coward;

    //        }
    //    }

    //}
    public void MissinStatement(string mission)
    {
        mission_Panel.SetActive(true);
        mission_statement.text = mission;
    }
}