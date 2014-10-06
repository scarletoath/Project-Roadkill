using UnityEngine;

public class OstrichCreature : Creature {

	// Use this for initialization
	override protected void Start () {
		ChangeState ( CreatureState.Moving );
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}

	protected override bool CanMove {
		get {
			return rigidbody.isKinematic;
		}
	}

	protected override bool DoOnCollisionEnter ( Collision Collision ) {
		return !( rigidbody.isKinematic = !base.DoOnCollisionEnter ( Collision ) );
	}

}