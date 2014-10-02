//-----------------------------------------------------------------------
// <copyright file="VideoOverlayPlaneRender.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using Tango;

/// <summary>
/// Responsible for drawing the AR screen video overlay.
/// </summary>
public class VideoOverlayPlaneRender : IBasePreRenderer
{
    private const int TEX_WIDTH = 1280; // HACK
    private const int TEX_HEIGHT = 720; // HACK
    private const TextureFormat TEX_FORMAT = TextureFormat.RGB565;

    private Texture2D m_texture;
    private double m_timestamp = 0.0;

    /// <summary>
    /// Perform any Camera.OnPreRender() logic
    /// here.
    /// </summary>
    public sealed override void OnPreRender()
    {
        VideoOverlayProvider.RenderLatestFrame(m_texture.GetNativeTextureID(), 
                                               TEX_WIDTH, 
                                               TEX_HEIGHT, 
                                               ref m_timestamp);
        GL.InvalidateState();
    }

    /// <summary>
    /// Initialize this instance.
    /// </summary>
    private void Start()
    {
        m_texture = new Texture2D(TEX_WIDTH, TEX_HEIGHT, TEX_FORMAT, false);
        renderer.material.mainTexture = m_texture;
    }
}
