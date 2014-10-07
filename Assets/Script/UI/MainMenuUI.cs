﻿using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	public void OnStartClick () {
		StartCoroutine ( LoadLevelAfterSound () );
	}

	public void OnHelpClick () {

	}

	public void OnCreditsClick () {
		Handheld.PlayFullScreenMovie ( "Credits.mp4" , Color.black , FullScreenMovieControlMode.CancelOnInput );
	}

	public void OnBackClick () {

	}

	public void OnTitlePlay () {
		animation [ "MainMenu - Title" ].normalizedTime = 1;
	}

	private IEnumerator LoadLevelAfterSound () {
		yield return new WaitForSeconds ( 0.330f );

		Application.LoadLevel ( "GameScene" );
	}

}