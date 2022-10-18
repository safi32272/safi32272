using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
	private int activePanel;

	public GameObject[] panels;

	public Text[] tabTexts;

	public Color passiveColor;

	public Color activeColor;

	public Tweening panelTween;

	public Vector2 closePos;

	public Vector2 openPos;

	private bool open = true;

	public GameObject mapCam;

	private bool playerDead;

	private AudioSource sounder;

	public AudioClip openSound;

	public AudioClip closeSound;

	public GameObject fadeObject;

	private void Start()
	{
		sounder = GetComponent<AudioSource>();
	}

	public void OpenDirectPanel(int i)
	{
		if (playerDead)
		{
			return;
		}
		if (open)
		{
			if (activePanel == i)
			{
				OpenClosePanel();
			}
			else
			{
				ActivatePanel(i);
			}
			return;
		}
		OpenClosePanel();
		if (activePanel != i)
		{
			ActivatePanel(i);
		}
	}

	public void OpenClosePanel()
	{
		open = !open;
		sounder.PlayOneShot((!open) ? closeSound : openSound);
		panelTween.SetTargetPos((!open) ? closePos : openPos, !open);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && !playerDead)
		{
			if (!open)
			{
				ActivatePanel(4);
			}
			OpenClosePanel();
		}
	}

	public void ActivatePanel(int i)
	{
		if (activePanel != i && !playerDead)
		{
			sounder.PlayOneShot(closeSound);
			tabTexts[activePanel].color = passiveColor;
			panels[activePanel].SetActive(value: false);
			activePanel = i;
			tabTexts[activePanel].color = activeColor;
			panels[activePanel].SetActive(value: true);
			mapCam.SetActive(i == 2);
		}
	}

	public void PlayerDead()
	{
		playerDead = true;
		if (open)
		{
			OpenClosePanel();
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void RestartGame()
	{
		fadeObject.SetActive(value: true);
		Invoke("LoadScene", 0.15f);
	}

	private void LoadScene()
	{
		sounder.PlayOneShot(closeSound);
		SceneManager.LoadScene(0);
	}

	public void MoreGames()
	{
		Application.OpenURL("market://search?q=pub:Eternity Game Arts");
	}
}
