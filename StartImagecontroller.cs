using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartImagecontroller : MonoBehaviour {
	public GameObject startimage, AddLoder,PrivacyObj,NativeBan;
	string filePath = "",imagePath="";
	Sprite tempaddsprite;
	public Sprite AccImg, GameImg;
	public static int firstcount = 0;
	// Use this for initialization
	void Awake(){
//		PlayerPrefs.DeleteAll ();
	
	}
	void Start () {

		if (firstcount==0)
		{
			firstcount++;
			if (PlayerPrefs.GetInt("GDPRConsentAd", 0)==0)
			{

			}
			else if (PlayerPrefs.GetInt("GDPRConsentAd")==1)
			{
				PrivacyObj.SetActive(false);
				startimage.SetActive(true);
				AddLoder.SetActive(true);
				startimage.GetComponent<Animator>().SetTrigger("fadeout");
				Invoke("companyimagefadein", 5f);
			}
		}
		else
		{
			SceneManager.LoadScene(1);
		}
	}
    public void On_NativeBanRun() {
        NativeBan.SetActive(true);
    }
	public void On_AgreeButton(){
		PlayerPrefs.SetInt ("GDPRConsentAd",1);
		PrivacyObj.SetActive (false);
		startimage.SetActive (true);
		AddLoder.SetActive (true);
		startimage.GetComponent<Animator> ().SetTrigger ("fadeout");
		Invoke("companyimagefadein",6f);
	}

	////////////////////////////// ******** Privacy Policy ********** ////////////////////////////////////

	public void On_PrivacyButton(){
		Application.OpenURL ("https://zeldagamers123.blogspot.com/2021/12/zelda-gamers-privacy-policy.html");
	}

	////////////////////////////// ******** Privacy Policy ********** ////////////////////////////////////

	void companyimagefadein(){
		startimage.GetComponent<Animator> ().SetTrigger ("fadein");
		Invoke ("load_gameimage",4f);
	}
	void load_gameimage(){
		startimage.GetComponent<Image> ().sprite = GameImg;
		startimage.GetComponent<Animator> ().SetTrigger ("fadeout");
		Invoke("gameimagefadein",10f);
	}
	void gameimagefadein(){
		startimage.GetComponent<Animator> ().SetTrigger ("fadein");
		SceneManager.LoadScene (1);
	}
		
}
