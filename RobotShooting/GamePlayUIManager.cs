using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class GamePlayUIManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixerGroup sfxGroup, musicGroup;
    public AudioSource GeneralPurposeAudioSource, MusicAudioSource;
    public float CarCameraDistance = 4f, CarCameraHeight = 2f, TankCameraDistance = 10f, TankCameraHeight = 3f;
    [Header("References")]
    public Player player;
    public Transform[] PlayerSpawnPoints;
    public GameObject Player, MechRobot, MechRobotCanvas, 
        Vehicle, FlyingVehicle, RCCCamera, JoystickCanvas, PlayerCamera,
          VehicleFlyCamera, RobotCamera, Tank, PlayerHeli;
    public GameObject RobotUI, VehicleFlyControl, FlyingUpDownPannel,
        RCCCanvas, CarPanel, TankPannel, FlyingCarCanvas, FlyingCarPanel, HeliPanel,
        RobotGuidedMissileButton, Ramps, PauseButtonPanel, MinimapPanel;
    public Transform CineCamera;
    public Text LevelText, ScoreText, coinsText;
    public Image PlayerHealthImage;
    public GameObject AnimatedRobot, AnimatedRobotCamera;
    public GameObject MissionDescPanel;
    public Text MissionDescription;
    public GameObject MegaOfferPanel;
    public Player[] AllPlayerObjects;
    [Header("Buttons")]
    public GameObject HeliTransformButton;
    public GameObject TankTransformButton, CarTransformButton, FlyButton, PausePanel, LoadingScreen,
        JumpPowerButton, CarToRobotButton, HeliToRobotButton, TankToRobotButton, carRocketButton;

    public bool isVehicleMode = false, isFlyMode = false, isFlyingVehicleMode = false, 
        isRobotMode = true, isTankMode, isHelicopterMode = false;
    public Rigidbody flyingCarRigidbody, VehicleRigidbody;
    public FollowTargetCamera targetCamera;
    public RCC_Camera rccCamera;
    public Camera CarCamera, FlyingCamera;
    public TankMiniController tankController;
    public CustomCamera robotCamera;

    [Header("GameOver")]
    public GameObject gameOverSuccessStars;
    public GameObject NextButton, DoubleRewardButton, CoinsGameObject, gameOverFailStars;
    public Text CoinsText, gameOverTitle;
    public Image gameOverPanel;
    //public Sprite failSprite;
    public float GameSuccessDelay = 5f, GameFailDelay = 3f;

    [HideInInspector]
    public int numberOfKills = 0;
    //[HideInInspector]
    public int score = 0, coinsEarned = 0, PlayerHealth = 100;
    //private ThirdPersonOrbitCamCustom camCustom;

    private int requiredScore = 0;
    private int requiredKills = 0;
    public static GamePlayUIManager Instance;
    private int gameMode;
    private FlyBehaviour flyBehaviour;
    public RCC_CarControllerV3 controllerV3, tankV3Controller;
    private Camera playerCamera;
    private MoveBehaviour moveBehaviour;
    private AudioSource[] allAudioSources;

    public void DisableAllPlayerAndCamera ()
    {
        MechRobotCanvas.SetActive(false);
        VehicleFlyControl.SetActive(false);
        MinimapPanel.SetActive(false);
        PauseButtonPanel.SetActive(false);
        RobotUI.SetActive(false);
        RCCCanvas.SetActive(false);
        RCCCamera.SetActive(false);
        Player.SetActive(false);
        Vehicle.SetActive(false);
        FlyingVehicle.SetActive(false);
        RCCCamera.SetActive(false);
        SetJoystick(false);
        PlayerCamera.SetActive(false);
        VehicleFlyCamera.SetActive(false);
        RobotCamera.SetActive(false);
        Tank.SetActive(false);
        PlayerHeli.SetActive(false);
        if (Data.selectedGameMode != 0)
        {
            AnimatedRobot.SetActive(true);
            AnimatedRobotCamera.SetActive(true);
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Data.LoadData();
        gameMode = Data.selectedGameMode;
        Time.timeScale = 1;
    }

    void Start()
    {
        //camCustom = PlayerCamera.GetComponent<ThirdPersonOrbitCamCustom>();
        //flyingCarRigidbody = FlyingVehicle.GetComponent<Rigidbody>();
        //VehicleRigidbody = Vehicle.GetComponent<Rigidbody>();
        //targetCamera = VehicleFlyCamera.GetComponent<FollowTargetCamera>();
        //rccCamera = RCCCamera.GetComponent<RCC_Camera>();
        RobotGuidedMissileButton.SetActive(false);
        robotCamera = RobotCamera.GetComponent<CustomCamera>();
        flyBehaviour = Player.GetComponent<FlyBehaviour>();
        playerCamera = PlayerCamera.GetComponentInChildren<Camera>();
        controllerV3 = Vehicle.GetComponent<RCC_CarControllerV3>();
        tankV3Controller = Tank.GetComponent<RCC_CarControllerV3>();
        moveBehaviour = Player.GetComponent<MoveBehaviour>();
        GiveRobotNewPosition();
        SetupCanvas();
        CalculateRequiredScore();
        UpdatePlayerHealth();
        allAudioSources = FindObjectsOfType<AudioSource>();
        print(allAudioSources.Length);
        foreach(AudioSource auso in allAudioSources)
        {
            auso.outputAudioMixerGroup = sfxGroup;
        }
        foreach (Player p in AllPlayerObjects)
        {
            p.ChangeSkin(Data.SelectedPlayerSkin);
        }
        GeneralPurposeAudioSource.outputAudioMixerGroup = sfxGroup;
        MusicAudioSource.outputAudioMixerGroup = musicGroup;
    }

    public void PlayAudioClip (AudioClip sound)
    {
        GeneralPurposeAudioSource.PlayOneShot(sound);
    }

    public void GiveRobotNewPosition ()
    {
        int rand = Random.Range(0, PlayerSpawnPoints.Length);
        Player.transform.position = PlayerSpawnPoints[rand].position;
        MechRobot.transform.position = PlayerSpawnPoints[rand].position;
    }

    bool gameOverIsSuccess = false;
    public void GameOver (bool isSuccess)
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LevelCompletedEventLog(Data.CurrentGameMode(), Data.selectedLevel, score, isSuccess);

        gameOverIsSuccess = isSuccess;

        Invoke("DelayedGameOver", 1.5f);
    }

    public void BuyMegaOffer ()
    {
        InAppPurchasing.Instance.BuyProductID("combooffer");
    }

    public void CloseMegaOffer ()
    {
        EnableGameOverUI();
        MegaOfferPanel.SetActive(false);
    }

    public void ActivateMegaPanel ()
    {
        MegaOfferPanel.SetActive(true);
    }
    public void DelayedGameOver ()
    {
        
        if (gameOverIsSuccess)
        {
            //gameOverPanel.transform.parent.gameObject.SetActive(true);
            CoinsText.text = coinsEarned.ToString();
            Data.coins += coinsEarned;
            if (Data.selectedLevel <= 99)
                NextButton.SetActive(true);
            else
                NextButton.SetActive(false);
            DoubleRewardButton.SetActive(true);
            gameOverSuccessStars.SetActive(true);
            gameOverFailStars.SetActive(false);
            gameOverTitle.text = "MISSION COMPLETE";
            EnemiesSpawner.Instance.DisableAllEnemies();
            UnlockNextLevel();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                DoubleRewardButton.SetActive(true);
            }
            DisableAllPlayerAndCamera();
            Invoke("ActivateMegaPanel", GameSuccessDelay);
            //Invoke("EnableGameOverUI", GameSuccessDelay);
        }
        else
        {
            //gameOverPanel.transform.parent.gameObject.SetActive(true);
            //gameOverPanel.sprite = failSprite;
            gameOverSuccessStars.SetActive(false);
            gameOverFailStars.SetActive(true);
            gameOverTitle.text = "MISSION FAILED";
            NextButton.SetActive(false);
            DoubleRewardButton.SetActive(false);
            CoinsText.transform.parent.gameObject.SetActive(false);
            Invoke("EnableGameOverUI", GameFailDelay);
        }
        Data.SaveData();
    }

    public void EnableGameOverUI ()
    {
         gameOverPanel.transform.parent.gameObject.SetActive(true);
    }

    public void CalculateRequiredScore ()
    {
        LevelText.text = "Level: " + (Data.selectedLevel + 1).ToString();
        if (Data.selectedGameMode == 0 || Data.selectedGameMode == 2 ||
            Data.selectedGameMode == 4 || Data.selectedGameMode == 5)
        {
            if (Data.selectedLevel != 0)
                requiredScore = 1000 + (Data.selectedLevel * 1000);
            else
                requiredScore = 1500;
            requiredKills = 0;
            ShowMissionDescription("Mission Requirement: Destroy everything to score " + requiredScore + " points.");
        }
        else
        {
            requiredKills = 2 + Data.selectedLevel;
            requiredScore = 0;
            ShowMissionDescription("Mission Requirement: Kill " + requiredKills + " robots or destroy helicopters.");
        }
        UpdateScore();
    }

    public void UnlockNextLevel ()
    {
       switch (Data.selectedGameMode)
        {
            case 0:
                if (Data.MechModeUnlockedLevel < 99)
                {
                    Data.MechModeUnlockedLevel++;
                    //Data.MechModeSelectedLevel = Data.selectedLevel + 1;
                }
                break;
            case 1:
                if (Data.RobotModeUnlockedLevel < 99)
                    Data.RobotModeUnlockedLevel++;
                break;
            case 2:
                if (Data.CarModeUnlockedLevel < 99)
                    Data.CarModeUnlockedLevel++;
                break;
            case 3:
                if (Data.TankModeUnlockedLevel < 99)
                    Data.TankModeUnlockedLevel++;
                break;
            case 4:
                if (Data.HeliModeUnlockedLevel < 99)
                    Data.HeliModeUnlockedLevel++;
                break;
            case 5:
                if (Data.FreeModeUnlockedLevel < 99)
                    Data.FreeModeUnlockedLevel++;
                break;
        }
        
    }

    public void UpdatePlayerHealth ()
    {
        PlayerHealthImage.fillAmount = PlayerHealth / 100f;
    }

    public void UpdateScore ()
    {
        coinsText.text = coinsEarned.ToString();
        if (Data.selectedGameMode == 0 || Data.selectedGameMode == 2 ||
            Data.selectedGameMode == 4 || Data.selectedGameMode == 5)
        {
            ScoreText.text = "Score: " + score.ToString() + "/" + requiredScore.ToString();
            if (score >= requiredScore)
            {
                GameOver(true);
            }
        }else
        {
            ScoreText.text = "Kills: " + numberOfKills.ToString() + "/" + requiredKills.ToString();
            if (numberOfKills >= requiredKills)
            {
                GameOver(true);
            }
        }
    }

    public void ChangeTankCamera ()
    {
        tankController.switchCamera();
    }

    public void ShowMissionDescription (string str)
    {
        MissionDescPanel.SetActive(true);
        MissionDescription.text = str;
        Invoke("HideDescPanel", 7f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
    public void HideDescPanel ()
    {
        MissionDescPanel.SetActive(false);
    }

    //MechRobot = 0, RobotMode = 1, CarMode = 2, TankMode = 3, HeliMode = 4, free mode = 5
    private void SetupCanvas ()
    {
        switch (gameMode)
        {
            case 0:
                MechRobot.SetActive(true);
                Ramps.SetActive(false);
                MechRobotCanvas.SetActive(true);
                JoystickCanvas.SetActive(true);
                break;
            case 1:
                ActivateRobotPlayer();
                FlyButton.SetActive(true);
                break;
            case 2:
                ActivateRobotPlayer();
                SetActiveTransformRobot();
                JoystickCanvas.SetActive(true);
                //CarTransformButton.SetActive(true);
                break;
            case 3:
                ActivateRobotPlayer();
                TransformRobotToTank(true);
                //TankTransformButton.SetActive(true);
                break;
            case 4:
                ActivateRobotPlayer();
                TransformToHelicopter(true);
                JoystickCanvas.SetActive(true);
                //HeliTransformButton.SetActive(true);
                break;
            case 5:
                ActivateRobotPlayer();
                HeliTransformButton.SetActive(true);
                TankTransformButton.SetActive(true);
                CarTransformButton.SetActive(true);
                break;
        }
    }

    private void ActivateRobotPlayer ()
    {
        Player.SetActive(true);
        RobotCamera.SetActive(true);
        if (gameMode == 5 || gameMode == 1)
        {
            JoystickCanvas.SetActive(true);
            RobotUI.SetActive(true);
        }

    }

    public void NextLevel ()
    {
        if (Data.selectedLevel <= 99)
        {
            if (Adscaller.instance != null && Adscaller.instance)
            {
                Adscaller.instance.on_fail_sucess_commercial_AD();
            }
            if (Analyticts.Instance != null)
                Analyticts.Instance.NextFromMissionResultEventLog();
            Data.selectedLevel++;
            Data.SaveData();
            Time.timeScale = 0;
            LoadingScreen.SetActive(true);
            SceneManager.LoadScene("City");
        }
    }

    public void PauseGame ()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.PauseEventLog();
        Time.timeScale = 0;
        PausePanel.SetActive(true);
    }

    public void ResumeGame ()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
    }

    public void Restart ()
    {
        if (Adscaller.instance != null && Adscaller.instance)
        {
            Adscaller.instance.on_fail_sucess_commercial_AD();
        }
        Time.timeScale = 0;
        LoadingScreen.SetActive(true);
        SceneManager.LoadScene("City");
    }

    public void BackToMainMenu ()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.BackToMenuFromMissionResultEventLog();
        if (Adscaller.instance != null && Adscaller.instance)
        {
            Adscaller.instance.on_pause_AD();
        }
        Time.timeScale = 0;
        LoadingScreen.SetActive(true);
        SceneManager.LoadScene("Garage");
    }

    public void DoubleReward ()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("DoubleRewardRequested");
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
            Analyticts.Instance.LogResourceEvent(true, coinsEarned * 2, "DoubleRewardOnLevelComplete");
        coinsEarned += coinsEarned;
        CoinsText.text = coinsEarned.ToString();
        Data.coins += coinsEarned;
        DoubleRewardButton.SetActive(false);
        Data.SaveData();
        MainAdsManagerController.instance.removeall_rewardevent();
    }

    // Update is called once per frame
    void Update()
    {
        if (isVehicleMode && gameMode == 5)
        {
            if (controllerV3.speed > 1f)
            {
                CarToRobotButton.SetActive(false);
            }
            else
            {
                CarToRobotButton.SetActive(true);
            }
        }
        if (isTankMode && gameMode == 5)
        {
            if (tankV3Controller.speed > 1f)
                TankTransformButton.SetActive(false);
            else
                TankTransformButton.SetActive(true);
        }
    }

    public void PowerJump ()
    {
        if (moveBehaviour.isGrounded())
        {
            moveBehaviour.PowerJump();
            SetJoystick(false);
            RobotUI.SetActive(false);
        }
    }

    public void PowerJumpCompleted ()
    {

        SetJoystick(true);
        RobotUI.SetActive(true);
    }

    public void FlyModeButton()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("SwitchRobotFlyMode");
        isFlyMode = !isFlyMode;
        JumpPowerButton.SetActive(!isFlyMode);
        FlyingUpDownPannel.SetActive(isFlyMode);
        RobotGuidedMissileButton.SetActive(isFlyMode);
        flyBehaviour.Fly();
        

        TankTransformButton.SetActive(!isFlyMode);
        HeliTransformButton.SetActive(!isFlyMode);
        CarTransformButton.SetActive(!isFlyMode);
        EnemiesSpawner.Instance.GameModeChanged();
    }

    public void VehicleFlyMode(bool isActive)
    {
        
        if (isActive)
        {
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("SwitchdToCarFlyMode");
            FlyingVehicle.transform.position = Vehicle.transform.position;
            FlyingVehicle.transform.rotation = Vehicle.transform.rotation;
            HeliPanel.SetActive(false);
            FlyingCarPanel.SetActive(true);
            Vehicle.SetActive(false);
            FlyingVehicle.SetActive(true);
            targetCamera.SwitchCamera(true);
            targetCamera.Target = FlyingVehicle.transform;
            flyingCarRigidbody.velocity = VehicleRigidbody.velocity;
            VehicleFlyControl.SetActive(true);
            RCCCanvas.SetActive(false);
            RCCCamera.SetActive(false);
            VehicleFlyCamera.SetActive(true);
            VehicleFlyCamera.transform.position = RCCCamera.transform.position;
            VehicleFlyCamera.transform.rotation = RCCCamera.transform.rotation;
            SetJoystick(true);
            isFlyingVehicleMode = true;
            isVehicleMode = false;
        }
        else
        {
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("SwitchdToCarMode");
            Vehicle.transform.position = FlyingVehicle.transform.position;
            Vehicle.transform.rotation = FlyingVehicle.transform.rotation;
            FlyingVehicle.SetActive(false);
            Vehicle.SetActive(true);
            VehicleRigidbody.velocity = flyingCarRigidbody.velocity;
            VehicleFlyControl.SetActive(false);
            RCCCanvas.SetActive(true);
            RCCCamera.transform.position = VehicleFlyCamera.transform.position;
            RCCCamera.transform.rotation = VehicleFlyCamera.transform.rotation;
            RCCCamera.SetActive(true);
            VehicleFlyCamera.SetActive(false);
            SetJoystick(false);
            isFlyingVehicleMode = false;
            isVehicleMode = true;
        }
        EnemiesSpawner.Instance.GameModeChanged();
    }

    [Header("Robot")]
    public GameObject TransformationRobot;
    public GameObject HeliTransformationObject, TankTransformationObject;
    public GameObject[] VehicleParts;
    public GameObject[] RobotBodyParts;
    public GameObject[] HeliBodyParts, TankParts;
    private Animator TransformAnimator, HeliTransformationAnimator;
    public Image Joystick, joystickPar;
    public float lookatspeed = 5f;

    public void TransformToHelicopter (bool isRobot)
    {
        if (isRobot)
        {
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("TransformToHelicopter");
            RobotUI.SetActive(false);
            SetJoystick(false);
            HeliTransformationObject.transform.position = Player.transform.position;
            HeliTransformationObject.transform.rotation = Player.transform.rotation;
            PlayerHeli.transform.position = Player.transform.position;
            PlayerHeli.transform.rotation = Player.transform.rotation;
            PlayerHeli.SetActive(true);
            PlayerHeli.GetComponent<HelicopterController>().enabled = false;
            SetActive(HeliBodyParts, false);
            VehicleFlyCamera.SetActive(true);
            targetCamera.SwitchCamera(false);
            targetCamera.Target = PlayerHeli.transform;
            FlyingCamera.farClipPlane = 0.5f;
            Player.SetActive(false);
            HeliTransformationObject.SetActive(true);
            HeliTransformationObject.transform.DOScale(new Vector3(3f, 2f, 1.5f), 1.5f);
            HeliTransformationAnimator = HeliTransformationObject.GetComponent<Animator>();
            HeliTransformationAnimator.Play("RobotToHeli");
            RobotCamera.SetActive(false);
            CineCamera.gameObject.SetActive(true);
            CineCamera.GetComponent<TransformationCinematicCamera>().target = HeliTransformationObject.transform;
            CineCamera.position = playerCamera.transform.position;
            CineCamera.rotation = playerCamera.transform.rotation;
            //CineCamera.DOMove(player.cineCameraTarget.position, 0.5f);
            CineCamera.DOMove(PlayerHeli.GetComponent<Player>().cineCameraTarget.position, 0.5f);
            VehicleFlyCamera.transform.position = RobotCamera.transform.position;
            VehicleFlyCamera.transform.rotation = RobotCamera.transform.rotation;
            isHelicopterMode = true;
            isRobotMode = false;
        }else
        {
            isHelicopterMode = false;
            FlyingCarCanvas.SetActive(false);
            isRobotMode = true;
            SetJoystick(false);
            Player.transform.position = PlayerHeli.transform.position;
            Player.transform.rotation = PlayerHeli.transform.rotation;
            HeliTransformationObject.transform.position = PlayerHeli.transform.position;
            HeliTransformationObject.transform.rotation = PlayerHeli.transform.rotation;
            Player.GetComponent<Rigidbody>().useGravity = false;
            Player.SetActive(true);
            SetActive(RobotBodyParts, false);
            PlayerHeli.SetActive(false);
            RobotCamera.SetActive(true);
            RobotCamera.transform.position = FlyingCamera.transform.position;
            RobotCamera.transform.rotation = FlyingCamera.transform.rotation;
            playerCamera.farClipPlane = 0.5f;
            HeliTransformationObject.SetActive(true);
            HeliTransformationObject.transform.DOScale(new Vector3(1f, 1f, 1f), 1.5f);
            HeliTransformationAnimator.Play("HeliToRobot");
            VehicleFlyCamera.SetActive(false);
            CineCamera.gameObject.SetActive(true);
            CineCamera.GetComponent<TransformationCinematicCamera>().target = HeliTransformationObject.transform;
            CineCamera.position = FlyingCamera.transform.position;
            CineCamera.rotation = FlyingCamera.transform.rotation;
            CineCamera.DOMove(player.cineCameraTarget.position, 0.5f);
        }
        EnemiesSpawner.Instance.GameModeChanged();
    }

    public void HeliToRobotAnimationComplete ()
    {
        CineCamera.DOMove(RobotCamera.transform.position, 0.5f).OnComplete(HeliToRobotCameraOnComplete);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = Player.transform;
    }

    public void HeliToRobotCameraOnComplete ()
    {
        SetJoystick(true);
        RobotUI.SetActive(true);
        Player.GetComponent<Rigidbody>().useGravity = true;
        SetActive(RobotBodyParts, true);
        playerCamera.farClipPlane = 1000f;
        CineCamera.gameObject.SetActive(false);
        HeliTransformationObject.SetActive(false);
    }

    public void RobotToHeliAnimationCompleted ()
    {
        CineCamera.DOMove(FlyingCamera.transform.position, 0.5f).OnComplete(RobotToHeliCameraOnComplete);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = PlayerHeli.transform;
    }

    public void RobotToHeliCameraOnComplete ()
    {
        if (gameMode == 5)
        {
            HeliToRobotButton.SetActive(true);
        }
        else
        {
            HeliToRobotButton.SetActive(false);
        }
        SetActive(HeliBodyParts, true);
        FlyingCamera.farClipPlane = 1000f;
        PlayerHeli.GetComponent<HelicopterController>().enabled = true;
        SetJoystick(true);
        FlyingCarCanvas.SetActive(true);
        FlyingCarPanel.SetActive(false);
        HeliPanel.SetActive(true);
        HeliTransformationObject.SetActive(false);
        CineCamera.gameObject.SetActive(false);
    }

    public void TransformRobotToTank(bool isRobot)
    {
        
        if (isRobot)
        {
            if (Analyticts.Instance != null)
                Analyticts.Instance.LogEvent("TransformToTank");
            RobotUI.SetActive(false);
            SetJoystick(false);
            TankTransformationObject.transform.position = Player.transform.position;
            TankTransformationObject.transform.rotation = Player.transform.rotation;
            Tank.transform.position = Player.transform.position;
            Tank.transform.rotation = Player.transform.rotation;
            Tank.SetActive(true);
            SetActive(TankParts, false);
            RCCCamera.SetActive(true);
            CarCamera.farClipPlane = 0.5f;
            Player.SetActive(false);
            TankTransformationObject.SetActive(true);
            TransformAnimator = TankTransformationObject.GetComponent<Animator>();
            TransformAnimator.Play("RobotToTank");
            RobotCamera.SetActive(false);
            CineCamera.gameObject.SetActive(true);
            CineCamera.GetComponent<TransformationCinematicCamera>().target = TankTransformationObject.transform;
            CineCamera.position = playerCamera.transform.position;
            CineCamera.rotation = playerCamera.transform.rotation;
            CineCamera.DOMove(player.cineCameraTarget.position, 0.5f);
            RCCCamera.transform.position = RobotCamera.transform.position;
            RCCCamera.transform.rotation = RobotCamera.transform.rotation;
            isTankMode = true;
            isRobotMode = false;
            rccCamera.TPSDistance = TankCameraDistance;
            rccCamera.TPSHeight = TankCameraHeight;
        } else
        {
            TankTransformationObject.transform.position = Tank.transform.position;
            TankTransformationObject.transform.rotation = Tank.transform.rotation;
            Player.transform.position = Tank.transform.position;
            Player.transform.rotation = Tank.transform.rotation;
            Tank.SetActive(false);
            RCCCanvas.SetActive(false);
            Player.SetActive(true);
            SetActive(RobotBodyParts, false);
            RCCCamera.SetActive(false);
            RobotCamera.SetActive(true);
            playerCamera.farClipPlane = 0.5f;           
            TankTransformationObject.SetActive(true);
            TransformAnimator = TankTransformationObject.GetComponent<Animator>();
            TransformAnimator.Play("TankToRobot");
            Player tankPlayer = Tank.GetComponent<Player>();
            CineCamera.gameObject.SetActive(true);
            CineCamera.GetComponent<TransformationCinematicCamera>().target = Tank.transform;
            CineCamera.position = RCCCamera.transform.position;
            CineCamera.rotation = RCCCamera.transform.rotation;
            CineCamera.DOMove(tankPlayer.cineCameraTarget.position, 0.5f);
            isTankMode = false;
            isRobotMode = true;;
        }
        EnemiesSpawner.Instance.GameModeChanged();
    }

    public void RobotToTankAnimationCompleted ()
    {
        CineCamera.DOMove(RCCCamera.transform.position, 0.5f).OnComplete(RobotToTankOnCompleted);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = Player.transform;
    }

    public void RobotToTankOnCompleted()
    {
        SetActive(TankParts, true);
        TankTransformationObject.SetActive(false);
        CarCamera.farClipPlane = 1000f;
        CineCamera.gameObject.SetActive(false);
        RCCCanvas.SetActive(true);
        CarPanel.SetActive(false);
        TankPannel.SetActive(true);
        if (gameMode == 5)
            TankToRobotButton.SetActive(true);
        else
            TankToRobotButton.SetActive(false);
    }

    public void TankToRobotAnimationComplete ()
    {
        CineCamera.DOMove(RobotCamera.transform.position, 0.5f).OnComplete(TankToRobotOnCompleted);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = player.MissileTarget;
    }

    public void TankToRobotOnCompleted ()
    {
        SetActive(RobotBodyParts, true);
        CineCamera.gameObject.SetActive(false);
        playerCamera.farClipPlane = 1000f;
        SetJoystick(true);
        TankTransformationObject.SetActive(false);
        RobotUI.SetActive(true);
    }

    public void SetActiveTransformToCar ()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("TransformToCar");
        TransformationRobot.SetActive(true);
        TransformAnimator = TransformationRobot.GetComponent<Animator>();
        TransformAnimator.Play("TransformToRobot");
        Vehicle.SetActive(false);
        RCCCanvas.SetActive(false);

        TransformationRobot.transform.position = Vehicle.transform.position;
        TransformationRobot.transform.rotation = Vehicle.transform.rotation;

        Player.transform.position = Vehicle.transform.position;
        Player.transform.rotation = Vehicle.transform.rotation;
        Player.SetActive(true);
        SetActive(RobotBodyParts, false);

        PlayerCamera.transform.position = RCCCamera.transform.position;
        PlayerCamera.transform.rotation = RCCCamera.transform.rotation;
        PlayerCamera.SetActive(true);
        RCCCamera.SetActive(false);

        playerCamera.farClipPlane = 0.4f;
        CineCamera.gameObject.SetActive(true);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = Vehicle.transform;
        CineCamera.position = RCCCamera.transform.position;
        CineCamera.rotation = RCCCamera.transform.rotation;
        CineCamera.DOMove(player.cineCameraTarget.position, 0.5f);

        isRobotMode = true;
        isVehicleMode = false;
    }

    public void SwitchToPlayerRobot () 
    {
        SetActive(RobotBodyParts, true);
        TransformationRobot.SetActive(false);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = player.MissileTarget;
        CineCamera.DOMove(PlayerCamera.transform.position, 0.5f).OnComplete(OnCompleteRobotTransformation);
    }

    private void OnCompleteRobotTransformation ()
    {
        SetJoystick(true);
        RobotUI.SetActive(true);
        playerCamera.farClipPlane = 300f;
        CineCamera.gameObject.SetActive(false);
    }

    public void SetActiveTransformRobot ()
    {
        if (Analyticts.Instance != null)
            Analyticts.Instance.LogEvent("TransformToRobotFromCar");
        TransformationRobot.SetActive(true);
        TransformAnimator = TransformationRobot.GetComponent<Animator>();
        TransformAnimator.Play("TransformToCar");
        TransformationRobot.transform.position = Player.transform.position;
        TransformationRobot.transform.rotation = Player.transform.rotation;
        Vehicle.transform.position = TransformationRobot.transform.position;
        Vehicle.transform.rotation = TransformationRobot.transform.rotation;
        Vehicle.SetActive(true);
        SetActive(VehicleParts, false);
        Player.SetActive(false);
        
        RobotUI.SetActive(false);
        RCCCamera.transform.position = PlayerCamera.transform.position;
        RCCCamera.transform.rotation = PlayerCamera.transform.rotation;
        RCCCamera.SetActive(true);
        RobotCamera.SetActive(false);
        SetJoystick(false);
        isRobotMode = false;
        isVehicleMode = true;
        rccCamera.TPSDistance = CarCameraDistance;
        rccCamera.TPSHeight = CarCameraHeight;

        CineCamera.gameObject.SetActive(true);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = player.MissileTarget;
        CarCamera.farClipPlane = 0.4f;
        CineCamera.position = PlayerCamera.transform.position;
        CineCamera.rotation = PlayerCamera.transform.rotation;
        CineCamera.DOMove(player.cineCameraTarget.position, 0.5f);
    }

    public void SwitchOnRCCCar ()  // Add this method to transformint robot animation event
    {
        SetActive(VehicleParts, true);
        TransformationRobot.SetActive(false);
        CineCamera.GetComponent<TransformationCinematicCamera>().target = Vehicle.transform;
        CineCamera.DOMove(RCCCamera.transform.position, 0.5f).OnComplete(switchToCarCamera);
    }

    private void switchToCarCamera ()
    {
        RCCCanvas.SetActive(true);
        CarPanel.SetActive(true);
        TankPannel.SetActive(false);
        CineCamera.gameObject.SetActive(false);
        CarCamera.farClipPlane = 300f;
    }

    public void SetActive (GameObject[] objs, bool state)
    {
        foreach (GameObject g in objs)
            g.SetActive(state);
    }

    private void SetJoystick (bool status)
    {
        Joystick.enabled = status;
        joystickPar.enabled = status;
    }

    public void TransformPlayer()
    {
        if (isVehicleMode)
        {
            Player.transform.position = Vehicle.transform.position;
            Player.transform.rotation = Vehicle.transform.rotation;
            Player.SetActive(true);
            Vehicle.SetActive(false);
            RCCCamera.SetActive(false);
            RobotCamera.SetActive(true);
            RCCCanvas.SetActive(false);
            RobotUI.SetActive(true);
            JoystickCanvas.SetActive(true);
            isRobotMode = true;
            isVehicleMode = false;
        }
        else if (isRobotMode)
        {
            Vehicle.transform.position = Player.transform.position;
            Vehicle.transform.rotation = Player.transform.rotation;
            Player.SetActive(false);
            Vehicle.SetActive(true);
            RobotUI.SetActive(false);
            RCCCanvas.SetActive(true);
            CarPanel.SetActive(true);
            TankPannel.SetActive(false);
            RCCCamera.transform.position = PlayerCamera.transform.position;
            RCCCamera.transform.rotation = PlayerCamera.transform.rotation;
            RCCCamera.SetActive(true);
            RobotCamera.SetActive(false);
            JoystickCanvas.SetActive(false);
            isRobotMode = false;
            isVehicleMode = true;
            rccCamera.TPSDistance = CarCameraDistance;
            rccCamera.TPSHeight = CarCameraHeight;
        }
    }
}
