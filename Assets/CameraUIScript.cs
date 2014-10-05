using UnityEngine;
using System.Collections;

public class CameraUIScript : Singleton<CameraUIScript> {
    public static string speartext = "Spear\nLevel\n1";
    public static string WSPDtext = "WSPD\nx1";
    public static string quietfoottext = "Sneak\nOff";
    public static string achievements = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 40, 60), speartext);
        GUI.Label(new Rect(70, 20, 40, 60), WSPDtext);
        GUI.Label(new Rect(120, 20, 40, 60), quietfoottext);

        GUI.Label(new Rect(Screen.width-120, 20, 100, Screen.height), achievements);
    }
}
