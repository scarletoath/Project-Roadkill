//-----------------------------------------------------------------------
// <copyright file="Helper.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Helper class for Tango.
/// </summary>
public static class Helper 
{
    static public bool usingCLevelCode = false;
    
    /// <summary>
    /// Filter for depth occlusion use.
    /// </summary>
    /// <param name="sourceArr">The source array of depth.</param>
    /// <param name="destArr">The destination array of depth.</param>
    /// <param name="level">Level of rounds to filter.</param>
    public static void Filter(float[] sourceArr, float[] destArr, int level)
    {
        GCHandle sourceHandler = GCHandle.Alloc(sourceArr, GCHandleType.Pinned);
        GCHandle destHandler  = GCHandle.Alloc(destArr, GCHandleType.Pinned);
        DepthNoiseFilter(sourceHandler.AddrOfPinnedObject(), destHandler.AddrOfPinnedObject(), level);
        destHandler.Free();
        sourceHandler.Free();
    } 
    
    /// <summary>
    /// Native filter function import.
    /// </summary>
    /// <param name="srouce">The source array ptr of depth.</param>
    /// <param name="dest">The destination array ptr of depth.</param>
    /// <param name="level">Level of rounds to filter.</param>
    [DllImport("TangoHelpers")]
    public static extern void DepthNoiseFilter(System.IntPtr srouce, System.IntPtr dest, int level);
}
