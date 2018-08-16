using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour {

	public RawImage rawImage;
	public VideoPlayer videoPlayer;
	public AudioSource audioSource;

	public UIAnimationManager animManager;
	public LoadingScreenController loadingScreen;

	public Animator skipButtonAnimator;

	// Use this for initialization
	void Start () {
		StartCoroutine (PlayVideo ());
	}

	IEnumerator PlayVideo ()
	{
        if (!TouchHandler.Instance.IsDebug)
        {

            videoPlayer.Prepare();

            WaitForSeconds wait = new WaitForSeconds(0.5f);

            while (!videoPlayer.isPrepared)
            {

                yield return wait;
                break;
            }

			if (LocalDataManager.Instance != null) {

				if (LocalDataManager.Instance.InAR) 
				{
					LocalDataManager.Instance.InAR = false;

					// Returning from the AR scene so deactivate the static PPal gameObject
					GlobalVariables.ARPocketPAl.SetActive (false);

					BackgroundMusic.Instance.StartBackgroundMusic();
					CameraController.Instance.StartZoomIn();

					ServerDataManager.Instance.OverrideLogin ();

					RemoveVideoImageObject ();

					this.gameObject.SetActive(false);

					yield break;
				}
			}

            rawImage.texture = videoPlayer.texture;

            videoPlayer.Play();
            audioSource.Play();

			StartCoroutine (DelayShowSkipButton ());

            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else { animManager.IntroFinished(); }
	}

	IEnumerator DelayShowSkipButton()
	{
		yield return new WaitForSeconds(3.1f);

		skipButtonAnimator.SetBool ("showButton", true);
	}

	void OnVideoFinished(VideoPlayer player)
	{
		player.Stop ();
		animManager.IntroFinished ();
	}

	public void SkipVideo (VideoPlayer player) {

		player.Stop ();
		animManager.IntroFinished ();
		player.gameObject.GetComponent<AudioSource> ().Stop ();
	}

	public void RemoveVideoImageObject () {

	//	rawImage.gameObject.SetActive (false);
		Destroy (rawImage.gameObject);
		Destroy (videoPlayer.gameObject);
	}
}
