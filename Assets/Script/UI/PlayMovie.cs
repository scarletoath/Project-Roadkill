using UnityEngine;

public class PlayMovie : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Handheld.PlayFullScreenMovie ( "Credits.mp4" , Color.black , FullScreenMovieControlMode.CancelOnInput );
	}

	// Update is called once per frame
	void Update () {

	}

}