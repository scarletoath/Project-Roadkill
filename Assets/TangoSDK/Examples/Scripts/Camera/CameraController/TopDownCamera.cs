//-----------------------------------------------------------------------
// <copyright file="TopDownCamera.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Used to show follow the target object from a
/// top-down perspective.
/// </summary>
public class TopDownCamera : IBaseCamera 
{
    private float m_counter = 0.0f;
    private Quaternion m_startRotation;
    private Quaternion m_endRotation;

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
        m_smoothTime = 0.5f;
        m_startRotation = transform.rotation;
        m_endRotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
    }

    /// <summary>
    /// Update the Top-Down Camera.
    /// </summary>
    public override void Update() 
    {
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

        if(m_counter <= m_smoothTime)
        {
            m_counter += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(m_startRotation, 
                                                                m_endRotation, 
                                                                m_counter/m_smoothTime);
        }
        else
        {
            transform.rotation = m_endRotation;
        }
    }

    /// <summary>
    /// On disable call back
    /// reset lerp time counter.
    /// </summary>
    private void OnDisable()
    {
        m_counter = 0.0f;
    }
}
