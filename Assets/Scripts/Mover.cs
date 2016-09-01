using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{

	//amount of time it moves for in seconds
	public float moveTime = 0.05f;
	public float speed = 1;

	float acceleration;
	public float currSpeed;

	Touch currTouch;

	float timeLeft = 0;
	bool moving = false;
	bool keepMoving = false;

	public bool useBursts = true;
	bool startingMove = false;
	bool endingMove = false;

	public Transform trans;


	// Use this for initialization
	void Start ()
	{
		//Cardboard.SDK.TapIsTrigger = true;
		acceleration = speed / (.333f * moveTime);
		if (trans == null) trans = transform.GetChild (0).transform;

	}

	public void move ()
	{
		
	}


	// Update is called once per frame
	void Update ()
	{

		/*if (Input.touchCount > 0) {
			transform.position += trans.transform.forward * speed * Time.deltaTime;
		}*/

		if (useBursts && Input.GetKeyDown (KeyCode.Space)) {
			if (!moving) {
				moving = true;
				timeLeft = moveTime;
				currSpeed = 0;
			} else {
				keepMoving = true;
			}
		}

		if (!moving && Input.GetKey (KeyCode.Space) && !useBursts) {
			moving = true;
			startingMove = true;
		}
		if (moving && !endingMove && !Input.GetKey (KeyCode.Space) && !useBursts) {
			endingMove = true;
		}
		if (Input.touchCount > 0) {
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				moving = true;
				startingMove = true;
			} else if (Input.GetTouch (0).phase == TouchPhase.Ended && !useBursts) {
				endingMove = true;
			} 
		}

		if (moving && useBursts) {
			//makes it move fast at first and then slow down a lil bit
			print("bursting");
			timeLeft -= Time.deltaTime;
			if (timeLeft >= moveTime * 2 / 3) {
				currSpeed += 2 * acceleration * Time.deltaTime;
			} else {
				if (currSpeed > 0) {
					currSpeed -= (currSpeed / timeLeft) * Time.deltaTime;
				} else {
					currSpeed = 0;
				}
			}
			trans.position += trans.forward * currSpeed * Time.deltaTime;

			if (timeLeft <= 0) {
				if (keepMoving) {
					keepMoving = false;
				} else {
					moving = false;
				}
			}
		} 
		else if (moving && !useBursts) {
			//makes it move fast at first and then slow down a lil bit
			if (startingMove || endingMove) {
				timeLeft -= Time.deltaTime;
			}

			if (startingMove) {
				currSpeed += 2 * acceleration * Time.deltaTime;
				if (currSpeed >= speed) {
					currSpeed = speed;
					startingMove = false;
				}
			} else if (endingMove) {
				if (currSpeed > 0f) {
					currSpeed -= acceleration * Time.deltaTime;
				} else {
					currSpeed = 0;
					endingMove = false;
					moving = false;
				}
			}

			if (moving)
				trans.position += trans.forward * currSpeed * Time.deltaTime;
		}

		

	}
}
