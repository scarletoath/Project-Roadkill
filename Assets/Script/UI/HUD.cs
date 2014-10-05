﻿using UnityEngine;

public class HUD : Singleton<HUD> {

	private const string DESC_SPEAR_LEVEL = "Spear\nLevel\n";
	private const string DESC_MOVE_SPEED = "Speed\nx ";
	private const string DESC_QUIET_FEET = "Sneak\n";

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnGUI () {
		GUI.Label ( new Rect ( 20 , 20 , 40 , 60 ) , DESC_SPEAR_LEVEL + GameManager.GetBonuses ().SpearLevel );
		GUI.Label ( new Rect ( 70 , 20 , 40 , 60 ) , DESC_MOVE_SPEED + GameManager.GetBonuses ().MoveSpeedMultiplier );
		GUI.Label ( new Rect ( 120 , 20 , 40 , 60 ) , DESC_QUIET_FEET + ( GameManager.GetBonuses ().HasQuietFeet ? GameManager.GetBonuses ().GetQuietFeetTimeRemaining ().ToString () : "Off" ) );

		GUI.Label ( new Rect ( Screen.width - 120 , 20 , 100 , Screen.height ) , GetKillCountAchievementText () );
	}

	private string GetKillCountAchievementText () {
		if ( Achievements.IsKillCountAchievement ( GameManager.NumDeadCreatures ) ) {
			return ( ( Achievements.KillCount ) GameManager.NumDeadCreatures ).ToString ().Replace ( '_' , ' ' );
		}
		else {
			return string.Empty;
		}
	}

}