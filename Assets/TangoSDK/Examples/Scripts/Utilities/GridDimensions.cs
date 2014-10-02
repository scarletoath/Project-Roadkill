//-----------------------------------------------------------------------
// <copyright file="GridDimensions.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Displays the status messages like sparse map saved, loaded succesfully
/// Disables the starter screen when record/ load is pressed.
/// </summary>
public class GridDimensions : MonoBehaviour
{
    // status message string for updates
    private string m_status;

    /// <summary>
    /// Update the status.
    /// </summary>
    private void Update()
    {
        _UpdateStatus(" grid Length = 0.5m \n" + " grid Width = 0.5m");
    }

    /// <summary>
    /// Used to display things on GUI.
    /// </summary>
    private void OnGUI()
	{
        GUI.Label(new Rect(Screen.width - 225, Screen.height - 100, 1300, 100),
            "<size=20>Scale: \n" + m_status + "</size>");
	}

    /// <summary>
    /// Update debug status messages here.
    /// </summary>
    /// <param name="newStatus">The new debug message you want to see on screen.</param>
    private void _UpdateStatus(string newStatus)
	{
        m_status = newStatus;
	}
}
