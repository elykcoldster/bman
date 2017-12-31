using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Brick : NetworkBehaviour {

	private Animator anim {
		get {
			return GetComponent<Animator> ();
		}
	}

	public void SetDestroyAnimation() {
		anim.SetTrigger ("destroy");
	}

	public void Destroy() {
		if (isServer) {
			NetworkServer.Destroy (gameObject);
		}
	}
}
