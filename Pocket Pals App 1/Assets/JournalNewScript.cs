using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, playerModel, cameraJournalPositionObject, cameraLookAtPositionObject, mapPlayerObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition;
	Vector3 cameraMapPlayerOffset, playerMapCharPosition;
	Quaternion cameraMapRotation;
    public Image xpSlider,lastSeen, mostSeen,rarestSeen, highestSeen;
    public Text xpText;
    public Text level;

    void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;
	}

	public void Init () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

		// Record the camera position to return back to after the menu
		cameraMapPlayerOffset = mainCamera.gameObject.transform.position - mapPlayerObject.gameObject.transform.position;
		cameraMapRotation = mainCamera.gameObject.transform.rotation;

		// Position all used assets
		mainCamera.gameObject.transform.position = cameraJournalPosition;
		mainCamera.gameObject.transform.LookAt (cameraLookAtPosition);

		playerModel.SetActive (true);

		// TODO Create new control scheme
		TouchHandler.Instance.CharCustControls ();

        GameData gd = LocalDataManager.Instance.GetData();
        xpSlider.fillAmount = LocalDataManager.Instance.GetData().GetPercentageExp();
        xpText.text = LocalDataManager.Instance.GetData().EXP.ToString();
        level.text = LocalDataManager.Instance.GetData().GetLevel().ToString();

        ChangeStatsImage(mostSeen, gd.Inventory.GetMostCaught());
        ChangeStatsImage(highestSeen, gd.Inventory.GetHighestLevel());
        ChangeStatsImage(rarestSeen, gd.Inventory.GetRarest());
        ChangeStatsImage(lastSeen, gd.Inventory.GetMostRecentData());

        CharacterCustomisation.Instance.Init();
    }

    private void ChangeStatsImage(Image i, PocketPalData pd)
    {
        if (pd == null) return;

        i.sprite = AssetManager.Instance.GetScreenShot(pd.ID);
    }

    public void ExitBackToMap () {

		// Return the Camera
		mainCamera.gameObject.transform.position = mapPlayerObject.gameObject.transform.position + cameraMapPlayerOffset;
		mainCamera.transform.rotation = cameraMapRotation;

		// Deactivate the player model used for the menu
		playerModel.SetActive (false);

		TouchHandler.Instance.MapControls ();

        gameObject.SetActive(false);
	}

	void Update () {
		
		// TODO Lerp movement between inspect positions

		// TODO Buttons trigger camMovement lerps - set up a controlScheme
		// TODO controlScheme allows rotating player. (prefab.transform.position.z as on it's side)
		// Message Triss when done to link up to server data, shop bought bool, grey out shop button

//		mainCamera.gameObject.transform.position = cameraJournalPositionObject.transform.position;
//		mainCamera.gameObject.transform.LookAt (cameraLookAtPositionObject.gameObject.transform.position);
		
	}
}
