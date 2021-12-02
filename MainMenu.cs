using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.Audio;
using BugsnagUnity;


public class MainMenu : MonoBehaviour
{
    [Header("CanvasPanels")]
    public GameObject GameModes;
    public GameObject MainMenuPanel, Shop, SettingScreen, EnviromentPanel, CharacterPanel,
        RateUsPanel, LevelSelectionScreen, QuitPanel;
    public GameObject Robot, Car, Tank, Helicopter, Train;
    public Image[] Stars;
    public Sprite StarOn, StarOff;
    public GameObject characterLock, characterNextButton, characterBuyButton;

    public GameObject  tapToContinueButton;
    public Image LoadingProgressBar;
    public Text loadingPercText, characterBuyText;
    public RectTransform levelContent;
    public Slider MusicSlider, SfxSlider;

    [Header("Values")]
    public float ScreenAnimationTime = 0.5f;
    public string PlayAccountURL, PrivacyURL;
    public int Skin_1Price, Skin_2Price;
    public int numberOfLevels = 100;
    public float widthOfLevelPanel = 316f;
    public float scrollLerpSpeed = 2f;
    public float fakeLoadingTime = 5f;
    public float HeliDistance = 7.5f, HeliHeight = 3f, HeliPitch = -5f, HeliOffSet = -1.5f, 
        CarDistance = 5f, CarHeight = 3f, CarPitch = -5f, CarOffSet = -1.5f, 
        RobotDistance = 5f, RobotHeight = 3f, RobotPitch = -5f, RobotOffSet = -1.5f, 
        TankDistance = 6f, TankHeight = 3f, TankPitch = -5f, TankOffSet = -1.5f, 
        speed = 3f, TrainDistance=7.5f, TrainHeight=3f,TrainPitch=-5f, TrainOffSet = -1.5f;
    public bool updateCameraPos = true;

    [Header("References")]
    public Player[] players;
    public GameObject LevelPrefab;
    public GameObject[] ModesHovers;
    public Text[] coinTexts;
    public AudioMixer audioMixer;
    public ScrollRect scrollRect;
    public CustomCamera customCamera;
    public GameObject[] CharacterOvers;
    public GameObject[] liveBackgroundObjects;

    [Header("Dialog")]
    public GameObject DialogBox;
    public Text TitleText, DetailText;
    public GameObject ShopButton;

    [Header("Fake Loading")]
    public GameObject FakeLoadingObject;
    public Image fakeLoadingProgressbar;

    [Header("Loading")]
    bool is_loading;
    public Image loading_image;
    public Text loading_Text;
    public GameObject loading_Panel;

    [Header("UnlockPanels")]
    public GameObject unlock_EverythingPanel,remove_adsPanel,unlock_ModesPanel,unlock_EnvPanel,unlock_levelPanel;

    private LevelUI[] levelUI;
    private float Distance = 6f, Height = 3f, Pitch = -5f, OffSet = -1.5f;

    public Button train_NextButton,robot_NextButton;
    public Button car_Mode, mech_Mode,train_Mode;
    

    public void ChangeSFXVol (bool isIncrease)
    {
        if (isIncrease)
        {
            SfxSlider.value += 1;
        }else
        {
            SfxSlider.value -= 1;
        }
        audioMixer.SetFloat("SfxVol", SfxSlider.value * 7f);
    }

    public void ChangeMusicVol(bool isIncrease)
    {
        if (isIncrease)
        {
            MusicSlider.value += 1;
        }
        else
        {
            MusicSlider.value -= 1;
        }
        audioMixer.SetFloat("music", MusicSlider.value * 7f);
    }

    public void CloseSettings ()
    {
        Data.SoundVolume = SfxSlider.value;
        Data.MusicVolume = MusicSlider.value;
        Data.SaveData();
    }

    public void ShowDialog (string title, string details, bool shopButton)
    {
        DialogBox.SetActive(true);
        TitleText.text = title;
        DetailText.text = details;
        ShopButton.SetActive(shopButton);
    }

    public void CreateLevels()
    {
        levelContent.sizeDelta = new Vector2(numberOfLevels * widthOfLevelPanel, 665f);
        for (int a = 0; a < numberOfLevels; a++)
        {
            GameObject tempLevel;
            tempLevel = Instantiate(LevelPrefab);
            levelUI[a] = tempLevel.GetComponent<LevelUI>();
            levelUI[a].Setup(a, levelContent.transform, false, 0);
        }
    }

    private void Awake()
    {
        Time.timeScale = 1;
        Data.LoadData();
        MusicSlider.value = Data.MusicVolume;
        SfxSlider.value = Data.SoundVolume;
        audioMixer.SetFloat("music", MusicSlider.value * 7f);
        audioMixer.SetFloat("SfxVol", SfxSlider.value * 7f);
        //PlayerPrefs.SetInt("selectedLevel", 20);
        //print((PlayerPrefs.GetInt("selectedLevel", 0)));
        if(PlayerPrefs.GetInt("Timeline", 0) != 1){
            train_Mode.interactable = false;
            train_Mode.transform.GetChild(1).gameObject.SetActive(true);

        }
        else
        {
            train_Mode.interactable = true;
            train_Mode.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("selectedLevel", 0) >= 8)
        {
            car_Mode.interactable = true;
            car_Mode.transform.GetChild(0).transform.gameObject.SetActive(false);
            car_Mode.transform.GetChild(1).transform.gameObject.SetActive(false);
        }
     if (PlayerPrefs.GetInt("selectedLevel", 0) >= 16)
        {
           mech_Mode.interactable = true;
           mech_Mode.transform.GetChild(0).transform.gameObject.SetActive(false);
            mech_Mode.transform.GetChild(1).transform.gameObject.SetActive(false);
        }

    }

    //public void AgreePolicies ()
    //{
    //    Data.isFirstRun = 0;
    //    Data.SaveData();
    //    privacyPolicyDialog.SetActive(false);
    //}

    public void BuyProduct(int id)
    {
        switch (id)
        {
            //case 0:
            //    InAppPurchasing.Instance.BuyProductID("noads");
            //    break;
            //case 1:
            //    InAppPurchasing.Instance.BuyProductID("unlockallrobots");
            //    break;
            //case 2:
            //    InAppPurchasing.Instance.BuyProductID("unlockalllevels");
            //    break;
            //case 3:
            //    InAppPurchasing.Instance.BuyProductID("combooffer");
            //    break;
        }
    }

    public void ShowRewardedVideo()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("RewardedVideoRequested");
        if (MainAdsManagerController.instance != null)
        {
            MainAdsManagerController.instance.removeall_rewardevent();
            MainAdsManagerController.videocomplete += GiveReward;
            MainAdsManagerController.instance.show_AD_Video();
        }
    }

    public void GiveReward(object sender, System.EventArgs e)
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogResourceEvent(true, 1000, "RewardedVideo");
        Data.coins += 1000;
        UpdateCoinsUI();
        Data.SaveData();
        MainAdsManagerController.instance.removeall_rewardevent();
    }

    bool sm = false;
    float newpos = 0;
    float timetrackscroller = 0.2f;
    public void OpenLevelSelection()
    {
        print("cxall");
        if (!sm)
        {

            //OpenNewScreen(GameModes, LevelSelectionScreen);
            GameModes.SetActive(false);
            LevelSelectionScreen.SetActive(true);
            LevelSelectionScreen.transform.localScale = Vector3.one;
            newpos = 0;

            switch (Data.selectedGameMode)
            {
                case 0:
                    SelectLevel(Data.MechModeUnlockedLevel);
                    newpos = Data.MechModeUnlockedLevel / 100f;
                    break;
                case 1:
                    SelectLevel(Data.RobotModeUnlockedLevel);
                    newpos = Data.RobotModeUnlockedLevel / 100f;
                    break;
                case 2:
                    SelectLevel(Data.CarModeUnlockedLevel);
                    newpos = Data.CarModeUnlockedLevel / 100f;
                    break;
                case 3:
                    SelectLevel(Data.TankModeUnlockedLevel);
                    newpos = Data.TankModeUnlockedLevel / 100f;
                    break;
                case 4:
                    SelectLevel(Data.HeliModeUnlockedLevel);
                    newpos = Data.HeliModeUnlockedLevel / 100f;
                    break;
                case 5:
                    SelectLevel(Data.FreeModeUnlockedLevel);
                    newpos = Data.FreeModeUnlockedLevel / 100f;
                    break;
            }
            scrollRect.horizontalNormalizedPosition = newpos;
            //scrollRect.DOHorizontalNormalizedPos(newpos, scrollLerpSpeed);
            //ShowFakeLoading();
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_levelselection();
            }
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("LevelSelectionOpen");
            atScrollerScreen = true;
            timetrackscroller = Time.time + scrollLerpSpeed;
        }
        else
        {
            //GameModes.SetActive(true);
            //LevelSelectionScreen.SetActive(false);
            OpenNewScreen(LevelSelectionScreen, GameModes);
        }
        sm = !sm;
    }

    public void OnScrollerValueChange ()
    {
        atScrollerScreen = false;
    }

    public bool atScrollerScreen = false;
    public void LerpScroller ()
    {
        if (timetrackscroller > Time.time)
        {
            scrollRect.horizontalNormalizedPosition = newpos;
            //scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, newpos, Time.deltaTime * scrollLerpSpeed);
        }
    }

    public void SelectLevel(int levelNumber)
    {
        Data.selectedLevel = levelNumber;
        switch (Data.selectedGameMode)
        {
            case 0:
                Data.MechModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.MechModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
            case 1:
                Data.RobotModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.RobotModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
            case 2:
                Data.CarModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.CarModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
            case 3:
                Data.TankModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.TankModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
            case 4:
                Data.HeliModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.HeliModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
            case 5:
                Data.FreeModeSelectedLevel = levelNumber;
                for (int a = 0; a < levelUI.Length; a++)
                {
                    if (a <= Data.FreeModeUnlockedLevel)
                    {
                        if (a == levelNumber)
                        {
                            levelUI[a].Setup(true, 1);
                        }
                        else
                        {
                            levelUI[a].Setup(false, 1);
                        }
                    }
                    else
                    {
                        levelUI[a].Setup(false, 0);
                    }
                }
                break;
        }
    }

    public void LoadLevel()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LevelStartedEventLog(Data.selectedLevel, Data.CurrentGameMode());
        Data.SaveData();
        //LoadingScreen.SetActive(true);
        is_loading = true;
        loading_Panel.SetActive(true);
        tap = false;
        StartCoroutine(LoadScene("GamePlay"));
    }
    public void SelectMode(int mode)
    {
        print(mode);
        if (mode == 3)
        {
            train_NextButton.gameObject.SetActive(true);
            robot_NextButton.gameObject.SetActive(false);
           
        }
        else
        {
            train_NextButton.gameObject.SetActive(false);
            robot_NextButton.gameObject.SetActive(true);
            Data.selectedGameMode = mode;
            //ActivateOneInArray(ModesHovers, mode);
        }
      

    }
    public void LoadTrainLevel()
    {
        is_loading = true;
        loading_Panel.SetActive(true);
        StartCoroutine(LoadScene("TrainGame"));
    }
    IEnumerator LoadScene()
    {
        yield return null;
        //LoadingProgressBar.fillAmount = 0;
        SceneManager.LoadScene("City");
        //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("City");
        //asyncOperation.allowSceneActivation = false;
        //while (!asyncOperation.isDone)
        //{
        //    //loadingPercText.text = "Loading: " + (int)(asyncOperation.progress * 100) + "%";
        //    //LoadingProgressBar.fillAmount = asyncOperation.progress;
        //    //if (asyncOperation.progress >= 0.9f)
        //    //{
        //        //loadingPercText.text = "Tap to continue";
        //        //LoadingProgressBar.gameObject.SetActive(false);
        //        //tapToContinueButton.SetActive(true);
        //        //if (tap)
        //        //    asyncOperation.allowSceneActivation = true;
        //    //}

        //    yield return null;
        //}
    }

    bool tap = false;
    public void TapToContinue()
    {
        tap = true;
    }

    void Start()
    {
        MainMenuPanel.SetActive(true);
        Shop.SetActive(false);
        SettingScreen.SetActive(false);
        CharacterPanel.SetActive(false);
    
        LevelSelectionScreen.SetActive(false);
        GameModes.SetActive(false);
        foreach (GameObject g in liveBackgroundObjects)
        {
            g.SetActive(true);
        }
        levelUI = new LevelUI[numberOfLevels];
        CreateLevels();
        Data.selectedGameMode = 5;
        UpdateCoinsUI();
        //Bugsnag.Notify(new System.InvalidOperationException("Test error"));
    }

    void Update()
    {
        if (updateCameraPos)
        {
            customCamera.TPSDistance = Mathf.Lerp(customCamera.TPSDistance, Distance, Time.deltaTime * speed);
            customCamera.TPSHeight = Mathf.Lerp(customCamera.TPSHeight, Height, Time.deltaTime * speed);
            customCamera.TPSPitchAngle = Mathf.Lerp(customCamera.TPSPitchAngle, Pitch, Time.deltaTime * speed);
            customCamera.TPSOffsetX = Mathf.Lerp(customCamera.TPSOffsetX, OffSet, Time.deltaTime * speed);
        }
        //LerpScroller();
    }

    public void UpdateCoinsUI()
    {
        foreach (Text t in coinTexts)
        {
            t.text = Data.coins.ToString();
        }
    }

    public void BuySkin()
    {
        if (currentSkin == 1)
        {
            if (Data.PlayerSkinLock_1 == 0)
            {
                if (Data.coins >= Skin_1Price)
                {
                    Data.coins -= Skin_1Price;
                    Data.PlayerSkinLock_1 = 1;
                    ShowDialog("Robot Unlocked", "Robot is unlocked, tap next to play with this robot.", false);
                    UpdateCoinsUI();
                    Data.SaveData();
                    SetupCharacterDisplay(currentSkin);
                    if (Analyticts.Instance != null)
                        Analyticts.Instance.ItemPurchaseEventLog("RobotSkin_1", Skin_1Price);
                }
                else
                {
                    ShowDialog("Insufficient Coins", "You are low at coins," +
                        " goto Shop and select a package.", true);
                }
            } 
        }else if (currentSkin == 2)
        {
            if (Data.PlayerSkinLock_2 == 0)
            {
                if (Data.coins >= Skin_2Price)
                {
                    Data.coins -= Skin_2Price;
                    Data.PlayerSkinLock_2 = 1;
                    ShowDialog("Robot Unlocked", "Robot is unlocked, tap next to play with this robot.", false);
                    UpdateCoinsUI();
                    Data.SaveData();
                    SetupCharacterDisplay(currentSkin);
                    if (Analyticts.Instance != null)
                        Analyticts.Instance.ItemPurchaseEventLog("RobotSkin_2", Skin_2Price);
                }
                else
                {
                    ShowDialog("Insufficient Coins", "You are low at coins," +
                        " goto Shop and select a package.", true);
                }
            }
        }
    }

    int currentSkin = 0;
    public void NextPrevious(bool isNext)
    {
        if (isNext)
        {
            currentSkin++;
            if (currentSkin > 2)
                currentSkin = 0;
        }
        else
        {
            currentSkin--;
            if (currentSkin < 0)
                currentSkin = 2;
        }
        SetupCharacterDisplay(currentSkin);       
    }

    public void SetupCharacterDisplay(int charID)
    {
        if (charID == 1)
        {
            if (Data.PlayerSkinLock_1 == 0)
            {
                characterLock.SetActive(true);
                characterNextButton.SetActive(false);
                characterBuyButton.SetActive(true);
                characterBuyText.text = Skin_1Price.ToString();
            }
            else
            {
                characterLock.SetActive(false);
                characterNextButton.SetActive(true);
                characterBuyButton.SetActive(false);
            }
        }
        else if (charID == 2)
        {
            if (Data.PlayerSkinLock_2 == 0)
            {
                characterLock.SetActive(true);
                characterNextButton.SetActive(false);
                characterBuyButton.SetActive(true);
                characterBuyText.text = Skin_2Price.ToString();
            }
            else
            {
                characterLock.SetActive(false);
                characterNextButton.SetActive(true);
                characterBuyButton.SetActive(false);
            }
        }
        else if (charID == 0)
        {
            characterLock.SetActive(false);
            characterNextButton.SetActive(true);
            characterBuyButton.SetActive(false);
        }
        foreach (Player p in players)
        {
            p.ChangeSkin(charID);
        }
    }

    public void ChangeAllPlayerSking ()
    {
        players[0].ChangeSkin(currentSkin);
        players[1].ChangeSkin(currentSkin);
        players[2].ChangeSkin(currentSkin);
        players[3].ChangeSkin(currentSkin);
    }

    public void SwitchOnRobot()
    {
        ChangeAllPlayerSking();
        Robot.SetActive(true);
        Car.SetActive(false);
        Train.SetActive(false);
        Helicopter.SetActive(false);
        Distance = RobotDistance;
        Height = RobotHeight;
        Pitch = RobotPitch;
        OffSet = RobotOffSet;
        //ActivateOneInArray(CharacterOvers, 0);
        customCamera.ResetOrbitValues();
    }

    public void SwitchOnCar()
    {
        ChangeAllPlayerSking();
        Robot.SetActive(false);
        Car.SetActive(true);
       Train.SetActive(false);
        Helicopter.SetActive(false);
        Distance = CarDistance;
        Height = CarHeight;
        Pitch = CarPitch;
        OffSet = CarOffSet;
        //ActivateOneInArray(CharacterOvers, 3);
        customCamera.ResetOrbitValues();
    }

    public void SwitchOnTank()
    {
        ChangeAllPlayerSking();
        Robot.SetActive(false);
        Car.SetActive(false);
        Tank.SetActive(true);
        Helicopter.SetActive(false);
        Distance = TankDistance;
        Height = TankHeight;
        Pitch = TankPitch;
        OffSet = TankOffSet;
        //ActivateOneInArray(CharacterOvers, 1);
        customCamera.ResetOrbitValues();
    }

    public void SwitchOnHeli()
    {
        ChangeAllPlayerSking();
        Robot.SetActive(false);
        Car.SetActive(false);
        Tank.SetActive(false);
        Helicopter.SetActive(true);
        Distance = HeliDistance;
        Height = HeliHeight;
        Pitch = HeliPitch;
        OffSet = HeliOffSet;
        //ActivateOneInArray(CharacterOvers, 2);
        customCamera.ResetOrbitValues();
    }

    /// <summary>
    /// Trains Transformation
    /// </summary>
    public void SwitchOnTrains()
    {
        ChangeAllPlayerSking();
        Robot.SetActive(false);
        Car.SetActive(false);
        Tank.SetActive(false);
        Helicopter.SetActive(false);
        Train.SetActive(true);
        Distance = TrainDistance;
        Height = TrainHeight;
        Pitch = TrainPitch;
        OffSet = TrainOffSet;
        //ActivateOneInArray(CharacterOvers, 1);
        customCamera.ResetOrbitValues();
    }
    bool shopbool = false;
    public void OpenShop()
    {
        if (!shopbool)
        {
            OpenNewScreen(MainMenuPanel, Shop);
            
            if (Analyticts.Instance != null)
                Analyticts.Instance.OpenShopEventLog();
          
        }
        else
        {
            print("----");
            OpenNewScreen(Shop, MainMenuPanel);
            MainMenuPanel.GetComponent<onEnableSuccess>().enabled = false;
           
        }
        shopbool = !shopbool;
        print(shopbool);
    }

    public void DialogToShop ()
    {
        print("shopppp" + shopbool);
        OpenNewScreen(CharacterPanel, Shop);
        DialogBox.SetActive(false);
        shopbool = !shopbool;
        chsel = !chsel;
        print("shopbool" + shopbool);
    }

    bool chsel = false;
    public void EnvSelection()
    {
        is_loading = true;
        loading_Panel.SetActive(true);
        StartCoroutine(LoadScene("EnvSelection"));
    }
    public void PlayerSelection()
    {
        is_loading = true;
        loading_Panel.SetActive(true);
        StartCoroutine(LoadScene("PlayerSelection"));


    }
    public void ModeSelection()
    {
        is_loading = true;
        loading_Panel.SetActive(true);
        StartCoroutine(LoadScene("ModeSelection"));


    }
    public void LevelSelection()
    {
        is_loading = true;
        loading_Panel.SetActive(true);
        StartCoroutine(LoadScene("LevelSelection"));
        print("levelselction");


    }
    bool env = false;

    public void EnvPanel()
    {
        if (!env)
        {
            OpenNewScreen(MainMenuPanel, EnviromentPanel);
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_levelselection();
            }
        }
        else
            OpenNewScreen(EnviromentPanel, MainMenuPanel);
        env = !env;
      
    }
    public void Play()
    {
        if (!chsel)
        {

            OpenNewScreen(EnviromentPanel, CharacterPanel);
            SetupCharacterDisplay(0);
            SwitchOnRobot();
            foreach (GameObject g in liveBackgroundObjects)
            {
                g.SetActive(false);
            }
            customCamera.isMainMenuCamera = true;
            customCamera.ResetOrbitValues();
            updateCameraPos = true;
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("PlayScreen");
            //ShowFakeLoading();
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_levelselection();
            }
        }
        else
        {
            OpenNewScreen(CharacterPanel, EnviromentPanel);
            foreach (GameObject g in liveBackgroundObjects)
            {
                g.SetActive(true);
            }
            updateCameraPos = false;
            customCamera.isMainMenuCamera = true;
            customCamera.ResetOrbitValues();
        }
        chsel = !chsel;
    }
    IEnumerator LoadScene(string sceneName)
    {
        yield return null;
        AsyncOperation asyncOperation;
        //Begin to load the Scene you specify
        if (sceneName == "GamePlay")
        {
            asyncOperation = SceneManager.LoadSceneAsync("City");
            asyncOperation.allowSceneActivation = false;

            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.08f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;
                    //loading_image.GetComponent<Animator>().enabled = true;
                }

                yield return null;
            }
        }
       else if (sceneName == "TrainGame")
        {
            asyncOperation = SceneManager.LoadSceneAsync("GGS2_Mission1");
            asyncOperation.allowSceneActivation = false;

            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.08f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    //Change the Text to show the Scene is ready
                    asyncOperation.allowSceneActivation = true;
                    //loading_image.GetComponent<Animator>().enabled = true;
                }

                yield return null;
            }
        }
        else if (sceneName == "EnvSelection")
        {


            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.8f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    loading_Panel.SetActive(false);
                    EnvPanel();
                    //Change the Text to show the Scene is ready


                }

                yield return null;
            }
        }
        else if (sceneName == "PlayerSelection")
        {


            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.8f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    loading_Panel.SetActive(false);
                    Play();
                    //Change the Text to show the Scene is ready


                }

                yield return null;
            }
        }
      
        else if (sceneName == "ModeSelection")
        {


            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.8f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    loading_Panel.SetActive(false);
                    ModeSelectionScreen();
                    //Change the Text to show the Scene is ready


                }

                yield return null;
            }
        }
       
        else if (sceneName == "LevelSelection")
        {


            while (loading_image.fillAmount != 1 && is_loading == true)
            {
                //Output the current progress
                loading_image.fillAmount = loading_image.fillAmount + 0.8f * Time.deltaTime * 1;
                string percent = (loading_image.fillAmount * 100).ToString("F0");
                loading_Text.text = string.Format("<size=35>{0}%</size>", percent);
                // Check if the load has finished
                if (loading_image.fillAmount == 1f)
                {
                    is_loading = false;
                    loading_image.fillAmount = 0;
                    loading_Panel.SetActive(false);
                    OpenLevelSelection();
                    //Change the Text to show the Scene is ready


                }

                yield return null;
            }
        }
    }
    bool mss = false;
    public void ModeSelectionScreen()
    {
        if (!mss)
        {
            OpenNewScreen(CharacterPanel, GameModes);
            //ActivateOneInArray(ModesHovers, Data.selectedGameMode);
            Data.SelectedPlayerSkin = currentSkin;
            Data.SaveData();
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("OpenModeSelection");
        }
        else
            OpenNewScreen(GameModes, CharacterPanel);
        mss = !mss;
    }

    bool settings = false;
    public void OpenSettings()
    {
        if (settings)
            OpenNewScreen(SettingScreen, null);
        else
            OpenNewScreen(null, SettingScreen);
        settings = !settings;
    }

    public void OpenRateUsPanel()
    {
        Adscaller.instance.rateus_Link();
    }

    bool quitdialog = false;
    public void OpenQuitPanel()
    {
        if (!quitdialog)
        {
            
            OpenNewScreen(null, QuitPanel);
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_exit_AD();
            }
        }
        else
        {
            OpenNewScreen(QuitPanel, null);
        }
        quitdialog = !quitdialog;
    }

    public void QuitGame()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("AppQuit");
        Application.Quit();
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL(PrivacyURL);
    }

    public void MoreGames()
    {
        Application.OpenURL(PlayAccountURL);
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("OpenMoreGames");
    }

    public void SubmitRating()
    {
        if (rating > 3)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
        }
        OpenNewScreen(RateUsPanel, null);
    }

    public void LaterRatting()
    {
        OpenNewScreen(RateUsPanel, null);
    }

    int rating = 4;
    public void UpdateRating(int starsCount)
    {
        rating = starsCount;
        for (int a = 0; a < Stars.Length; a++)
        {
            if (a < starsCount)
            {
                Stars[a].sprite = StarOn;
            }
            else
            {
                Stars[a].sprite = StarOff;
            }
        }
    }


    GameObject screenTwo;
    GameObject screenOne;
    public void OpenNewScreen(GameObject firstScreen, GameObject secondScreen)
    {
        screenOne = firstScreen;
        screenTwo = secondScreen;
        if (screenOne == null && screenTwo != null)
        {
            screenTwo.SetActive(true);
            screenTwo.transform.localScale = Vector3.zero;
            screenTwo.transform.DOScale(1, ScreenAnimationTime);
        }
        else if (screenOne != null && screenTwo != null)
        {
            screenOne.transform.localScale = Vector3.one;
            screenOne.transform.DOScale(0, ScreenAnimationTime).OnComplete(OpenScreenOnComplete);
        }
        else if (screenOne != null && screenTwo == null)
        {
            screenOne.SetActive(true);
            screenOne.transform.localScale = Vector3.one;
            screenOne.transform.DOScale(0, ScreenAnimationTime);
        }
    }

    private void OpenScreenOnComplete()
    {
        screenOne.SetActive(false);
        if (screenTwo != null)
        {
            screenTwo.SetActive(true);
            screenTwo.transform.localScale = Vector3.zero;
            screenTwo.transform.DOScale(1, ScreenAnimationTime);
        }
    }

    private void ActivateOneInArray(GameObject[] objAr, int actid)
    {
        print(actid);
        foreach (GameObject g in objAr)
        {
            g.SetActive(false);
        }
        objAr[actid].SetActive(true);
    }

    public void ShowFakeLoading ()
    {
        FakeLoadingObject.SetActive(true);
        fakeLoadingProgressbar.fillAmount = 0;
        //fakeLoadingProgressbar.DOFillAmount(1f, fakeLoadingTime);
        Invoke("HideFakeLoading", fakeLoadingTime);
    }

    public void HideFakeLoading ()
    {
        FakeLoadingObject.SetActive(false);
    }

    bool unlockeverything = false;

    public void UnlockEverythingPanel()
    {
        if (!env)
        {
            OpenNewScreen(MainMenuPanel, EnviromentPanel);
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_levelselection();
            }
        }
        else
            OpenNewScreen(EnviromentPanel, MainMenuPanel);
        env = !env;

    }
    public void AdsCallerNext()
    {
        Adscaller.instance.on_levelselection();
    }
    public void UnlockEverything()
    {
        GameAppManager.instance.Buy_UnlockAll();
    }
    public void UnlockAllLevels()
    {
        GameAppManager.instance.Buy_UnlockAll_Levels();
    }
    public void UnlockPlayers()
    {
        GameAppManager.instance.Buy_UnlockAll_Players();
    }
    public void unlockRemoveAds()
    {
        GameAppManager.instance.Buy_noAds();
    }
}
