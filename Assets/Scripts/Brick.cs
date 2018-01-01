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

		if (Random.value < GameManager.Instance.PowerUpChance) {
			int index = Random.Range (0, GameManager.Instance.PowerUps.Count);

			PowerUp powerUp = Instantiate (GameManager.Instance.PowerUps [index]);
			powerUp.transform.position = transform.position;

			NetworkServer.Spawn (powerUp.gameObject);
		}

		if (isServer) {
			NetworkServer.Destroy (gameObject);
		}
	}
}
