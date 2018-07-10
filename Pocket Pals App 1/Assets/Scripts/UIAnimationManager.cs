using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {

	public void OpenUI (Animator anim) {

		anim.SetBool ("isDisplayed", true);

	}

	public void CloseUI (Animator anim) {

		anim.SetBool ("isDisplayed", false);

	}
}
