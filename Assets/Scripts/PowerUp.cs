using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

	protected virtual void OnTriggerEnter2D(Collider2D c) {
		if (!isServer) {
			return;
		}

		if (c.GetComponent<PlayerManager> ()) {
			PlayerManager pm = c.GetComponent<PlayerManager> ();
			TriggerAction (pm);
			Destroy (gameObject);
			NetworkServer.Destroy (gameObject);
		}
	}

	protected virtual void TriggerAction (PlayerManager pm) {
	}

}
