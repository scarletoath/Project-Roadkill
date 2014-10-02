//-----------------------------------------------------------------------
// <copyright file="DepthViewerController.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;
using Tango;

/// <summary>
/// DepthTextureRender has the functionality to render the depth image
/// as well as turn on/off filter and change.
/// </summary>
public class DepthViewerController : MonoBehaviour
{
    public DepthTexture m_depthTexture;

    private float m_blendValue = 0.5f;
    private bool m_isUsingFilter = false;
    private float m_depthTexMaxLength = 8.0f;
    private int m_filterLevel = 1;

    private Texture2D m_texture;
    private double m_timestamp = 0.0;

    /// <summary>
    /// Start unity call back,
    /// init the new texture.
    /// </summary>
    private void Start()
    {
        m_texture = new Texture2D(1280, 720, TextureFormat.RGB565, false);
        m_texture.filterMode = FilterMode.Bilinear;
        m_texture.wrapMode = TextureWrapMode.Clamp;
    }

    /// <summary>
    /// Update is called once per frame,
    /// pass both color texture and depth texture into shader.
    /// </summary>
    private void Update() 
    {
        renderer.material.SetTexture("_ColorTex", m_texture);
        renderer.material.SetTexture("_DepthTex", 
                                     m_depthTexture.GetDepthTexture(m_isUsingFilter, m_depthTexMaxLength, m_filterLevel));
        renderer.material.SetFloat("_BlendValue", m_blendValue);
        VideoOverlayProvider.RenderLatestFrame(
            m_texture.GetNativeTextureID(), m_texture.width, m_texture.height, ref m_timestamp);
        GL.InvalidateState();
    }
    
    /// <summary>
    /// Updates GUI.
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(1000, 70, 150, 70), "+"))
        {
            m_blendValue += 0.1f;
        }
        if (GUI.Button(new Rect(1000, 140, 150, 70), "-"))
        {
            m_blendValue -= 0.1f;
        }
        if (GUI.Button(new Rect(1000, 210, 150, 70), 
                       m_isUsingFilter ? "Filter On" : "Filter Off"))
        {
            m_isUsingFilter = !m_isUsingFilter;
        }
        m_blendValue = Mathf.Clamp(m_blendValue, 0.0f, 1.0f);
    }   
}