using System.Collections;
using UnityEngine;

public class DeerCreature : Creature {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Vector3 diff = this.transform.position - PlayerController.Position;
		diff.y = 0;
		if ( diff.magnitude < 20.0f ) {
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
		transform.rotation = lookAway * this.transform.rotation;

		for ( float i=0 ; i < 10 ; i += Time.deltaTime * 8 ) {
			if ( IsDead ) yield break;
			transform.position += direction.normalized * 8 * Time.deltaTime;
			yield return null;
		}

		transform.Rotate ( new Vector3 ( 0 , 1 , 0 ) , 180f );
	}

	protected override void DoOnCollisionEnter ( Collision Collision ) {
		if ( Collision.gameObject.name != "Terrain" && !IsDead ) {
			Destroy ( gameObject , 1.0f );
			IsDead = true;
			PlayerController.SplatterBlood ();
		}
	}

}