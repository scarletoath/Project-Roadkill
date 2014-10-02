//-----------------------------------------------------------------------
// <copyright file="VIOController.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// This is a basic movement controller based on
/// pose estimation returned from the VIO Provider.
/// </summary>
public class VIOController : MonoBehaviour
{
    public float m_movementScale = 1.0f;

    private Tango.VIOProvider.VIOStatus m_vioStatus;

    /// <summary>
    /// Check the vio status of the controller.
    /// </summary>
    /// <returns> A code representing the current status.</returns>
    public Tango.VIOProvider.CAPIOdometryStatusCodes GetVIOStatusCode()
    {
        return m_vioStatus.status_code;
    }

    /// <summary>
    /// Check to see if this controller is relocalized.
    /// </summary>
    /// <returns> True if it is relocalized, false otherwise.</returns>
    public bool IsRelocalized()
    {
        return (m_vioStatus.relocalized == 1) ? true : false;
    }

    /// <summary>
    /// Initialize the controller.
    /// </summary>
    private void Awake()
    {
        // TODO : This is a small hack to prevent render batch failure
        // FIX THIS!
        m_vioStatus = new Tango.VIOProvider.VIOStatus(-1);
    }

    /// <summary>
    /// Get the latest pose.
    /// </summary>
    private void Update()
    {
        if (Tango.VIOProvider.GetLatestPose(ref m_vioStatus))
        {
            transform.position = m_vioStatus.translation * m_movementScale;
            transform.rotation = m_vioStatus.rotation;
        }
    }

    /// <summary>
    /// Enable the AR screen.
    /// </summary>
	private void _EnableScreen()
	{
	}

    /// <summary>
    /// Disable the AR screen.
    /// </summary>
	private void _DisableScreen()
	{
	}
}