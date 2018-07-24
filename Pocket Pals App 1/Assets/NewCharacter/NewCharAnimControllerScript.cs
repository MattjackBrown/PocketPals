using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharAnimControllerScript : MonoBehaviour {

	public Animator animator;

	public GameObject net, bins;

	string isWalking = "isWalking";
	string isRunning = "isRunning";
	string isBins = "isBins";
	string isDab = "isDab";
	string isFloss = "isFloss";
	string isNet = "isNet";
	string isStar = "isStar";


	void ResetPoses () {
		animator.SetBool (isBins, false);
		animator.SetBool (isDab, false);
		animator.SetBool (isFloss, false);
		animator.SetBool (isNet, false);
		animator.SetBool (isStar, false);

		net.SetActive (false);
		bins.SetActive (false);
	}

	public void Walk () {
		animator.SetBool (isWalking, true);
		animator.SetBool (isRunning, false);
		ResetPoses ();
	}

	public void Run () {
		animator.SetBool (isRunning, true);
		animator.SetBool (isWalking, false);
		ResetPoses ();
	}

	public void Idle () {
		animator.SetBool (isWalking, false);
		animator.SetBool (isRunning, false);
		ResetPoses ();
	}

	public void Bins () {
		ResetPoses ();
		animator.SetBool (isBins, true);

		bins.SetActive (true);
	}

	public void Dab () {
		ResetPoses ();
		animator.SetBool (isDab, true);
	}

	public void Floss () {
		ResetPoses ();
		animator.SetBool (isFloss, true);
	}

	public void Net () {
		ResetPoses ();
		animator.SetBool (isNet, true);

		net.SetActive (true);
	}

	public void Star () {
		ResetPoses ();
		animator.SetBool (isStar, true);
	}

}
