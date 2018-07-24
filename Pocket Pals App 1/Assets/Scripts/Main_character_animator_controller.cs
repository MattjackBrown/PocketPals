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
		Star
	}

	public Pose selectedPose = Pose.None;
	float poseTimer;
	float timeToPose = 2.0f;
	bool waiting = true;

	// Use this for initialization
	void Start ()
    {
		poseTimer = 0.0f;

		// For Debugging
//		selectedPose = Pose.Net;
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
			
			newCharAnimator.Walk ();
			poseTimer = 0.0f;
			waiting = true;

        } else {

			if (waiting) {

				newCharAnimator.Idle ();

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

					}
				} else {
					
					poseTimer += Time.deltaTime;
				}
			}
        }
    }
}
