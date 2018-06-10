using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Collections;

public class MessageController : MonoBehaviour {

	public CamController camController;

	IEnumerator Start () {
		yield return new WaitForSeconds (3f);
		// Create UDP client
		int receiverPort = 1999;
		UdpClient receiver = new UdpClient (receiverPort);
		receiver.BeginReceive (DataReceived, receiver);
	}

	// This is called whenever data is received
	private void DataReceived (IAsyncResult ar) {
		
		UdpClient c = (UdpClient)ar.AsyncState;
		IPEndPoint receivedIpEndPoint = new IPEndPoint (IPAddress.Any, 0);
		Byte [] receivedBytes = c.EndReceive (ar, ref receivedIpEndPoint);

		string packet = System.Text.Encoding.UTF8.GetString (receivedBytes, 0, 20);
		if (packet.Contains ("_")) {
			HandleButton (packet);
		} else {
			HandlePosition (packet);
		}

		// Restart listening for udp data packages
		c.BeginReceive (DataReceived, ar.AsyncState);
	}


	void HandlePosition (string angle) {
		string [] position = angle.Split (',');
		camController.SetTurnAngle (position [2]);
	}

	void HandleButton (string status) {
		camController.SetUpAngle (status);
	}
}