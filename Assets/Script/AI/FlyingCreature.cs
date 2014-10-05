using UnityEngine;

public class FlyingCreature : Creature {

	public float FlyingHeightMin;
	public float FlyingHeightMax;

	private float FlyingHeight;

	// Use this for initialization
	override protected void Start () {
		base.Start ();

		FlyingHeight = Random.Range ( FlyingHeightMin , FlyingHeightMax );
	}

	// Update is called once per frame
	override protected void Update () {
		base.Update ();

		if ( !IsDead ) {
			TempVec = transform.position;
			TempVec.y = FlyingHeight;
			transform.position = TempVec;
		}
	}
}
