using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CamController : MonoBehaviour {

	public float speed;

	private float currSpeed = 0;
	private bool shouldMove = false;
	private Vector3 tempAngle = Vector3.zero;
	private float yAngle = 0;
	private float turnAngleY = 0;
	private float lastYAngle = 0;
	private float xAngle = 0;
	private float zero = 0;
	private List<float> zeroList = new List<float>();
	private float tareNum = 0;

	IEnumerator Start () {
		Application.targetFrameRate = 60;
		//cleared controlled camera just in case 
		Wrld.Api.Instance.CameraApi.ClearControlledCamera ();
		//wait for map to load
		yield return new WaitForSeconds (2f);
		shouldMove = true;
	}

	// Update is called once per frame
	void Update () {
		if (shouldMove && !zero.Equals(0)) {
			//make speed change with x angle
			currSpeed = speed + ClampAngle (transform.eulerAngles.x);
			transform.Translate (Vector3.forward * Time.deltaTime * currSpeed);

			//set up/down angle
			tempAngle.x = Mathf.LerpAngle (tempAngle.x, xAngle, Time.deltaTime * 1f);

			//set turn angle
			if (yAngle < zero - 3 && yAngle < lastYAngle) {
				turnAngleY -= 5;
			} else if (yAngle > zero + 3 && yAngle > lastYAngle) {
				turnAngleY += 5;
			}

			tempAngle.y = Mathf.LerpAngle (tempAngle.y, turnAngleY, Time.deltaTime * 1f);

			transform.eulerAngles = tempAngle;
			lastYAngle = yAngle;
		}

//#if UNITY_EDITOR
//	if (Input.GetKey (KeyCode.UpArrow)){
//		transform.eulerAngles += new Vector3 (-1, 0, 0);
//	} else if (Input.GetKey (KeyCode.DownArrow)){
//		transform.eulerAngles += new Vector3 (1, 0, 0);
//	} else if (Input.GetKey (KeyCode.LeftArrow)) {
//		transform.eulerAngles += new Vector3 (0, -1, 0);
//	} else if (Input.GetKey (KeyCode.RightArrow)) {
//		transform.eulerAngles += new Vector3 (0, 1, 0);
//	}
//#endif

	}

	public void SetTurnAngle (string value) {
		if (tareNum < 20) {
			tareNum++;
			zeroList.Add (float.Parse (value));
		} else if (zero.Equals(0)) {
			zero = zeroList.Average ();
		} else {
			yAngle = float.Parse (value);
		}
	}

	public void SetUpAngle (string message) {
		if (message.Contains ("GREEN_DOWN")) {
			xAngle = 30f;
		} else if (message.Contains ("GREEN_UP")) {
			xAngle = 0;
		} else if (message.Contains ("RED_UP")) {
			xAngle = 0;
		} else if (message.Contains ("RED_DOWN")) {
			xAngle = -30;
		}
	}

	float ClampAngle (float angle) {
		if (angle > 180) {
			angle = Mathf.Clamp (((360 - angle) * -1), -50, 0);
			return angle;
		} else if (angle < 0) {
			return 0;
		} else {
			return angle * 2;
		}
	}
}
