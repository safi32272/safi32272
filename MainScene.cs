using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [Header("MainPanels")]
    public GameObject[] main_Panels;
    public GameObject main_menupanel;
    public GameObject exit_Panel;
    public GameObject setting_panel;
    [Header("Mode Selection")]
    public Button animal_hunting;
    public Button mode_Nextbutton;
    [Header("Env Selection")]
    public Button forest;
    public Button env_Nextbutton;
    [Header("Weather Selection")]
    public Button sunny_weather;
    public Button weather_Nextbutton;
    [Header("Player Selection")]
    //public Button sunny_weather;
    public Image lock_image;
    public Image buy_image;
    public GameObject[] players;
    public Button player_Nextbutton;
    int player_index=0;
    public Button next_Button, prev_Button;
    [Header("Levels")]
    public Button[] levels_button;
    public Button level_nextbutton;
    int unlock_levels;
    [Header("Loadinng")]
    bool is_loading;
    public GameObject loading_panel;
    public Text loading_text;
    public Slider loading_slider;
    public GameObject LoadingP_Native;
    bool adscalled;
    [Header("Sounds")]
    public AudioSource main_menuSound;
    public AudioClip button_click,click_on,click_of;
    public AudioClip switchSound;
    public Slider volume_slider;

    [Header("Setting")]
    private float distance;
    public Slider camerazoom_slider;
    [Header("Quality")]
    public Slider qualityToggles;
    private int activeQuality;

    [Header("InAppPanels")]
    public GameObject remove_adsPenal;
    public GameObject shop_Penal;
    [Header("MessagePanel")]
    public GameObject message_panel;
    public Text message_text;
    void Start()
    {
       
        //PlayerPrefs.SetInt("OpenLevel", 10);
        unlock_levels = PlayerPrefs.GetInt("OpenLevel", 0);
        if (unlock_levels > 9)
        {
            //print(unlock_levels);
            unlock_levels = 9;
        }
        if (GlobelScript.first_timeLoad)
        {
            volume_slider.value = volume_slider.maxValue;
            main_menuSound.volume = volume_slider.maxValue;
            GlobelScript.first_timeLoad = false;
        }
        else
        {
            volume_slider.value = PlayerPrefs.GetFloat("Volume");
            main_menuSound.volume = PlayerPrefs.GetFloat("Volume");
        }
       
        UnlockLevels();
        PlayerPrefs.SetFloat("Zoom", 8);

        Players();

        //setting
        distance = PlayerPrefs.GetFloat("Zoom");
        camerazoom_slider.value = distance;

        activeQuality = PlayerPrefs.GetInt("Quality");
        //print(activeQuality);
        qualityToggles.value = activeQuality;
        shop_Penal.SetActive(true);
    }
    #region LevelsLocking Unlocking

    public void UnlockLevels()
    {
        foreach (var item in levels_button)
        {
            item.interactable = false;
            item.transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int i = 0; i <= unlock_levels; i++)
        {
            levels_button[i].interactable = true;
            levels_button[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
    

    #endregion
    #region Panels Selection
    public void OnModeSelection(int index)
    {
        if (index == 0)
        {
            mode_Nextbutton.gameObject.SetActive(true);
            animal_hunting.interactable = false;
            animal_hunting.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            mode_Nextbutton.gameObject.SetActive(false);
            animal_hunting.interactable = true;
            animal_hunting.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            message_panel.SetActive(true);
            message_text.text = "This Feature is Locked,Play all Levels of First Mode To unlock this Feature";
        }
    }
    public void OnEnvSelection(string env)
    {
        if (env == "forest")
        {
            forest.interactable = false;
            env_Nextbutton.gameObject.SetActive(true);
            forest.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            forest.interactable = true;
            env_Nextbutton.gameObject.SetActive(false);
            forest.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            message_panel.SetActive(true);
            message_text.text = "This Feature is Locked,Play all Levels of First Mode To unlock this Feature";
        }
    }
    public void OnWeatherSelection(int index)
    {
        if (index == 0)
        {
            weather_Nextbutton.gameObject.SetActive(true);
           sunny_weather.interactable = false;
           sunny_weather.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            weather_Nextbutton.gameObject.SetActive(false);
            sunny_weather.interactable = true;
            sunny_weather.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            message_panel.SetActive(true);
            message_text.text = "This Feature is Locked,Play all Levels of First Mode To unlock this Feature";
        }
    }
    public void OnPlayerSelection(int index)
    {
        if (index == 0)
        {
            lock_image.gameObject.SetActive(false);
            player_Nextbutton.gameObject.SetActive(true);
            buy_image.gameObject.SetActive(false);
            
        }
        else
        {
            lock_image.gameObject.SetActive(true);
            player_Nextbutton.gameObject.SetActive(false);
            buy_image.gameObject.SetActive(true);
        }
    }
    public void OnLevelSelection()
    {

    }
    public void OnSelectLevel(int index)
    {
        PlayerPrefs.SetInt("currentlevel", index);
        level_nextbutton.gameObject.SetActive(true);
       
       
    }
    public void LoadGamePlay()
    {
        ButtonClick(click_on);
        is_loading = true;
        loading_panel.SetActive(true);
        StartCoroutine(LoadPanel("GamePlay", level_nextbutton.gameObject));
    }
  

    #endregion

    #region LoadingPanels 
    public void EnableGameObject(GameObject panel)
    {
        ButtonClick(click_on);
        is_loading = true;
        loading_panel.SetActive(true);
        foreach (var item in main_Panels)
        {
            item.SetActive(false);
        }
        StartCoroutine(LoadPanel("PanelSelection", panel));

    }
    public void OnEnableGameObjectBack(GameObject panel)
    {
        ButtonClick(click_of);
        foreach (var item in main_Panels)
        {
            item.SetActive(false);
        }
        panel.SetActive(true);
    }

    IEnumerator LoadPanel(string sceneName,GameObject panel)
    {
        yield return null;
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
                if (loading_slider.value >= 0.8 && adscalled==false)
                {
                    LoadingP_Native.SetActive(false);
                    LevelIntertial();
                    adscalled = true;

                }  // Check if the load has finished
                if (loading_slider.value == 1f)
                {
                    is_loading = false;
                    loading_slider.value = 0;
                    adscalled = false;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;
               
                }

                yield return null;
            }
        }
        else if (sceneName == "PanelSelection")
        {
            print("loading");
            LoadingP_Native.SetActive(true);

            while (loading_slider.value != 1 && is_loading == true)
            {
                        //Turning ON Native AD
                                                           //Turning OFF Native AD

                //Output the current progress
                loading_slider.value = loading_slider.value + 0.10f * Time.deltaTime * 1;
                string percent = (loading_slider.value * 100).ToString("F0");
                loading_text.text = string.Format("<size=35>{0}%</size>", percent);
                if (loading_slider.value >= 0.8 && adscalled == false)
                {
                    LoadingP_Native.SetActive(false);
                    LevelIntertial();
                    adscalled = true;

                }
                // Check if the load has finished
                if (loading_slider.value == loading_slider.maxValue)
                {
                    
                    is_loading = false;
                    loading_slider.value = 0;
                    loading_panel.SetActive(false);
                    adscalled = false;
                    panel.SetActive(true);
               


                }

                yield return null;
            }
        }
    }

    #endregion

    #region sounds

    public void ButtonClick(AudioClip audio)
    {
        main_menuSound.PlayOneShot(audio);
    }
    public void OnSoundOf()
    {
        
        main_menuSound.volume -= volume_slider.value;
       

    }
    
    public void OnSoundOn(Slider volum)
    {

        main_menuSound.volume += volum.value;
       
    }
    //public void ChangeSoundVolume(float f)
    //{
    //    AudioListener.volume = f / 10f;
    //    PlayerPrefs.SetFloat("Volume", f);
    //}
    void OnEnable()
    {
        //Register Slider Events
        volume_slider.onValueChanged.AddListener(delegate { changeVolume(volume_slider.value); });
        qualityToggles.onValueChanged.AddListener(delegate { ToggleQuality((int)qualityToggles.value); });
    }

    //Called when Slider is moved
    void changeVolume(float sliderValue)
    {
        AudioListener.volume = sliderValue/10f;
        PlayerPrefs.SetFloat("Volume", sliderValue);
        main_menuSound.volume += sliderValue / 10f;

        //    PlayerPrefs.SetFloat("Volume", f);
    }

    void OnDisable()
    {
        //Un-Register Slider Events
        volume_slider.onValueChanged.RemoveAllListeners();
        qualityToggles.onValueChanged.RemoveAllListeners();
    }
    #endregion
    #region MainMenu
    public void OnRateUs()
    {
        ButtonClick(click_on);
        Adscaller.instance.rateus_Link();
    }
    public void OnMore()
    {
        ButtonClick(button_click);
        Adscaller.instance.moreapps_Link();
    }
    public void OnPrivacy()
    {
        ButtonClick(button_click);
        Application.OpenURL("https://gamezappstudio.blogspot.com/");

    }
    public void OnClickSetting()
    {
        ButtonClick(button_click);
        main_menupanel.SetActive(false);
        setting_panel.SetActive(true);
    }
    public void OnSettingClose()
    {
        ButtonClick(click_of);
        main_menupanel.SetActive(true);
        setting_panel.SetActive(false);
    }
    public void OnClickExit()
    {
        ButtonClick(click_of);
        main_menupanel.SetActive(false);
       exit_Panel.SetActive(true);
        Adscaller.instance.on_exit_AD();
    }
    public void OnGameQuitYes()
    {
        Application.Quit();
    }
    public void OnGameQuitNo()
    {
        ButtonClick(click_on);
        main_menupanel.SetActive(true);
        exit_Panel.SetActive(false);
    }
    public void OnClickShop()
    {
        ButtonClick(click_on);
        main_menupanel.SetActive(false);
        shop_Penal.SetActive(true);

    }
    public void OnClickShopClose()
    {
        ButtonClick(click_of);
        main_menupanel.SetActive(true);
        shop_Penal.SetActive(false);
    }
    public void OnClickRemoveads()
    {
        ButtonClick(click_on);
        main_menupanel.SetActive(false);
        remove_adsPenal.SetActive(true);

    }
    public void OnClickRemoveadsClose()
    {
        ButtonClick(click_of);
        main_menupanel.SetActive(true);
        remove_adsPenal.SetActive(false);

    }
    #endregion

    #region PlayerSelection
    public void Players()
    {
        foreach (var item in players)
        {
            item.SetActive(false);
        }
        players[player_index].SetActive(true);
        OnPlayerSelection(player_index);
    }
    public void NextPlayer()
    {
        ButtonClick(button_click);
        player_index++;
        print(player_index);
        next_Button.interactable = player_index+1 !=players.Length;
        players[player_index - 1].SetActive(false);
        players[player_index].SetActive(true);
        prev_Button.interactable = true;
        OnPlayerSelection(player_index);


    }
    public void PrevPlayer()
    {
        player_index--;
        print(player_index);
        prev_Button.interactable = player_index != 0;
        ButtonClick(button_click);
        players[player_index + 1].SetActive(false);
        players[player_index].SetActive(true);
        next_Button.interactable = true;
        OnPlayerSelection(player_index);

    }

    #endregion

    #region SettingPanel
    public void ChangeDistance(float f)
    {
        if (distance != f)
        {
            //print(f);
            PlayerPrefs.SetFloat("Zoom", 8);
            
        }
    }
    public void ToggleQuality(int i)
    {
        //print(i);
        if (activeQuality != i)
        {
            //print(i);
            //qualityToggles[activeQuality].interactable = true;
           main_menuSound.PlayOneShot(switchSound);
            activeQuality = i;
            //qualityToggles[activeQuality].interactable = false;
            PlayerPrefs.SetInt("Quality", activeQuality);
            QualitySettings.SetQualityLevel(activeQuality);
        }
        ButtonClick(click_on);
    }
    #endregion
    #region InApp Panels
    public void UnlokcAll()
    {
        ButtonClick(click_on);
        GameAppManager.instance.Buy_UnlockAll();
    }
    public void UnlockAllLevels()
    {
        ButtonClick(click_on);
        GameAppManager.instance.Buy_UnlockAll_Levels();
    }
    public  void RemoveAds()
    {
        ButtonClick(click_on);
        GameAppManager.instance.Buy_noAds();
    }

    public void LevelIntertial()
    {
        if (Adscaller.instance != null)
        {
            Adscaller.instance.on_levelselection();
        }
        else
        {
            print("adscallerempty");
        }
        
    }
    #endregion
}
