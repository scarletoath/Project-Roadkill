//-----------------------------------------------------------------------
// <copyright file="CameraRenderer.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Responsible for locating all object of types IBasePreRenderer
/// and IBasePostRenderer, then making sure their drawing code is called.
/// </summary>
public class CameraRenderer : MonoBehaviour
{
    private IBasePreRenderer[] m_allRenderers;
    private IBaseOnRenderImage[] m_allOnRenderImageRenderer;

    /// <summary>
    /// Get all references.
    /// </summary>
    private void Start()
    {
        m_allRenderers = GameObject.FindObjectsOfType(typeof(IBasePreRenderer)) as IBasePreRenderer[];
        m_allOnRenderImageRenderer = 
            GameObject.FindObjectsOfType(typeof(IBaseOnRenderImage)) as IBaseOnRenderImage[];
    }

    /// <summary>
    /// Call OnPreRender on all IBasePreRender objects.
    /// </summary>
    private void OnPreRender()
    {
        foreach (IBasePreRenderer renderer in m_allRenderers)
        {
            renderer.OnPreRender();
        }
    }

    /// <summary>
    /// Call OnRenderImage on all IBaseOnRenderImage objects.
    /// </summary>
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        bool isRendersActive = false;
        foreach (IBaseOnRenderImage renderer in m_allOnRenderImageRenderer)
        {
            if(renderer.gameObject.activeSelf)
            {
                isRendersActive = true;
                renderer.OnRenderImage(source, destination);
            }
        }
        if(!isRendersActive)
        {
            Graphics.Blit(source, destination);
        }
    }
}
