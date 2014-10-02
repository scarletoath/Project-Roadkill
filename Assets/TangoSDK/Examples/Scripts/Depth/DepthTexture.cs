//-----------------------------------------------------------------------
// <copyright file="DepthTexture.cs" company="Google">
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
/// DepthTexture get the depth value and convert it to
/// a texture.
/// </summary>
public class DepthTexture : MonoBehaviour
{
    private const int DEPTH_TEX_WIDTH = 320;
    private const int DEPTH_TEX_HEIGHT = 180;

    private Texture2D m_depthTexture;
    private Color[] m_depthColorArr;
    
    // raw depth data
    private int[] m_depthMillimeterArr;
    
    // real z value in meters
    private float[] m_depthMeterArr;
    
    // z value after run through the filter
    private float[] m_filteredDepthMeterArr;
    
    private double m_timeStamp = 0.0;
    private double m_prevTimeStamp = 0.0;

    /// <summary>
    /// Get most recent updated depthTexture.
    /// </summary>
    /// <param name="usingFilter"> If use the depth noise filter.</param>
    /// <param name="maxLength"> Farest(max) value from depth.</param>
    /// <param name="filterLevel"> Levels of surrounding points check in filter.</param>
    /// <returns>Depth texture.</returns>
    public Texture2D GetDepthTexture(bool usingFilter, float maxLength, int filterLevel) 
    {   
        DepthProvider.GetLatestFrame(ref m_depthMillimeterArr, ref m_timeStamp);
        if (m_timeStamp == m_prevTimeStamp)
        {
            return m_depthTexture;
        }
        m_prevTimeStamp = m_timeStamp;
        _GetZFromRaw(ref m_depthMeterArr, m_depthMillimeterArr);
        
        float[] zarray;
        if (usingFilter)
        {
            Helper.Filter(m_depthMeterArr, m_filteredDepthMeterArr, filterLevel);
            zarray = m_filteredDepthMeterArr;
        }
        else
        {
            zarray = m_depthMeterArr;
        }

        float maxLengthInverse = 1.0f / maxLength;
        for (int i = 0; i < DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT; i++) 
        {
            m_depthColorArr[i].r = zarray[i] * maxLengthInverse;
            m_depthColorArr[i].g = zarray[i] * maxLengthInverse;
            m_depthColorArr[i].b = zarray[i] * maxLengthInverse;
        }
        m_depthTexture.SetPixels(m_depthColorArr);
        m_depthTexture.Apply();
        return m_depthTexture;
    }
    
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start() 
    {
        m_depthTexture = new Texture2D(DEPTH_TEX_WIDTH, DEPTH_TEX_HEIGHT);
        m_depthColorArr = new Color[DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT];
        m_depthMeterArr = new float[DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT];
        m_filteredDepthMeterArr = new float[DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT];
        m_depthMillimeterArr = new int[DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT];
        for (int i = 0; i < DEPTH_TEX_WIDTH * DEPTH_TEX_HEIGHT; i++) 
        {
            m_depthColorArr[i] = Color.blue;
        }
    }
    
    /// <summary>
    /// Get z buffer from raw data, put pixels upside down.
    /// </summary>
    /// <param name="z"> Z buffer in meters. </param>
    /// <param name="intArr"> Raw int array data. </param>
    private void _GetZFromRaw(ref float[] z, int[] intArr)
    {
        for (int i = 0; i < DEPTH_TEX_HEIGHT; i++)
        {
            for (int j = 0; j < DEPTH_TEX_WIDTH; j++)
            {
                int index = ((DEPTH_TEX_HEIGHT - i - 1) * DEPTH_TEX_WIDTH) + j;
                int z_raw = intArr[index];
                z[(i * DEPTH_TEX_WIDTH) + j] = (float)z_raw * 0.001f;
            }
        }
    }
}