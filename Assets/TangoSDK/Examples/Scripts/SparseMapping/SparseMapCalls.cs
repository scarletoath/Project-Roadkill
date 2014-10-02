//-----------------------------------------------------------------------
// <copyright file="SparseMapCalls.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using Tango;
using UnityEngine;

/// <summary>
/// Basic sparse map functionality like load/save/reset.
/// </summary>
public class SparseMapCalls : MonoBehaviour
{
    /// <summary>
    /// Creates new instance of prefab aka new Navigator.cs, Synchroniser.cs .
    /// Assigns path of sparse map file to load.
    /// </summary>
    /// <param name="fileName"> Path of file to be loaded.</param>
    public void LoadSparseMap(string fileName)
    {
        TangoApplication.Instance.m_sparseMapPath = fileName;
        CreateFreshMapInstance();
    }

    /// <summary>
    /// Assigns path of sparse map file to save.
    /// </summary>
    /// <param name="fileName"> Path of file to be saved.</param>
    /// <returns> True if save successful else false.</returns>
    public bool SaveSparseMap(string fileName)
    {
        return VIOProvider.SaveSparseMap(fileName);
    }

    /// <summary>
    /// Destroys instance of camera.
    /// </summary>
    public void ResetSparseMap()
    {
        TangoApplication.Instance.ShutDownApplication();
    }

    /// <summary>
    /// Destroys existing instance of camera and creates new instance.
    /// </summary>
    public void CreateFreshMapInstance()
    {
        ResetSparseMap();
        TangoApplication.Instance.InitApplication();

        // add all components you use like VIO, Depth, Video
    }
}
