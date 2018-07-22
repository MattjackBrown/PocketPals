using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewFinderScaleScript : MonoBehaviour {

	public Animator animator;

	void OnEnable () {

		animator.enabled = true;
		animator.SetBool ("showViewfinder", true);
	}

	public void DisableAnimator () {
		animator.enabled = false;
	}
}
