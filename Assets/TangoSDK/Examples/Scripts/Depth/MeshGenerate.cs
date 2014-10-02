//-----------------------------------------------------------------------
// <copyright file="MeshGenerate.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine;
using Tango;

/// <summary>
/// Mesh construct class.
/// </summary>
public class MeshGenerate : MonoBehaviour 
{
    // tango ARCamera object
    public GameObject m_camObj;

    // render mode, point cloud by default
    public MeshTopology m_renderMode = MeshTopology.Points;
    
    // step size, every n pixel take 1 value
    public int m_stepSize = 1;
    
    // since the depth image has some non-valid data on the left and bottom
    // edge, so we set a edge size to skip the non-valid data
    public int m_edgeSize = 0;
    
    // debug texture
    public Texture2D debugTexture;
    
    // some const value
    private const int DEPTH_BUFFER_WIDTH = 320;
    private const int DEPTH_BUFFER_HEIGHT = 180;
    private const float MILLIMETER_TO_METER = 0.001f;

    // camera's property
    // used for put mesh in the center of screen
    // and make it in the right scale
    private int m_screenWidth;
    private int m_screenHeight;
    
    // debug use: texture's pixels arr
    private Color[] m_pixelArr;
    
    // raw depth data
    private int[] m_depthArr_int;
    
    // real z value in meters
    private float[] m_depthMeter;
    
    private int m_texWidth, m_texHeight;
    private int m_traverseWidth, m_traverseHeight;
    private double m_timeStamp;
    private double m_prevTimestamp;
    
    // m_vertices will be assigned to this mesh
    private Mesh m_mesh;
    private MeshCollider m_meshCollider;

    // mesh texture
    private Texture2D m_texture;
    private double m_textureTimestamp = 0.0;

    // mesh data
    private Vector3[] m_vertices;
    private Vector2[] m_uvs;
    private int[] m_triangles;
    
    private int m_frameCounter = 0;
    
    // vertex cut out threshold from depth z distance
    private float m_depthDistThreshold = 0.3f;
    
    // vertex cut out threshold from neighbor vetex distnce
    private float m_neighborDistThreshold = 0.5f;
    
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    public void Start() 
    {
        #if (UNITY_EDITOR)
        m_texWidth = debugTexture.width;
        m_texHeight = debugTexture.height;
        m_traverseWidth = m_texWidth / m_stepSize;
        m_traverseHeight = m_texHeight / m_stepSize;
        
        #elif (UNITY_ANDROID)
        m_texWidth = DEPTH_BUFFER_WIDTH;
        m_texHeight = DEPTH_BUFFER_HEIGHT;
        m_traverseWidth = m_texWidth / m_stepSize;
        m_traverseHeight = m_texHeight / m_stepSize;
        m_depthMeter =  new float[DEPTH_BUFFER_WIDTH * DEPTH_BUFFER_HEIGHT];
        m_depthArr_int = new int[DEPTH_BUFFER_WIDTH * DEPTH_BUFFER_HEIGHT];
        m_timeStamp = m_prevTimestamp = 0.0;
        #else
        #error platform is not supported
        #endif

        // re-calculate edge size based on step size
        m_edgeSize = m_edgeSize / m_stepSize;
        
        // note: this is the real screen size, not camera size
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;
        
        // get the reference of mesh
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null) 
        {
            MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
            meshFilter.mesh = m_mesh = new Mesh();
            MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            renderer.material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
        } 
        else 
        {
            m_mesh = mf.mesh;
        }
        _CreateMesh();

        m_texture = new Texture2D(1280, 720, TextureFormat.RGB565, false);
        m_texture.filterMode = FilterMode.Bilinear;
        m_texture.wrapMode = TextureWrapMode.Clamp;
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update() 
    {
        _UpdateMesh();
    }
    
    /// <summary>
    /// Update mesh texture.
    /// </summary>
    private void _UpdateTexture()
    {
        VideoOverlayProvider.RenderLatestFrame(
            m_texture.GetNativeTextureID(), m_texture.width, m_texture.height, ref m_textureTimestamp);
        GL.InvalidateState();
        this.renderer.material.mainTexture = m_texture;
    }
    
    /// <summary>
    /// Create the mesh using two passes.
    /// first pass get all the m_vertices and texture coordinates
    /// second pass to get all the m_triangles.
    /// </summary>
    private void _CreateMesh()
    {
        // alloc m_vertices array, m_uvs array and triangle array.
        int arrSize = m_traverseWidth * m_traverseHeight;
        m_vertices = new Vector3[arrSize];
        m_uvs = new Vector2[arrSize];
        int counter = 0;
        
        // traverse the screen to fill in the vertice array
        // and m_uvs array
        counter = 0;
        for (int i = 0; i < m_traverseHeight; i++) 
        {
            for (int j = 0; j < m_traverseWidth; j++)
            {
                // m_uvs
                m_uvs[counter].x = 
                    (float)((float)j / (float)m_traverseWidth);
                m_uvs[counter].y = 
                    (float)(((float)m_traverseHeight - (float)i) / (float)m_traverseHeight);
                counter++;
            }
        }
        
        // second pass to assign the m_triangles  
        int w = m_traverseWidth - 1;
        int h = m_traverseHeight - 1;
        int triCounter = 0;
        m_triangles = new int[6 * w * h];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                // w + 1 is the real width of m_vertices
                // triangle 1
                m_triangles[triCounter] = (i * (w + 1)) + j;
                m_triangles[triCounter + 1] = ((i + 1) * (w + 1)) + j;
                m_triangles[triCounter + 2] = (i * (w + 1)) + (j + 1);
                
                // triangle 2
                m_triangles[triCounter + 3] = ((i + 1) * (w + 1)) + j;
                m_triangles[triCounter + 4] = ((i + 1) * (w + 1)) + (j + 1);
                m_triangles[triCounter + 5] = (i * (w + 1)) + (j + 1);
                triCounter += 6;
            }
        }
        
        m_mesh.Clear();
        m_mesh.vertices = m_vertices;
        m_mesh.uv = m_uvs;
        m_mesh.triangles = m_triangles;
        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
    }
    
    /// <summary>
    /// Update the mesh m_vertices and m_triangles
    /// separated computation into different frame.
    /// </summary>
    private void _UpdateMesh()
    {
        if (m_frameCounter == 0)
        {
            DepthProvider.GetLatestFrame(ref m_depthArr_int, ref m_timeStamp);
            if (m_prevTimestamp == m_timeStamp) 
            {
                return;
            }
            m_prevTimestamp = m_timeStamp;
            
            // convert the compressed int array to z meters array
            _GetZFromRaw(ref m_depthMeter, m_depthArr_int);
            m_frameCounter++;
            return;
        }
        
        if (m_frameCounter == 1)
        {
            int vertexCounter = 0;
            float xd = m_screenWidth / m_traverseWidth;
            float yd = m_screenHeight / m_traverseHeight;
            for (int i = 0; i < m_traverseHeight; i++) 
            {
                for (int j = 0; j < m_traverseWidth; j++)
                {
                    m_vertices[vertexCounter] = 
                        m_camObj.camera.ScreenToWorldPoint(new Vector3(j * (float)xd, 
                                                                     i * (float)yd,
                                                                     m_depthMeter[(i * m_stepSize * m_texWidth) + (j * m_stepSize)]));
                    vertexCounter++;
                }
            }
            m_frameCounter++;
            return;
        }
        
        if (m_frameCounter == 2)
        {
            int w = m_traverseWidth - 1;
            int h = m_traverseHeight - 1;
            
            int triCounter = 0;
            int len = 6 * w * h;
            for (int i = 0; i < len; i++) 
            {
                m_triangles[i] = 0;
            }
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    // w + 1 is the real width of m_vertices
                    // vertex index map
                    // 1 3
                    // 0 2
                    int index0 = (i * m_stepSize * m_texWidth) + (j * m_stepSize);
                    int index1 = ((i + 1) * m_stepSize * m_texWidth) + (j * m_stepSize);
                    int index2 = (i * m_stepSize * m_texWidth) + ((j + 1) * m_stepSize);
                    int index3 = ((i + 1) * m_stepSize * m_texWidth) + ((j + 1) * m_stepSize);
                    
                    // triangle 1
                    float distance = Mathf.Abs(m_depthMeter[index0] - m_depthMeter[index1]);
                    distance += Mathf.Abs(m_depthMeter[index0] - m_depthMeter[index2]);
                    distance += Mathf.Abs(m_depthMeter[index1] - m_depthMeter[index2]);
                    if (m_depthMeter[index0] > m_depthDistThreshold &&
                        m_depthMeter[index1] > m_depthDistThreshold &&
                        m_depthMeter[index2] > m_depthDistThreshold && 
                        distance < m_neighborDistThreshold)
                    {
                        m_triangles[triCounter] = (i * (w + 1)) + j;
                        m_triangles[triCounter + 1] = ((i + 1) * (w + 1)) + j;
                        m_triangles[triCounter + 2] = (i * (w + 1)) + (j + 1);
                    }
                    
                    // triangle 2
                    distance = 0.0f;
                    distance += Mathf.Abs(m_depthMeter[index1] - m_depthMeter[index2]);
                    distance += Mathf.Abs(m_depthMeter[index2] - m_depthMeter[index3]);
                    distance += Mathf.Abs(m_depthMeter[index1] - m_depthMeter[index3]);
                    if (m_depthMeter[index1] > m_depthDistThreshold &&
                        m_depthMeter[index3] > m_depthDistThreshold &&
                        m_depthMeter[index2] > m_depthDistThreshold && 
                        distance < m_neighborDistThreshold)
                    {
                        m_triangles[triCounter + 3] = ((i + 1) * (w + 1)) + j;
                        m_triangles[triCounter + 4] = ((i + 1) * (w + 1)) + (j + 1);
                        m_triangles[triCounter + 5] = (i * (w + 1)) + (j + 1);
                    }
                    triCounter += 6;
                }
            }
            m_frameCounter++;
            return;
        }
        if (m_frameCounter == 3)
        {
            // update the m_vertices
            m_mesh.vertices = m_vertices;
            m_mesh.triangles = m_triangles;
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
            m_mesh.SetIndices(m_triangles, m_renderMode, 0);
            _UpdateTexture();
            m_frameCounter = 0;
        }
    }
    
    /// <summary>
    /// Get float depth data from raw int data
    /// also flip around x axis.
    /// </summary>
    /// <param name="z">Float array of depth.</param>
    /// <param name="intArr">Raw int data.</param>
    private void _GetZFromRaw(ref float[] z, int[] intArr)
    {
        for (int i = 0; i < DEPTH_BUFFER_HEIGHT; ++i)
        {
            int invRowIndex = (DEPTH_BUFFER_HEIGHT - i - 1) * DEPTH_BUFFER_WIDTH;
            int rowIndex = i * DEPTH_BUFFER_WIDTH;
            for (int j = 0; j < DEPTH_BUFFER_WIDTH; ++j)
            {
                int index = invRowIndex + j;
                int z_raw = intArr[index];
                
                // depth data comes in with unit of millimeter, 
                // we convert it into the meter unit we are using in unity.
                z[rowIndex + j] = (float)z_raw * MILLIMETER_TO_METER;
            }
        }
    }
}
