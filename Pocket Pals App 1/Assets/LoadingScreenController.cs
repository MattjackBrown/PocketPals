using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Text loadingDescription;
    private int currentIter = 0;
    private Dictionary<int, string> loadingMessages= new Dictionary<int, string>();

    private bool hasData = false;
    private bool hasMap = false;

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
        hasMap = false;
        hasData = false;
        loadingBarValue = 0;
    }

	// Update is called once per frame
	void Update ()
    {
		

   //     if (TouchHandler.Instance.IsDebug)
   //     {
   //         BackgroundMusic.Instance.StartBackgroundMusic();
   //         CameraController.Instance.StartZoomIn();


			//this.transform.parent.gameObject.SetActive(false);
   //     }
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

    public int AddLoadingMessage(string message)
    {
        currentIter++;
        loadingMessages.Add(currentIter, message);
        UpdateLoadingText();
        return currentIter;
    }

    public void TicketLoaded(int ticket)
    {
        loadingMessages.Remove(ticket);       
        UpdateLoadingText();
    }

    public void UpdateLoadingText()
    {

        string loadmessage = "Loading: ";

        if (loadingMessages.Count <1)
        {
            return;
        }

        for (int i =0; i <  loadingMessages.Count -1; i++)
        {
            string s = loadingMessages.ElementAt(i).Value;
            loadmessage += s + ", ";
        }
        loadmessage += loadingMessages.ElementAt(loadingMessages.Count - 1).Value + "...";
        loadingDescription.text = loadmessage;
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

    public void DataLoaded()
    {
        hasData = true;
        CheckAllowToComplete();
    }

    public void MapLoaded()
    {
        hasMap = true;
        CheckAllowToComplete();
    }

	public void CheckAllowToComplete (bool arSkip = false)
    {
        if (!arSkip)
        {
            if (!hasData || !hasMap) return;
        }

        loadingDescription.text = "Finalising...";

        timeToLoad = 2.0f;
		isLoggedIn = true;

		// Stops the journal button being clickable during the initial camera swoop in
		CameraController.Instance.DisableJournalButton ();
    }
}
