//-----------------------------------------------------------------------
// <copyright file="Common.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tango
{
    /// <summary>
    /// This struct holds common global functionality used by
    /// this SDK.
    /// </summary>
    public struct Common
    {
        /// <summary>
        /// Codes returned by Tango API functions.
        /// </summary>
        public enum RetCodes
        {
            kCAPISuccess = 0,
            kCAPIFail = -1,
            kCAPINotImplemented = -2,
            kCAPIAlreadyInstasiated = 100,
            kCAPINotInitialized = 101,
            kCAPIDataNotAvailable = 102,
            kCAPIWrongInputData = 103,
            kCAPIOperationFailed = 104
        }
        
        public const string TANGO_UNITY_DLL = "tango_api";
        
    #if (UNITY_EDITOR || UNITY_STANDALONE_OSX)
        private static bool m_mirroring = true; 
    #elif (UNITY_IPHONE || UNITY_ANDROID) 
        private static bool m_mirroring = false; 
    #else 
        private static bool m_mirroring = false;
    #endif
        private static Resolution m_depthFrameResolution;
        private static int m_depthBufferSize;

        /// <summary>
        /// Property for mirroring.
        /// </summary>
        /// <value> Bool - sets mirroring.</value>
        public static bool Mirroring
        {
            get { return m_mirroring; }
            set { m_mirroring = value; }
        }

        /// <summary>
        /// Property for the current depth frame resolution.
        /// </summary>
        /// <value> Resolution - Sets depth frame resolution reference.</value>
        public static Resolution DepthFrameResolution
        {
            get { return m_depthFrameResolution; }
            set { m_depthFrameResolution = value; }
        }

        /// <summary>
        /// Property for the depth buffer size.
        /// </summary>
        /// <value> Bool - Sets the size of the depth buffer.</value>
        public static int DepthBufferSize
        {
            get { return m_depthBufferSize; }
            set { m_depthBufferSize = value; }
        }

        /// <summary>
        /// Get the world rotation.
        /// </summary>
        /// <returns> Quaternion representing the world rotation.</returns>
        public static Quaternion GetWorldRotation()
        {
            return OrientationManager.GetWorldRotation();
        } 
        
        /// <summary>
        /// Gets current window resoltion where width is
        /// always larger than height.
        /// </summary>
        /// <returns> Vector2 containing the screen width and height. </returns>
        public static Vector2 GetWindowResolution()
        {
            Vector2 screenSize;
            if (Screen.width > Screen.height)
            {
                screenSize = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                screenSize = new Vector2(Screen.height, Screen.width);
            }
            return screenSize;
        }
        
        /// <summary>
        /// Get the aspect resolution of the window.
        /// </summary>
        /// <returns> Window resolution aspect ratio as a single
        /// precision floating point.</returns>
        public static float GetWindowResoltionAspect()
        {
            Vector2 resolution = GetWindowResolution();
            return resolution.x / resolution.y;
        }
        
        /// <summary>
        /// Calls Application.Quit.
        /// </summary>
        public static void Quit()
        {   
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}