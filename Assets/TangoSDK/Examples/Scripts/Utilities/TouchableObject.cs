//-----------------------------------------------------------------------
// <copyright file="TouchableObject.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// Base class used for creating custom buttons.
/// Can be inherited to create custom buttons.
/// </summary>
public class TouchableObject : MonoBehaviour 
{
     // camera which detects button touch through raycast
    public Camera m_raycastCamera;

     // Flag for Editor mode testing
    private bool m_isOutTouch = true;
	
    /// <summary>
    /// Called every frame.
    /// </summary>
	protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
		{
            if (_RayCastToSelfObject())
			{
				TouchDown();
			}
		}
        if (Input.GetKey(KeyCode.Mouse0))
		{
            if (_RayCastToSelfObject())
			{
                m_isOutTouch = false;
				OnTouch();
			}
			else
			{
                if (!m_isOutTouch)
				{
                    m_isOutTouch = true;
					OutTouch();
				}
			}
		}
        if (Input.GetKeyUp(KeyCode.Mouse0))
		{
            if (_RayCastToSelfObject())
			{
				TouchUp();
			}
		}
	}

    /// <summary>
    /// Overridable initialize function.
    /// </summary>
	protected virtual void Init()
    {
    }

    /// <summary>
    /// Touch event similar to key down.
    /// </summary>
	protected virtual void TouchDown()
    {
    }

    /// <summary>
    /// Touch event similar to key hold.
    /// </summary>
    protected virtual void OnTouch()
    {
    }	

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void OutTouch()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void TouchUp()
    {
    }

    /// <summary>
    /// Initialize this component.
    /// </summary>
    private void Start()
    {
        Init();

        // if no camera is assigned in inspector, it checks the gameobject for a camera
        if (m_raycastCamera == null)
        {
            m_raycastCamera = Camera.main;
        }
    }

    /// <summary>
    /// Raycast from camera to world and where it hits.
    /// </summary>
    /// <returns>  Returns true or false if hit occurs.</returns>
    private bool _RayCastToSelfObject()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 screenPosition = Input.mousePosition;
        if (Physics.Raycast(m_raycastCamera.ScreenPointToRay(screenPosition), out hit, Mathf.Infinity)) 
        {
            Debug.DrawRay(m_raycastCamera.transform.position, hit.point - m_raycastCamera.transform.position, Color.red);
            if (hit.transform.gameObject == gameObject) 
            {
                return true;
            }
        }
        return false;
    }
}