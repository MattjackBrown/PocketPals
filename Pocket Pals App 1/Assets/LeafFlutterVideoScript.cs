using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LeafFlutterVideoScript : MonoBehaviour {

//	public RawImage rawImage;
	public VideoPlayer videoPlayer;

	public bool why = false;

	// Use this for initialization
	void Start () {
		StartCoroutine (PlayVideo ());
	}

	IEnumerator PlayVideo () {

			videoPlayer.Prepare();

			WaitForSeconds wait = new WaitForSeconds(0.5f);

			while (!videoPlayer.isPrepared)
			{
				yield return wait;
				break;
			}

			why = true;

//			rawImage.texture = videoPlayer.texture;

			videoPlayer.Play();

			videoPlayer.loopPointReached += OnVideoFinished;
	}

	void OnVideoFinished(VideoPlayer player)
	{
		// Should probably do some object pooling
//		Destroy (this);
	}
}
