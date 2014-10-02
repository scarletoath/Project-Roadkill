//-----------------------------------------------------------------------
// <copyright file="ARScreen.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Encapsulates functionality to draw the video overlay.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ARScreen : MonoBehaviour
{
	public float m_frustumLength = 3.0f; // HACK : Chase

	private float m_frustumHeight = 0.0f;
	private float m_frustumWidth = 0.0f;
	private Vector3[] m_frustumPoints;
	private Camera m_tangoCamera;
    private Mesh m_mesh;
    private MeshFilter m_meshFilter;

    /// <summary>
    /// Initialize the AR Screen.
    /// </summary>
	private void Start()
	{
		m_frustumPoints = new Vector3[5];

        CameraController cameraController = GameObject.FindObjectOfType(typeof(CameraController)) as CameraController;
        if (cameraController != null)
        {
            m_tangoCamera = cameraController.gameObject.GetComponent<Camera>();
        }
        else
        {
            DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR, 
                                               "ARScreen.Start : CameraController not found!");
        }
	
        m_meshFilter = GetComponent<MeshFilter>();
        m_mesh = new Mesh();
        ResizeScreen();

        transform.rotation = Quaternion.identity;
	}

    /// <summary>
    /// Creates a plane and sizes it for the AR Screen.
    /// </summary>
	public void ResizeScreen()
	{
		m_frustumHeight = 2.0f * m_frustumLength * Mathf.Tan(m_tangoCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		m_frustumWidth = m_frustumHeight * m_tangoCamera.aspect;

		m_frustumPoints[0] = Vector3.zero;
		m_frustumPoints[1] = new Vector3(-m_frustumWidth, -m_frustumHeight, m_frustumLength);
		m_frustumPoints[2] = new Vector3(-m_frustumWidth, m_frustumHeight, m_frustumLength);
		m_frustumPoints[3] = new Vector3(m_frustumWidth, -m_frustumHeight, m_frustumLength);
		m_frustumPoints[4] = new Vector3(m_frustumWidth, m_frustumHeight, m_frustumLength);
	
		transform.position = new Vector3(0.0f, 0.0f, m_frustumLength);

		// verts
		Vector3[] verts = new Vector3[6];
		verts[0] = m_frustumPoints[1];
		verts[1] = m_frustumPoints[2];
		verts[2] = m_frustumPoints[3];
		verts[3] = m_frustumPoints[3];
		verts[4] = m_frustumPoints[2];
		verts[5] = m_frustumPoints[4];

		// indices
		int[] indices = new int[6];
		indices[0] = 0;
		indices[1] = 1;
		indices[2] = 2;
		indices[3] = 3;
		indices[4] = 4;
		indices[5] = 5;

		// uvs
		Vector2[] uvs = new Vector2[6];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(0, 1);
		uvs[2] = new Vector2(1, 0);
		uvs[3] = new Vector2(1, 0);
		uvs[4] = new Vector2(0, 1);
		uvs[5] = new Vector2(1, 1);

		m_mesh.Clear();
		m_mesh.vertices = verts;
		m_mesh.triangles = indices;
		m_mesh.uv = uvs;
		m_meshFilter.mesh = m_mesh;
	}
}
