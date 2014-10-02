//-----------------------------------------------------------------------
// <copyright file="SparseMapButtons.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using Tango;

/// <summary>
/// Display buttons and draws trails for the basic sparse map function calls.
/// </summary>
public class SparseMapButtons : MonoBehaviour
{
    // Trail loader object in your scene
    public TrailManager m_trailManagerRef;

    // string to print status / debug messages
    private string m_debugString = string.Empty;

    private SparseMapCalls m_sparseMapCallRef;

    // bool to show recording status.
    private bool m_recording;

    private bool m_displayMapList = false;

    // To create scroll List of files
    private Vector2 m_scrollView;

    // array of file names found in app directory
    private string[] m_files;

    // stores the path of the file chosen
    private string m_path;

    /// <summary>
    /// Loads sparse map and trail.txt file.
    /// </summary>
    /// <param name="path">Path of file to be loaded.</param>
    protected void LoadSelectedFile(string path)
    {
        if (path != string.Empty)
        {
            // call to load sparse map
            m_sparseMapCallRef.LoadSparseMap(path);

            // call to load trail
            if (m_trailManagerRef.LoadTrailFromFile(path + ".txt"))
            {
                m_trailManagerRef.CreateTrailFromList();
                m_debugString = "Trail loaded successfully ";
            }
            else
            {
                m_debugString = "Trail file not found";
            }
        }
        else
        {
            m_debugString = "sparse map file is null";
        }
    }

    /// <summary>
    /// Initializating following: 
    /// - the basic sparse map calls script.
    /// - files found while loading.
    /// </summary>
    private void Start()
    {
        m_sparseMapCallRef = GameObject.Find("SparseMapCallsObject").GetComponent<SparseMapCalls>();
        m_files = new string[]
        {
            string.Empty
        };
    }

    /// <summary>
    /// Displays following buttons: 
    /// - Record Map / Save Map.
    /// - Load Map / Cancel Load.
    /// - Reset.
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 10, 150, 70), m_recording ? "Save map" : "Record map"))
        {
            _RecordSaveSparseMap();
        }

        // GUI.color = Color.white;
        if (GUI.Button(new Rect(0, 90, 150, 70), m_displayMapList ? "Cancel Load" : "Load map"))
        {
            _LoadSparseMapList();
        }

        if (GUI.Button(new Rect(0, 170, 150, 70), "Reset"))
        {
            m_recording = false;
            m_trailManagerRef._ReInitializeTrail();
            m_sparseMapCallRef.ResetSparseMap();
        }

        if (m_displayMapList)
        {
            GUILayout.BeginArea(new Rect(Screen.width * 0.25f, Screen.height * 0.2f, Screen.width * 0.5f, Screen.height * 0.7f));
            m_scrollView = GUILayout.BeginScrollView(m_scrollView);
            for (int i = 0; i < m_files.Length; i++)
            {
                if (GUILayout.Button(m_files[i], GUILayout.Width(450), GUILayout.Height(100)))
                {
                    m_debugString = "clicked : " + m_files[i];
                    m_path = Application.persistentDataPath + "/" + m_files[i];
                    m_displayMapList = false;
                    LoadSelectedFile(m_path);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Label("number of files : " + m_files.Length.ToString());
            GUILayout.EndArea();
        }
        GUI.Label(new Rect(0, 250, 1300, 100), "<size=25> Status : " + m_debugString + "</size>");
    }

    /// <summary>
    /// Records sparse map and saves (filename).txt file with trail.
    /// </summary>
    private void _RecordSaveSparseMap()
    {
        if (!m_recording)
        {
            // start fresh map
            m_sparseMapCallRef.CreateFreshMapInstance();
            m_recording = true;

            // Updates status message on screen
            m_debugString = "New Sparse Map Recording...";

            // start recording sparsemap in txt file
            m_trailManagerRef.StartTrailBuilding();
        }
        else
        {
            string fileName = Application.persistentDataPath + "/sparse_map" + _GetCurrentTimeStamp();
            m_debugString = "Done Save sparse_map" + _GetCurrentTimeStamp();
            m_trailManagerRef.StopTrailBuilding(fileName + ".txt");

            // save sparsemap
            if (m_sparseMapCallRef.SaveSparseMap(fileName))
            {
                m_recording = false;
            }
            else
            {
                m_debugString = "Error saving map, check Navigator.cs";
            }
        }
    }

    /// <summary>
    /// Loads sparse map and Loads a (filename).txt file.
    /// </summary>
    private void _LoadSparseMapList()
    {
        if (!m_displayMapList)
        {
            m_debugString = "choose file, trying to load";
            m_displayMapList = true;
            m_files = FileAccessUtilities.RetrieveFilesList(Application.persistentDataPath, ".txt");
        }
        else
        {
            m_displayMapList = false;
        }
    }

    /// <summary>
    /// Returns timestamp in a particular format.
    /// </summary>
    /// <returns> Returns system time stamp.</returns>
    private string _GetCurrentTimeStamp()
    {
        return string.Format("{0:yyyyMMdd_HHmmss}", System.DateTime.Now);
    }
}
