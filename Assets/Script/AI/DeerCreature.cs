using System.Collections;
using UnityEngine;

public class DeerCreature : Creature {

	public float DetectionRange = 20.0f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Vector3 diff = transform.position - PlayerController.Position;
		diff.y = 0;

		if ( diff.sqrMagnitude < DetectionRange * DetectionRange ) {
			Vector3 localvec = Quaternion.Inverse ( transform.rotation ) * diff;
			float doty = Vector3.Dot ( localvec , Vector3.forward );
			if ( doty < 0 ) {
				localvec.y = 0;
				Quaternion rot = Quaternion.FromToRotation ( Vector3.forward , localvec );
				StopCoroutine ( "run" );
				StartCoroutine ( run ( diff , rot ) );
			}
		}
	}

	IEnumerator run ( Vector3 direction , Quaternion lookAway ) {
		transform.rotation = lookAway * transform.rotation;

		for ( float i = 0 ; i < 10 ; i += Time.deltaTime * MoveSpeed ) {
			if ( IsDead ) yield break;

			transform.position += direction.normalized * MoveSpeed * Time.deltaTime;

			yield return null;
		}

		transform.Rotate ( Vector3.up , 180f );
	}

	protected override void DoOnCollisionEnter ( Collision Collision ) {
		if ( !IsDead && PlayerController.IsPlayerObject ( Collision.gameObject ) ) {
			Destroy ( gameObject , 1.0f );
			IsDead = true;

			PlayerController.SplatterBlood ();
		}
	}

}