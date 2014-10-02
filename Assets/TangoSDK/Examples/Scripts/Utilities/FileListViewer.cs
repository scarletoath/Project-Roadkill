//-----------------------------------------------------------------------
// <copyright file="FileListViewer.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// File browser for app directory only.
/// </summary>
public class FileListViewer
{
    // stores the path of the file chosen
    public string m_path;

    // bool to check if GUI should be enabled or disabled
    public bool m_isGUIDisabled;

    // array of file names found in app directory
    protected string[] m_files;

    // call back delegate function
    protected CallBack m_callBack;
    private Vector2 m_scrollView;

    /// <summary>
    /// Constructor for this class.
    /// </summary>
    /// <param name="callBackFromClass">Call back function to be called.</param>
    public FileListViewer(CallBack callBackFromClass)
    {
        m_isGUIDisabled = true;
        m_callBack = callBackFromClass;
        m_scrollView.x = 10;
        m_scrollView.y = 10;
        m_files = new string[]
        {
            string.Empty
        };
        RetrieveFilesList();
    }

    /// <summary>
    /// Called when the user clicks on a file button.
    /// </summary>
    /// <param name="path">Path of file to be loaded.</param>
    public delegate void CallBack(string path);

    /// <summary>
    /// Use this for GUI related calls.
    /// </summary>
    public void OnGUI()
    {
        if (m_isGUIDisabled == false)
        {
            GUILayout.BeginArea(new Rect(Screen.width * 0.25f, Screen.height * 0.2f, Screen.width * 0.5f, Screen.height * 0.7f));
            m_scrollView = GUILayout.BeginScrollView(m_scrollView);
            for (int i = 0; i < m_files.Length; i++)
            {
                if (GUILayout.Button(m_files[i], GUILayout.Width(450), GUILayout.Height(100)))
                {
                    Debug.Log("clicked" + i);
                    m_path = m_files[i];
                    m_isGUIDisabled = true;
                    m_callBack(m_path);
                }
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Cancel Load", GUILayout.Width(200), GUILayout.Height(120)))
            {
                m_isGUIDisabled = true;
            }
            GUILayout.Label("number of files : " + m_files.Length.ToString());
            GUILayout.EndArea();
        }
    }
        
    /// <summary>
    /// Used to re calculate files and update the list of files shown.
    /// </summary>
    public void ReInitialize()
    {
        m_files = null;
        m_files = new string[]
        {
            string.Empty
        };
        RetrieveFilesList();
        m_isGUIDisabled = false;
    }

    /// <summary>
    /// Used to retreive a list of files in the directory
    /// Also removes .txt files from the list of files found.
    /// </summary>
    protected void RetrieveFilesList()
    {
        string[] tempFiles = Directory.GetFiles(Application.persistentDataPath);

        // this loop separates file name from full path
        for (int i = tempFiles.Length - 1; i >= 0; i--)
        {
            tempFiles[i] = Path.GetFileName(tempFiles[i]);
        }

        for (int i = tempFiles.Length - 1; i >= 0; i--)
        {
            if (tempFiles[i].Contains(".txt"))
            {
                tempFiles = tempFiles.Where(name => name != tempFiles[i]).ToArray();
            }
        }
        m_files = tempFiles;
    }
}
