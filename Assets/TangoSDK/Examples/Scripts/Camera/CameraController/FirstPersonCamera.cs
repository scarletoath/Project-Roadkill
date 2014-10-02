﻿//-----------------------------------------------------------------------
// <copyright file="FirstPersonCamera.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Used to show follow the target object from a first
/// person perspective.
/// </summary>
public class FirstPersonCamera : IBaseCamera
{
    private bool m_isCameraLocked = false;

    /// <summary>
    /// Set camera initial parameters.
    /// </summary>
    /// <param name="targetObj"> Reference to the target game object.</param>
    /// <param name="offset"> Position to maintain while following the
    /// target object.</param>
    public override void SetCamera(GameObject targetObject,
                                   Vector3 offset)
    {
        m_targetObject = targetObject;
        m_offset = offset;
        m_smoothTime = 0.05f;
    }

    /// <summary>
    /// Update the first person camera.
    /// </summary>
    public override void Update()
    {
        transform.LookAt(m_targetObject.transform.position + 
                                     m_targetObject.transform.forward);
        Vector3 endPosition = m_targetObject.transform.position + m_offset;
        float newPositionY = Mathf.SmoothDamp(transform.position.y, 
                                              endPosition.y, 
                                              ref m_velocityY, 
                                              m_smoothTime);
        float newPositionX = Mathf.SmoothDamp(transform.position.x, 
                                              endPosition.x, 
                                              ref m_velocityX, 
                                              m_smoothTime);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, 
                                              endPosition.z, 
                                              ref m_velocityZ, 
                                              m_smoothTime);
        transform.position = new Vector3(newPositionX, 
                                                     newPositionY, 
                                                     newPositionZ);

        if(m_isCameraLocked)
        {
            transform.rotation = m_targetObject.transform.rotation;
            return;
        }
        float dist = Vector3.Distance(transform.position,
                                      m_targetObject.transform.position);
        if(dist < 0.4f)
        {
            m_smoothTime = 0.0f;
            m_isCameraLocked = true;
        }
    }

    /// <summary>
    /// On disable call back
    /// reset the smoothtime and lock camera flag.
    /// </summary>
    private void OnDisable()
    {
        m_smoothTime = 0.2f;
        m_isCameraLocked = false;
    }
}
