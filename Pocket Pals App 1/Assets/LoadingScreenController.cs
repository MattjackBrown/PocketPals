using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {

	public Sprite beAwareImage;
	public Slider loadingBar;
	public UIAnimationManager animManager;

	Image image;
	bool loadingStarted, isLoggedIn = false;
	float loadingBarValue = 0.0f;
	float timeToLoad = 32.0f;

	void Start () {

		image = GetComponent<Image> ();
		loadingStarted = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if(TouchHandler.Instance.IsDebug) this.gameObject.SetActive(false);
        if (loadingStarted) {

			if (isLoggedIn || (!isLoggedIn && loadingBarValue < 0.8f)) {

				if (loadingBarValue > 1.2f) {

					animManager.LoadingBarFinished ();
					BackgroundMusic.Instance.StartBackgroundMusic ();
					this.gameObject.SetActive (false);

				} else {

					loadingBarValue += Time.deltaTime / timeToLoad;
					loadingBar.value = loadingBarValue;
				}
			}
		}
	}

	public void SetBeAwareImage () {

		image.sprite = beAwareImage;
		loadingBar.gameObject.SetActive (true);
		loadingStarted = true;

	}

	public void AllowToComplete () {

		timeToLoad = 4.0f;
		isLoggedIn = true;
        this.gameObject.SetActive(false);
    }
}
