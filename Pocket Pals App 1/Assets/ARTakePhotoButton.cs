using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTakePhotoButton : MonoBehaviour {


	public GameObject UIcanvas;
	public CaptureAndSave snapShot;

	public void TakePhoto () {

		UIcanvas.SetActive (false);

		StartCoroutine (WaitThenTakePhoto ());
	}

	IEnumerator WaitThenTakePhoto ()
	{
		yield return new WaitForSeconds(0.2f);

//		CaptureAndSave snapShot = GameObject.FindObjectOfType<CaptureAndSave>();

		snapShot.CaptureAndSaveToAlbum(ImageType.PNG);

		StartCoroutine (WaitThenRenableUI ());

	}


	IEnumerator WaitThenRenableUI ()
	{
		yield return new WaitForSeconds (1.0f);

		UIcanvas.SetActive (true);

	}
}
