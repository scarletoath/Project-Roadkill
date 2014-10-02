//-----------------------------------------------------------------------
// <copyright file="IBaseCamera.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class that all camera behaviors should
/// derive from.
/// </summary>
[RequireComponent(typeof(Camera))]
public abstract class IBaseCamera : MonoBehaviour
{
    protected GameObject m_targetObject;
    protected Vector3 m_offset;
    protected Vector3 m_lookAtPosition;

    protected float m_smoothTime = 0.3f;
    protected float m_velocityX = 0.0f;
    protected float m_velocityY = 0.0f;
    protected float m_velocityZ = 0.0f;

    /// <summary>
    /// Property to get/set camera offset.
    /// </summary>
    /// <value> Vector3 - offset.</value>
    public Vector3 Offset
    {
        get
        {
            return m_offset;
        }

        set
        {
            m_offset = value;
        }
    }

    /// <summary>
    /// Property to get/set target object.
    /// </summary>
    /// <value> GameObject - target object.</value>
    public GameObject TargetObject
    {
        get
        {
            return m_targetObject;
        }

        set
        {
            m_targetObject = value;
        }
    }
    
    /// <summary>
    /// All derived classes must provide their
    /// own update functionality.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Set up a camera's parameters.
    /// </summary>
    public abstract void SetCamera(GameObject targetObject, Vector3 offset);
}
