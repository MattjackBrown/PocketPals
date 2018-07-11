using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {

	public GameObject startingUI;

	GameObject currentUI;

	void Start () {

		currentUI = startingUI;

	}

	public void OpenUI (GameObject nextUI) {

		nextUI.SetActive (true);

		currentUI.GetComponent<Animator> ().SetBool ("isDisplayed", false);
		nextUI.GetComponent<Animator> ().SetBool ("isDisplayed", true);

		currentUI = nextUI;
	}
}
