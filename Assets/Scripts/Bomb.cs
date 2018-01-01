using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {

	private CircleCollider2D cc;

	[SyncVar]
	public float lifeTime = -1f;
	[SyncVar]
	private int range = 1;

	private float bombTimer;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CircleCollider2D> ();
		bombTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeTime < 0f || !hasAuthority) {
			return;
		}

		bombTimer += Time.deltaTime;

		if (bombTimer >= lifeTime) {
			CmdExplode ();
		}
	}

	public void Init (float t, int r) {
		lifeTime = t;
		range = r;
	}

	protected void OnTriggerExit2D(Collider2D c) {
		if (!hasAuthority) {
			return;
		}
		cc.isTrigger = false;
	}

	[Command]
	private void CmdExplode() {
		GameManager gm = GameManager.Instance;
		GameObject centerObject = Instantiate (gm.explosionCenter);

		centerObject.transform.position = transform.position;
		// centerObject.GetComponent<Fire> ().Init (range, FireCastType.Center);
		NetworkServer.Spawn (centerObject);


		SpawnDirectional (Vector3.up, gm.explosionV, gm.explosionUp);
		SpawnDirectional (Vector3.down, gm.explosionV, gm.explosionDown);
		SpawnDirectional (Vector3.right, gm.explosionH, gm.explosionRight);
		SpawnDirectional (Vector3.left, gm.explosionH, gm.explosionLeft);

		NetworkServer.Destroy (gameObject);
	}

	private void SpawnDirectional(Vector3 dir, GameObject midPrefab, GameObject endPrefab) {
		int layerMask = 1 << LayerMask.NameToLayer ("Terrain");

		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2(dir.x, dir.y), range * 0.5f, layerMask);

		int r;

		if (hit.transform == null) {
			r = range;
		} else {
			r = (int)((hit.distance + 0.25f) * 2);

			if (hit.transform.tag == "Block") {
				r--;
			} else {
				if (hit.transform.GetComponent<Brick> ()) {
					hit.transform.GetComponent<Brick> ().SetDestroyAnimation ();
				}
			}

		}

		for (int i = 0; i < r; i++) {
			GameObject obj;

			if (i == r - 1) {
				obj = Instantiate (endPrefab);
			} else {
				obj = Instantiate (midPrefab);
			}

			obj.transform.position = transform.position + dir * 0.5f * (i + 1);
			NetworkServer.Spawn (obj);
		}
	}
}
