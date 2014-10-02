using UnityEngine;
using System.Collections;

public class OstrichAIScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(this.rigidbody.isKinematic)
		this.transform.position += Vector3.forward * Time.deltaTime * 30;
	}

	void OnCollisionEnter(Collision col)
	{
		this.rigidbody.isKinematic = false;
	}
}
