using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AceResolutionManager : MonoBehaviour
{


	public static int count = 0;

	void OnEnable ()
	{
		count++;

		Debug.Log (SystemInfo.systemMemorySize);
	}

	void Start ()
	{

		if (count != 1)
			return;

		Application.targetFrameRate = 40;

		Debug.Log ("Dected screen width is :" + Screen.width);
		#if UNITY_ANDROID || UNITY_WSA
		if (Screen.width > 1300) {
			SetResolution (1280, 720);
		} else {
			SetResolution ((int)(Screen.width / 1.55f), (int)(Screen.height / 1.55f));
		}

		//quality based on memory
		if (SystemInfo.systemMemorySize > 2200) { 
			QualitySettings.SetQualityLevel (2);
		} else {
			QualitySettings.SetQualityLevel (0);
		}
		#endif

	}

	void SetResolution (int width, int height)
	{
		Debug.Log ("supported res lenght :" + Screen.resolutions.Length);
	 
		Screen.SetResolution (width, height, true);
		 

	}

 


}
