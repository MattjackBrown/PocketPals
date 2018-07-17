using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {

	public Sprite beAwareImage;
	public Slider loadingBar;
	public UIAnimationManager animManager;

	Image image;
	bool loadingStarted;
	float loadingBarValue = -0.5f;
	float timeToLoad = 4.0f;


	void Start () {

		image = GetComponent<Image> ();
		loadingStarted = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (loadingStarted) {

			if (loadingBarValue > 1.2f) {

				animManager.LoadingBarFinished ();

				this.gameObject.SetActive (false);
				//loadingStarted = false;

			} else {

				loadingBarValue += Time.deltaTime / timeToLoad;
				loadingBar.value = loadingBarValue;
			}

		}
	}

	public void SetBeAwareImage () {

		image.sprite = beAwareImage;
		loadingBar.gameObject.SetActive (true);
		loadingStarted = true;

	}
}
