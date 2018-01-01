using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirePowerUp : PowerUp {

	protected override void TriggerAction (PlayerManager pm)
	{
		base.TriggerAction (pm);

		pm.IncreaseExplosionRange ();
	}

}
