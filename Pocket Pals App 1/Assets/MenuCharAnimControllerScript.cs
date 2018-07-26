using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharAnimControllerScript : MonoBehaviour {

	public Animator animator;

	public GameObject net, bins;

	string DemoBins = "DemoBins";
	string DemoDab = "DemoDab";
	string DemoFloss = "DemoFloss";
	string DemoNet = "DemoNet";
	string DemoStar = "DemoStar";
	string DemoHands = "DemoHands";


	public void ResetPoses () {
		animator.SetBool (DemoBins, false);
		animator.SetBool (DemoDab, false);
		animator.SetBool (DemoFloss, false);
		animator.SetBool (DemoNet, false);
		animator.SetBool (DemoStar, false);
		animator.SetBool (DemoHands, false);

		net.SetActive (false);
		bins.SetActive (false);
	}

	public void DemoIdlePose () {
		ResetPoses ();
	}

	public void DemoBinsPose () {
		ResetPoses ();
		animator.SetBool (DemoBins, true);

		bins.SetActive (true);

	}

	public void DemoDabPose () {
		ResetPoses ();
		animator.SetBool (DemoDab, true);

	}

	public void DemoFlossPose () {
		ResetPoses ();
		animator.SetBool (DemoFloss, true);

	}

	public void DemoNetPose () {
		ResetPoses ();
		animator.SetBool (DemoNet, true);

		net.SetActive (true);

	}

	public void DemoStarPose () {
		ResetPoses ();
		animator.SetBool (DemoStar, true);

	}

	public void DemoHandsPose () {
		ResetPoses ();
		animator.SetBool (DemoHands, true);

	}
}
