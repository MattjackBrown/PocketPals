using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_character_animator_controller : MonoBehaviour
{

	public NewCharAnimControllerScript newCharAnimator;

    public float speed = 2.0f;
    public float rotationSpeed = 75.0f;

    public GPS playerMove;

	public enum Pose
	{
		None,
		Bins,
		Dab,
		Floss,
		Net,
		Star,
		HandsHips
	}

	public Pose selectedPose = Pose.None;
	float poseTimer = 0.0f;
	float timeToPose = 1.0f;
	bool waiting = true;
    public float runSpeed;

	// Use this for initialization
	void Start ()
    {
		poseTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {

        float translation = Input.GetAxis ("Vertical")*speed;
        float rotation = Input.GetAxis ("Horizontal")*rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;


        transform.Translate (0, 0, translation);
        transform.Rotate (0, rotation, 0); 

        if(translation != 0 || playerMove.Moving)
        {
            if (playerMove.currentSpeed >= runSpeed)
            {
                newCharAnimator.Run();
            }
            else
            {
                newCharAnimator.Walk();
            }

            poseTimer = 0.0f;
			waiting = true;

        } else {

			if (waiting) {

				if (poseTimer > timeToPose) {

					waiting = false;

					switch (selectedPose) {

					case Pose.None:
						break;

					case Pose.Bins:
						newCharAnimator.Bins ();
						break;

					case Pose.Dab:
						newCharAnimator.Dab ();
						break;

					case Pose.Floss:
						newCharAnimator.Floss ();
						break;

					case Pose.Net:
						newCharAnimator.Net ();
						break;

					case Pose.Star:
						newCharAnimator.Star ();
						break;

					case Pose.HandsHips:
						newCharAnimator.HandsHips ();
						break;

					}
				} else {

					poseTimer += Time.deltaTime;

					newCharAnimator.Idle ();
					
				}
			}
        }
    }

	public void ChoosePoseNone () {
		selectedPose = Pose.None;
	}

	public void ChoosePoseBins () {
		selectedPose = Pose.Bins;
	}

	public void ChoosePoseDab () {
		selectedPose = Pose.Dab;
	}

	public void ChoosePoseFloss () {
		selectedPose = Pose.Floss;
	}

	public void ChoosePoseNet () {
		selectedPose = Pose.Net;
	}

	public void ChoosePoseStar () {
		selectedPose = Pose.Star;
	}

	public void ChoosePoseHandsHips () {
		selectedPose = Pose.HandsHips;
	}

	public void ResetTimer () {
		waiting = true;
		poseTimer = 0.0f;
	}
}
