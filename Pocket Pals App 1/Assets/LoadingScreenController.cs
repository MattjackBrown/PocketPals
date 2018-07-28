using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {

	public Sprite beAwareImage;
	public Slider loadingBar;
	public UIAnimationManager animManager;
	public static LoadingScreenController Instance { set; get;}

	Image image;
	bool loadingStarted, isLoggedIn = false;
	float loadingBarValue = 0.0f;
	float timeToLoad = 32.0f;

	void Start(){Instance = this;}

	void Awake () 
	{
		
		image = GetComponent<Image> ();
		loadingStarted = false;

	}

	void LateUpdate()
	{

	}

    public void ResetBar()
    {
        loadingStarted = false;
        isLoggedIn = false;
        loadingBarValue = 0;
    }

	// Update is called once per frame
	void Update ()
    {
		

        if (TouchHandler.Instance.IsDebug)
        {
            BackgroundMusic.Instance.StartBackgroundMusic();
            CameraController.Instance.StartZoomIn();


			this.transform.parent.gameObject.SetActive(false);
        }
        if (loadingStarted) {

			if (isLoggedIn || (!isLoggedIn && loadingBarValue < 0.8f)) {

				if (loadingBarValue > 1.2f) {

					animManager.LoadingBarFinished ();
					BackgroundMusic.Instance.StartBackgroundMusic ();
					CameraController.Instance.StartZoomIn ();
                    if (LocalDataManager.Instance.GetData().IsFirstLogIn == 1)
                    {
                        FirstLogonSequence();
                    }
					this.gameObject.SetActive (false);

				} else {

					loadingBarValue += Time.deltaTime / timeToLoad;
					loadingBar.value = loadingBarValue;
				}
			}
		}
	}

    public void FirstLogonSequence()
    {
        foreach (ItemData id in AssetManager.Instance.GetStartItems(3, 5, 10, 1))
        {
            LocalDataManager.Instance.AddItem(id);
        }
         TutorialManager.Instance.StartMainTutorial();
        ServerDataManager.Instance.HasDoneFirstLogin();
    }

	public void SetBeAwareImage () {

		image.sprite = beAwareImage;
		loadingBar.gameObject.SetActive (true);
		loadingStarted = true;

	}

	public void AllowToComplete () {

		timeToLoad = 2.0f;
		isLoggedIn = true;

		// Stops the journal button being clickable during the initial camera swoop in
		CameraController.Instance.DisableJournalButton ();
    }
}
