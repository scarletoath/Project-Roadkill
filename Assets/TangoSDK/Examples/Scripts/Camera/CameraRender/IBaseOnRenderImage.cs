//-----------------------------------------------------------------------
// <copyright file="IBaseOnRenderImage.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class for anything that should be
/// draw in the OnRenderImage process.
/// </summary>
public abstract class IBaseOnRenderImage : MonoBehaviour 
{
    /// <summary>
    /// Abstract post process function from source texture to destination.
    /// </summary>
    /// <param name="source"> Source render texture. </param>
    /// <param name="destination"> Destination render texture. </param>
    public abstract void OnRenderImage(RenderTexture source, 
                                       RenderTexture destination);
}
