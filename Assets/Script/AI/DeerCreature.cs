using UnityEngine;

public class DeerCreature : Creature {

	public float DetectionRange = 20.0f;
	public float EscapeDistance = 10.0f;

	private Vector3 EscapeDir;
	private float CurrentEscapeDistance;

	private Vector3 TempVec;
	private float DotProduct;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}

	protected override void DoIdle () {
		UseState ();

		CheckPlayerProximity ();
	}

	protected override void DoEscapeFrom () {
		CheckPlayerProximity ();

		if ( IsStateJustChanged ) {
			CurrentEscapeDistance = 0;

			IsStateJustChanged = false;
		}

		transform.forward = Vector3.RotateTowards (
			transform.forward ,
			EscapeDir ,
			MaxTurnSpeed * Time.deltaTime ,
			0.0f
		);

		transform.Translate ( 0 , 0 , MoveSpeed * Time.deltaTime , Space.Self );
		CurrentEscapeDistance += MoveSpeed * Time.deltaTime;

		if ( CurrentEscapeDistance >= EscapeDistance ) {
			ChangeState ( CreatureState.Looking , PlayerController.Object );
		}
	}

	protected override void DoLookAt () {
		UseState ();

		CheckPlayerProximity ();

		// TempVec = world displacement of target from this creature

		TempVec = CurrentTarget.transform.position - transform.position;
		TempVec.y = 0;

		// Idle if looking directly at player
		if ( Vector3.Angle ( transform.forward , TempVec ) == 0 ) {
			ChangeState ( CreatureState.Idle );

			return;
		}

		transform.forward = Vector3.RotateTowards (
			transform.forward ,
			TempVec ,
			MaxTurnSpeed * Time.deltaTime ,
			0.0f
		);
	}

	protected override void DoOnCollisionEnter ( Collision Collision ) {
		if ( !IsDead && PlayerController.IsPlayerObject ( Collision.gameObject ) ) {
			Destroy ( gameObject , DestroyTime );
			IsDead = true;

			PlayerController.SplatterBlood ();
		}
	}

	private void CheckPlayerProximity () {
		EscapeDir = transform.position - PlayerController.Position;
		EscapeDir.y = 0;

		// Escape if player is in frontal cone of sight and too near
		if ( EscapeDir.sqrMagnitude < DetectionRange * DetectionRange ) {
			// TempVec = local displacement from this creature

			TempVec = transform.InverseTransformDirection ( EscapeDir );
			DotProduct = Vector3.Dot ( TempVec , Vector3.forward );

			if ( DotProduct < 0 ) {
				TempVec.y = 0;

				ChangeState ( CreatureState.Escaping , PlayerController.Object );
			}
		}
	}

}