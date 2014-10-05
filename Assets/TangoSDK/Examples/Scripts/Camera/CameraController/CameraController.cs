//-----------------------------------------------------------------------
// <copyright file="CameraController.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Updates the attached camera based on the current
/// active behavior.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Camera type enum.
    /// </summary>
    public enum CameraType
    {
        FIRST_PERSON = 0x1,
        THIRD_PERSON = 0x2,
        TOP_DOWN = 0x4
    }

    public GameObject m_targetObject;

    private CameraType m_currentCamera;
    private IBaseCamera m_firstPersonCamera;
    private IBaseCamera m_thirdPersonCamera;
    private IBaseCamera m_topDownCamera;

    public bool m_showBehaviorButtons = true;

    /// <summary>
    /// Property to get/set whether camera
    /// behavior buttons should be drawn to 
    /// the screen.
    /// </summary>
    /// <value> Bool - True if buttons should be shown.</value>
    public bool ShowBehaviorButtons
    {
        get
        {
            return m_showBehaviorButtons;
        }
        set
        {
            m_showBehaviorButtons = value;
        }
    }

    /// <summary>
    /// Current camera type.
    /// </summary>
    /// <value> Current camera type to render.</value>
    public CameraType CurrentCamera
    {
        get
        {
            return m_currentCamera;
        }

        set
        {
            m_currentCamera = value;
        }
    }

    /// <summary>
    /// Enabled based on camera type.
    /// </summary>
    /// <param name="cameraType">Enable which camera.</param>
    public void EnableCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.FIRST_PERSON:
            {
                m_firstPersonCamera.enabled = true;
                m_thirdPersonCamera.enabled = false;
                m_topDownCamera.enabled = false;
                break;
            }
            case CameraType.THIRD_PERSON:
            {
                m_firstPersonCamera.enabled = false;
                m_thirdPersonCamera.enabled = true;
                m_topDownCamera.enabled = false;
                break;
            }
            case CameraType.TOP_DOWN:
            {
                m_firstPersonCamera.enabled = false;
                m_thirdPersonCamera.enabled = false;
                m_topDownCamera.enabled = true;
                break;
            }
        }
        m_currentCamera = cameraType;
    }

    /// <summary>
    /// Monobehavior Start call back.
    /// Get three type of camera's reference and set initial values.
    /// </summary>
    private void Awake()
    {
        m_firstPersonCamera = gameObject.GetComponent<FirstPersonCamera>() as IBaseCamera;
        if (m_firstPersonCamera == null)
        {
            m_firstPersonCamera = gameObject.AddComponent<FirstPersonCamera>();
        }
        m_firstPersonCamera.SetCamera(m_targetObject, Vector3.zero);

        m_thirdPersonCamera = gameObject.GetComponent<ThirdPersonCamera>() as IBaseCamera;
        if (m_thirdPersonCamera == null)
        {
            m_thirdPersonCamera = gameObject.AddComponent<ThirdPersonCamera>();
        }
        m_thirdPersonCamera.SetCamera(m_targetObject, new Vector3(0.0f, 0.0f, -5.0f));

        m_topDownCamera = gameObject.GetComponent<TopDownCamera>() as IBaseCamera;
        if (m_topDownCamera == null)
        {
            m_topDownCamera = gameObject.AddComponent<TopDownCamera>();
        }
        m_topDownCamera.SetCamera(m_targetObject, new Vector3(0.0f, 15.0f, 0.0f));

        EnableCamera(CameraType.FIRST_PERSON);
    }

    /// <summary>
    /// Update current behavior. 
    /// DEBUG USE.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EnableCamera(CameraType.FIRST_PERSON);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            EnableCamera(CameraType.THIRD_PERSON);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            EnableCamera(CameraType.TOP_DOWN);
        }
    }

    /// <summary>
    /// Draw buttons to swap current behavior. 
    /// DEBUG USE.
    /// </summary>
    private void OnGUI()
    {
        if (m_showBehaviorButtons)
        {
            if (GUI.Button(new Rect(1000, 70, 150, 70), "First"))
            {
                EnableCamera(CameraType.FIRST_PERSON);
            }
            if (GUI.Button(new Rect(1000, 140, 150, 70), "Third"))
            {
                EnableCamera(CameraType.THIRD_PERSON);
            }
            if (GUI.Button(new Rect(1000, 210, 150, 70), "Top"))
            {
                EnableCamera(CameraType.TOP_DOWN);
            }
        }
    }
}
