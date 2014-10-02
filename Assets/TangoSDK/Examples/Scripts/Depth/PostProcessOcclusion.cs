//-----------------------------------------------------------------------
// <copyright file="PostProcessOcclusion.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using Tango;

/// <summary>
/// Send texture buffers into the occlusion shader.
/// </summary>
public class PostProcessOcclusion : MonoBehaviour 
{
    public Material m_depthOcclusionMaterial;
    public DepthTexture m_depthTexture;
    
    public bool m_isUsingFilter = true;

    private const int TEX_WIDTH = 1280; // HACK
    private const int TEX_HEIGHT = 720; // HACK

    private const TextureFormat TEX_FORMAT = TextureFormat.RGB565;

    private Texture2D m_texture;
    private double m_timestamp = 0.0;
    
    /// <summary>
    /// Use this for initialization.
    /// Init all the data for depth texture construct.
    /// </summary>
    private void Start() 
    {
        camera.depthTextureMode = DepthTextureMode.Depth;
        m_texture = new Texture2D(TEX_WIDTH, TEX_HEIGHT, TEX_FORMAT, false);
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// Pass the color texture and depth texture into the post process shader.
    /// </summary>
    private void Update() 
    {
        VideoOverlayProvider.RenderLatestFrame(m_texture.GetNativeTextureID(), 
                                               TEX_WIDTH, 
                                               TEX_HEIGHT, 
                                               ref m_timestamp);
        
        m_depthOcclusionMaterial.SetTexture("_ColorTex", m_texture);
        m_depthOcclusionMaterial.SetTexture("_DepthTex", 
                                            m_depthTexture.GetDepthTexture(m_isUsingFilter, Camera.main.farClipPlane, 1));
        GL.InvalidateState();
    }
    
    /// <summary>
    /// Post process from source texture to destination.
    /// </summary>
    /// <param name="source"> Source render texture. </param>
    /// <param name="destination"> Destination render texture. </param>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_depthOcclusionMaterial);
    }
}