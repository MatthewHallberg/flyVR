using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wrld {
	public class CamController : MonoBehaviour {

		public float speed;

		private float currSpeed = 0;
		private bool shouldMove = false;

		IEnumerator Start () {
			Application.targetFrameRate = 60;
			Api.Instance.CameraApi.ClearControlledCamera ();
			yield return new WaitForSeconds (2f);
			//wait for map to load
			shouldMove = true;
		}

		// Update is called once per frame
		void Update () {
			if (shouldMove) {
				//make speed change with x angle
				currSpeed = speed + ClampAngle (transform.eulerAngles.x);
				transform.Translate (Vector3.forward * Time.deltaTime * currSpeed);
			}

#if UNITY_EDITOR

		if (Input.GetKey (KeyCode.UpArrow)){
			transform.eulerAngles += new Vector3 (-1, 0, 0);
		} else if (Input.GetKey (KeyCode.DownArrow)){
			transform.eulerAngles += new Vector3 (1, 0, 0);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.eulerAngles += new Vector3 (0, -1, 0);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			transform.eulerAngles += new Vector3 (0, 1, 0);
		}

#endif
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
}
