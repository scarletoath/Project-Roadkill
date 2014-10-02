//-----------------------------------------------------------------------
// <copyright file="ARUFOGameLogic.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// This class contains all game logic of AR UFO scene.
/// </summary>
public class ARUFOGameLogic : MonoBehaviour 
{
    public Transform m_arCameraTransform;
    public GameObject m_UFOObject;
    public PostProcessOcclusion m_postProcessOcclusion;
    public Material m_depthOcclusionMaterial;
    
    private bool m_isShowingDepthImage = false;
    private bool m_isUsingFilter = true;
    private bool m_isUsingOcclusion = true;

    /// <summary>
    /// Upon initialization make sure we only use first
    /// person camera.
    /// </summary>
    private void Start()
    {
        // Tell the camera to hide the buttons to swap the camera behaviors!
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        
        if (cameraController != null)
        {
            cameraController.ShowBehaviorButtons = false;
        }

        if (m_isShowingDepthImage)
        {
            m_depthOcclusionMaterial.SetInt("_IsShowingDepth", 1);
        }
        else
        {
            m_depthOcclusionMaterial.SetInt("_IsShowingDepth", 0);
        }

        m_postProcessOcclusion.m_isUsingFilter = m_isUsingFilter;

        if (m_isUsingOcclusion)
        {
            m_depthOcclusionMaterial.SetInt("_IsEnabledOcclusion", 1);
        }
        else
        {
            m_depthOcclusionMaterial.SetInt("_IsEnabledOcclusion", 0);
        }
    }
    
    /// <summary>
    /// Updates GUI.
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 250, 70), m_isShowingDepthImage ? "Depth View On" : "Depth View Off")) 
        {
            m_isShowingDepthImage = !m_isShowingDepthImage;
            if (m_isShowingDepthImage)
            {
                m_depthOcclusionMaterial.SetInt("_IsShowingDepth", 1);
            }
            else
            {
                m_depthOcclusionMaterial.SetInt("_IsShowingDepth", 0);
            }
        }
        if (GUI.Button(new Rect(10, 90, 250, 70), m_isUsingFilter ? "Filter On" : "Filter Off")) 
        {
            m_isUsingFilter = !m_isUsingFilter;
            m_postProcessOcclusion.m_isUsingFilter = m_isUsingFilter;
        }
        if (GUI.Button(new Rect(10, 170, 250, 70), m_isUsingOcclusion ? "Occlusion On" : "Occlusion Off")) 
        {
            m_isUsingOcclusion = !m_isUsingOcclusion;
            if (m_isUsingOcclusion)
            {
                m_depthOcclusionMaterial.SetInt("_IsEnabledOcclusion", 1);
            }
            else
            {
                m_depthOcclusionMaterial.SetInt("_IsEnabledOcclusion", 0);
            }
        }
        
        if (GUI.Button(new Rect(950, 10, 250, 150), "Add UFO")) 
        {
            _AddUFO();
        }
        if (GUI.Button(new Rect(950, 170, 250, 70), "Destory All UFOs")) 
        {
            _DestoryAllUFO();
        }
    }
    
    /// <summary>
    /// Add UFO.
    /// </summary>
    private void _AddUFO()
    {
        Vector3 objectPosition = new Vector3(m_arCameraTransform.position.x, 
                                             m_arCameraTransform.position.y, 
                                             m_arCameraTransform.position.z);
        
        GameObject newUFOObject = (GameObject)Instantiate(m_UFOObject, 
                                                          objectPosition, 
                                                          Quaternion.identity); 
        newUFOObject.transform.parent = m_arCameraTransform.gameObject.transform;
        newUFOObject.transform.localPosition = new Vector3(0f, -0.1f, 0.5f);
        newUFOObject.transform.parent = gameObject.transform;
        newUFOObject.SetActive(true);
    }
    
    /// <summary>
    /// Destory all UFOs.
    /// </summary>
    private void _DestoryAllUFO()
    {
        if (transform.childCount == 0) 
        {
            return;
        }
        foreach (Transform child in transform) 
        {
            Destroy(child.gameObject);
        }
    }
}
