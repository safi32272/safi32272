using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    [Header("MainPanels")]
    public GameObject[] penals;
    public GameObject success_penal;
    public GameObject failed_penal;
    public GameObject pause_penal;
  
    public GameObject setting_penal;
    public GameObject game_ControleUI;
  

    [Header("Loadinng")]
    bool is_loading;
    public GameObject loading_panel;
    public Text loading_text;
    public Slider loading_slider;

    [Header("Sounds")]
    public AudioSource sounder;
    public AudioClip openSound, closeSound, level_complete,level_failed;

    [Header("Mission panel")]
    public GameObject mission_penal;
    public Text mission_statement;

    [Header("Levels Manager")]
    public GameObject[] levels;
    //public GameObject TimelineLevel;
    int current_level;

    [Header("StartManager")]
   

    public GameObject controller;

    //public Enemy[] companions = new Enemy[2];

    public RpgCamera rpgCamera;

    public GameObject gameMenu;

    public GameObject starter;

    //public GameObject enemies;

    public CameraShake cameraShake;

    public BaseCharacterController characterController;

    public AudioSource backMusic;

    public AudioClip ambiance;

    public GameObject bonusGold;

    [Header("EFFECT")]
    public ParticleSystem levelUpParticlePrefab;

    private ParticleSystem levelUpParticle;


    public static GamePlayManager _instance;
    private void Awake()
    {
        _instance = this;
        Camera main = Camera.main;
        float[] array = new float[32];
        array[0] = 200f;
        array[15] = 200f;
        array[16] = 200f;
        main.layerCullDistances = array;

        levelUpParticle =Instantiate(levelUpParticlePrefab);
    }

    public void StartGame()
    {
        starter.SetActive(value: true);
        Invoke("ActivateGame", 0.15f);
        controller.SetActive(value: true);
        ClosePanelSound(false);

        mission_penal.SetActive(false);
    }

    private void ActivateGame()
    {
        backMusic.clip = ambiance;
        backMusic.Play();
        //companions[0].enabled = true;
        //companions[1].enabled = true;
        rpgCamera.enabled = true;
        characterController.enabled = true;
        gameMenu.SetActive(value: true);
        //enemies.SetActive(value: true);
        cameraShake.enabled = true;
       
    }
    void Start()
    {
        //main_menuSound.volume = PlayerPrefs.GetFloat("Volume");
        foreach (var item in levels)
        {
            item.SetActive(false);
        }
        current_level = PlayerPrefs.GetInt("currentlevel", 0);
    
        levels[current_level].SetActive(true);
       
       
        //print(current_level);
     

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        AudioListener.volume = PlayerPrefs.GetFloat("Volume") / 10f;
    }
    public void ShowUI()
    {
        game_ControleUI.SetActive(true);

    }
    public void HideUi()
    {
        game_ControleUI.SetActive(false);
    }
    public void OnGamePause()
    {
        ClosePanelSound(true);
        HideUi();
        pause_penal.SetActive(true);
        Time.timeScale = 0;
        if (Adscaller.instance != null)
        {
            Adscaller.instance.on_pause_AD();
        }
        
    }
    public void OnGameResume()
    {
        ClosePanelSound(false);
        StartCoroutine(LoadPanel("PanelSelection",pause_penal));
    }
    public void OnGameRestart()
    {
        ClosePanelSound(false);
        StartCoroutine(LoadScene("Next"));
    }
    public void OnGameNext()
    {
        ClosePanelSound(false);
        //print(PlayerPrefs.GetInt("currentlevel"));
        current_level++;
        PlayerPrefs.SetInt("currentlevel", current_level);
        PlayerPrefs.SetInt("openlevel",PlayerPrefs.GetInt("openlevel")+1);
        if (PlayerPrefs.GetInt("currentlevel", 0) >= 10)
        {
            current_level = 0;
            PlayerPrefs.SetInt("currentlevel", current_level);
            PlayerPrefs.SetInt("openlevel", 9);
        }
        //print(PlayerPrefs.GetInt("currentlevel"));
        
        StartCoroutine(LoadScene("Next"));
    }
    public void OnGameMainMenu()
    {
        ClosePanelSound(false);
        StartCoroutine(LoadScene("MainMenu"));
    }
    public void OnSuccess()
    {
        SendLevelUpEffect(characterController.gameObject.transform);
        cameraShake.enabled = false;
        rpgCamera.enabled = false;
        rpgCamera.gameObject.GetComponent<SmoothFollow>().enabled = true;
        characterController.drink_waterPosition.GetComponent<Animator>().enabled = true;
        rpgCamera.gameObject.GetComponent<SmoothFollow>().target=characterController.drink_waterPosition.transform;
        game_ControleUI.SetActive(false);
        Invoke("OnLevelComplete", 6f);
        backMusic.clip = level_complete;
        backMusic.Play();
        //Adscaller.instance.on_fail_sucess_commercial_AD();
    }
    public void OnLevelComplete()
    {
        EnabledGamePanel(success_penal);
        Adscaller.instance.on_fail_sucess_commercial_AD();
    }
    public void OnFailed()
    {
        //sounder.Stop();
        EnabledGamePanel(failed_penal);
        Adscaller.instance.on_fail_sucess_commercial_AD();
    }
    public void OnClickSetting()
    {
        EnabledGamePanel(setting_penal);
    }
    public void OnClickCloseSetting()
    {
        EnabledGamePanel(pause_penal);
    }
    public void EnabledGamePanel(GameObject panel)
    {
        ClosePanelSound(true);
        foreach (var item in penals)
        {
            item.SetActive(false);
        }
      
        panel.SetActive(true);
    }
    public void MissionAssigned(string missionstatement)
    {
        mission_penal.SetActive(true);
        mission_statement.text = missionstatement;
    }
   
    //public void OpenDirectPanel(int i)
    //{
    //    if (playerDead)
    //    {
    //        return;
    //    }
    //    if (open)
    //    {
    //        if (activePanel == i)
    //        {
    //            OpenClosePanel();
    //        }
    //        else
    //        {
    //            ActivatePanel(i);
    //        }
    //        return;
    //    }
    //    OpenClosePanel();
    //    if (activePanel != i)
    //    {
    //        ActivatePanel(i);
    //    }
    //}
    //public void PlayerDead()
    //{
    //    playerDead = true;
    //    if (open)
    //    {
    //        OpenClosePanel();
    //    }
    //}
    //public void ActivatePanel(int i)
    //{
    //    if (activePanel != i && !playerDead)
    //    {
    //        sounder.PlayOneShot(closeSound);
    //        tabTexts[activePanel].color = passiveColor;
    //        panels[activePanel].SetActive(value: false);
    //        activePanel = i;
    //        tabTexts[activePanel].color = activeColor;
    //        panels[activePanel].SetActive(value: true);
    //        mapCam.SetActive(i == 2);
    //    }
    //}

    public void ClosePanelSound(bool open)
    {
        if (open)
        {
            sounder.PlayOneShot(openSound);
        }
        else
        {
            sounder.PlayOneShot(closeSound);
        }
       
    }
  
    private void SendLevelUpEffect(Transform t)
    {
        levelUpParticle.transform.SetParent(t, worldPositionStays: false);
        levelUpParticle.Play();
    }
    IEnumerator LoadPanel(string sceneName, GameObject panel)
    {
        Time.timeScale = 1;
        loading_slider.value = 0;
        yield return null;
        is_loading = true;
        loading_panel.SetActive(true);
        AsyncOperation asyncOperation;
        //Begin to load the Scene you specify
        if (sceneName == "GamePlay")
        {
            asyncOperation = SceneManager.LoadSceneAsync("GamePlay");
            asyncOperation.allowSceneActivation = false;

            while (loading_slider.value != 1 && is_loading == true)
            {
                //Output the current progress
                loading_slider.value = loading_slider.value + 0.08f * Time.deltaTime * 1;
                string percent = (loading_slider.value * 100).ToString("F0");
                loading_text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_slider.value == 1f)
                {
                    is_loading = false;
                    loading_slider.value = 0;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;

                }

                yield return null;
            }
        }
        else if (sceneName == "PanelSelection")
        {


            while (loading_slider.value != 1 && is_loading == true)
            {
                //Output the current progress
                loading_slider.value = loading_slider.value + 0.08f * Time.deltaTime * 1;
                string percent = (loading_slider.value * 100).ToString("F0");
                loading_text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_slider.value >= 1f)
                {
                    is_loading = false;
                    loading_slider.value = 0;
                    panel.SetActive(false);
                    loading_panel.SetActive(false);
                    game_ControleUI.SetActive(true);

                }

                yield return null;
            }
        }
    }
    IEnumerator LoadScene(string sceneName)
    {
        Time.timeScale= 1;
        yield return null;
        is_loading = true;
        loading_slider.value = 0;
        loading_panel.SetActive(true);
        AsyncOperation asyncOperation;
        //Begin to load the Scene you specify
        if (sceneName == "Next")
        {
            asyncOperation = SceneManager.LoadSceneAsync("GamePlay");
            asyncOperation.allowSceneActivation = false;

            while (loading_slider.value != 1 && is_loading == true)
            {
                //Output the current progress
                loading_slider.value = loading_slider.value + 0.08f * Time.deltaTime * 1;
                string percent = (loading_slider.value * 100).ToString("F0");
                loading_text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_slider.value == 1f)
                {
                    is_loading = false;
                    loading_slider.value = 0;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;

                }

                yield return null;
            }
        }
        else if (sceneName == "MainMenu")
        {
            asyncOperation = SceneManager.LoadSceneAsync("MainMenu");
            asyncOperation.allowSceneActivation = false;

            while (loading_slider.value != 1 && is_loading == true)
            {
                //Output the current progress
                loading_slider.value = loading_slider.value + 0.08f * Time.deltaTime * 1;
                string percent = (loading_slider.value * 100).ToString("F0");
                loading_text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_slider.value == 1f)
                {
                    is_loading = false;
                    loading_slider.value = 0;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;

                }

                yield return null;
            }
        }
    }


}
