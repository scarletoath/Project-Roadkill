//-----------------------------------------------------------------------
// <copyright file="FPS.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// FPS class to calculate framerate per second.
/// </summary>
public class FPS : MonoBehaviour 
{
	public static float fps = 0.0f;
	public float updateInterval = 0.5f;

	private float timeCounter = 0.0f;
	private int frameCounter  = 0;

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	private void Update()
	{
		timeCounter += Time.deltaTime;
		frameCounter++;

		// calculate fps inside the interval.
		if (timeCounter >= updateInterval)
		{
			fps = frameCounter / timeCounter;

			timeCounter = 0.0f;
			frameCounter = 0;
		}
	}
}
