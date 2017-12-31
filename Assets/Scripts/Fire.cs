using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum FireCastType {
	Center,
	Left,
	Right,
	Up,
	Down
}

public class Fire : NetworkBehaviour {

	[SerializeField]
	protected float lifeTime = 1f;

	private float timer = 0f;

	protected void Update() {
		if (!isServer) {
			return;
		}

		timer += Time.deltaTime;

		if (timer >= lifeTime) {
			// NetworkServer.Destroy (gameObject);
		}
	}

	public void Destroy() {
		if (isServer) {
			NetworkServer.Destroy (gameObject);
		}
	}

	protected void OnTriggerEnter2D(Collider2D c) {

		if (c.GetComponent<PlayerManager> ()) {
			PlayerManager pm = c.GetComponent<PlayerManager> ();

			if (pm.hasAuthority && !pm.Dead && !pm.Invincible) {
				pm.CmdSetDeathFlag ();
				pm.rb.velocity = Vector2.zero;
			}
		}
	}
}
