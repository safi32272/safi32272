using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

[Serializable]
public class players
{
	public GameObject playerObject;
	public int price;
	public string[] info;




}

public class PlayerSelection : MonoBehaviour, ISubmitHandler
{

	 
	public players[]
		AllPlayers;
	public Transform RotationTransform;
	public Vector3 RotationSpeed;
	public Text[] infoTexts;
	public Text priceText;
	public int playerIndex;
	public Text selectButtonText;
	public Text popUpPriceText;
	public GameObject NextMenuObject;
	public GameObject backButton;
	public GameObject selectionMenu, purchasePopUp, InsufficientFundsPopUp;


	void OnEnable ()
	{
		priceText.text = "Free";
	 
		playerIndex = 0;
		PlayerPrefs.SetInt ("Player_Pucharse_status_of_" + playerIndex, 1);
		UpdateInfo ();
	
		AceButton.buttonDown += OnButtonClick;

		purchasePopUp.SetActive (false);
		InsufficientFundsPopUp.SetActive (false);
		selectionMenu.SetActive (true);
		backButton.SetActive (true);
	
	}

	void OnDisable ()
	{
		AceButton.buttonDown -= OnButtonClick;
	}

	public void OnSubmit (BaseEventData eventData)
	{

		Debug.Log (eventData.selectedObject.name);
		OnButtonClick (eventData.selectedObject.name, null);

	}

	void OnButtonClick (System.Object obj, EventArgs args)
	{

		switch (obj.ToString ()) {
		case "Select":

			OnselectPress ();
			break;

		case "Next":
			OnNext ();
			break;
		case "Previous":
			OnPrevious ();
			break;
		case "Ok":

			InsufficientFundsPopUp.SetActive (false);
			selectionMenu.SetActive (true);
			break;

		case "Yes":
			onPlayerConfirmation ();
			purchasePopUp.SetActive (false);
			selectionMenu.SetActive (true);
			break;
		case "No":

			purchasePopUp.SetActive (false);
			selectionMenu.SetActive (true);
			break;

		}
		AceEvents.playSingleshotsounds ("Click", null);
		#if UNITY_ANDROID && !UNITY_EDITOR
		Vibration.Vibrate (5);
		#endif
	}

	void OnselectPress ()
	{
		if (selectButtonText.text.Contains ("Buy")) {

			if (AllPlayers [playerIndex].price <= TotalCurrency.getCurrencyCount ()) {

				purchasePopUp.SetActive (true);
				selectionMenu.SetActive (false);
			} else {

				InsufficientFundsPopUp.SetActive (true);
				selectionMenu.SetActive (false);
			}
  
		} else {


			NextMenuObject.SetActive (true);
			backButton.SetActive (true);
			this.gameObject.SetActive (false);
		}

	}

	void onPlayerConfirmation ()
	{

		PlayerPrefs.SetInt ("Player_Pucharse_status_of_" + playerIndex, 1);
		TotalCurrency.Static.SubtractCurrency (AllPlayers [playerIndex].price);
		UpdateInfo ();
	}

	void UpdateInfo ()
	{

		if (AceEvents.AceTweenCall != null) {
			AceEvents.AceTweenCall ("OnPlayerInfoUpdate", null);
		}

		StartCoroutine (AceHelper.waitThenCallback (0.1f, () => {
			limitBoundsForIndex ();

			priceText.text = "" + AllPlayers [playerIndex].price;//to display in selection menu
			popUpPriceText.text = "Do you want to purchase this For " + AllPlayers [playerIndex].price + "?";//o display in popup menu


			for (int i = 0; i < infoTexts.Length; i++) {
				infoTexts [i].text = AllPlayers [playerIndex].info [i];

			}

			if (PlayerPrefs.GetInt ("Player_Pucharse_status_of_" + playerIndex, 0) == 1) {
				selectButtonText.text = "Select";
				priceText.text = "Purchased";
			} else {
				selectButtonText.text = "Buy";
			}
			if (playerIndex == 0) {
				priceText.text = "Free";
			}
			AllPlayers [playerIndex].playerObject.SetActive (true);

			PlayerPrefs.SetInt ("PlayerIndex", playerIndex);
		
		}));

	}

	public void OnNext ()
	{
		playerIndex++;
		UpdateInfo ();
	}

	public void OnPrevious ()
	{
		playerIndex--;
		UpdateInfo ();
	}

	public  bool canRotate = false;
	float startMousePosX;

	void Update ()
	{
		
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			canRotate = false;
			startMousePosX = Input.mousePosition.x;
		}
		if (Input.GetKeyUp (KeyCode.Mouse0)) {
			canRotate = true;
		}

	
		if (RotationTransform != null) {

			if (canRotate) { 
				RotationTransform.Rotate (-RotationSpeed * Time.deltaTime * 0.6f);
			} else if (Input.mousePosition.x > Screen.width / 8 && Input.mousePosition.x < Screen.width / 1.14f) {
				 
				RotationTransform.Rotate (-RotationSpeed * -Time.deltaTime * (startMousePosX - Input.mousePosition.x));
				startMousePosX = Input.mousePosition.x;
			}
		}
		
	}

	void limitBoundsForIndex ()
	{
		if (playerIndex > AllPlayers.Length - 1) {
			playerIndex = 0;
			
		} else if (playerIndex < 0) {
			playerIndex = AllPlayers.Length - 1;
			
		}

		foreach (players loop in AllPlayers) {
			loop.playerObject.SetActive (false);
		}
	}
}
