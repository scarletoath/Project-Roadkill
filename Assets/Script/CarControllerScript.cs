using UnityEngine;
using System.Collections;

public class CarControllerScript : MonoBehaviour {
	Vector3 velocity;
	Vector3 angular_velocity;
	// Use this for initialization
	void Start () {
		velocity = new  Vector3 (0,0,0);
		angular_velocity = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		getKeyBoardControls ();
	}

	void OnCollisionEnter(Collision col)
	{
	
	}

	void getKeyBoardControls()
	{

		if (Input.GetKey (KeyCode.W)) 
		{
			this.transform.position += this.transform.rotation * Vector3.forward * Time.deltaTime * 10	;
			//this.rigidbody.AddForce(this.transform.rotation * Vector3.forward * 5000);
		}
		if (Input.GetKey (KeyCode.S)) {
			this.transform.position += this.transform.rotation * Vector3.back  * Time.deltaTime * 10;
			//this.rigidbody.AddForce(this.transform.rotation * Vector3.back* 5000);
		}
		if (Input.GetKey (KeyCode.A)) {
			this.transform.rotation = Quaternion.AngleAxis(-0.5f,new Vector3(0,1,0)) * this.transform.rotation;
			//this.rigidbody.AddForceAtPosition(this.transform.rotation *  Vector3.left * 0.5f, this.transform.position + this.transform.rotation * (Vector3.forward * 2.5f + Vector3.left*1.5f),ForceMode.Acceleration);
		}
		if (Input.GetKey (KeyCode.D)) {
			this.transform.rotation = Quaternion.AngleAxis(0.5f,new Vector3(0,1,0)) * this.transform.rotation;
			//this.rigidbody.AddForceAtPosition(this.transform.rotation *  Vector3.right * 0.5f, this.transform.position + this.transform.rotation * (Vector3.forward * 2.5f + Vector3.right*1.5f),ForceMode.Acceleration);
		}
	}
}
