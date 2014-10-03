using UnityEngine;

public class OstrichCreature : Creature {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	protected override bool CanMove {
		get {
			return rigidbody.isKinematic;
		}
	}

	protected override void DoOnCollisionEnter ( Collision Collision ) {
		rigidbody.isKinematic = false;
	}

}