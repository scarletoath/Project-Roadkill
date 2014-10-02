//-----------------------------------------------------------------------
// <copyright file="UFOController.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// This class will control UFO movements.
/// </summary>
public class UFOController : MonoBehaviour
{
    public float m_rotateSpeed;
    private float m_startY;
    private float m_time;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start() 
    {
        m_startY = transform.position.y;
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update() 
    {
        m_time += Time.deltaTime;
        transform.Rotate(transform.up, m_rotateSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 
                                         m_startY + Mathf.PingPong(m_time * 0.2f, 0.3f), 
                                         transform.position.z);
    }
}
