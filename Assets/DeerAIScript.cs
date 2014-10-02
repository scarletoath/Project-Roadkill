using UnityEngine;
using System.Collections;

public class DeerAIScript : MonoBehaviour {
	public GameObject caveman;
	// Use this for initialization
	void Start () {
	
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.gameObject.name == "Caveman") {
						Destroy (this.gameObject);

			//caveman.transform.Find ("blood").gameObject.GetComponent<ParticleSystem> ().Stop();
						caveman.transform.Find ("blood").gameObject.GetComponent<ParticleSystem> ().Play ();
				}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 diff = this.transform.position - caveman.transform.position;
		if (diff.magnitude < 10.0f) 
		{
			this.transform.position += diff.normalized * 2 * Time.deltaTime; 
		}
	}
}
