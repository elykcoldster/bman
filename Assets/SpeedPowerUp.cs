using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUp {

	protected override void TriggerAction (PlayerManager pm)
	{
		base.TriggerAction (pm);

		pm.playerMovement.SetMoveSpeed (pm.playerMovement.MoveSpeed + 0.5f);
	}

}
