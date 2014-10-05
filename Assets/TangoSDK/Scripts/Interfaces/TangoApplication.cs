//-----------------------------------------------------------------------
// <copyright file="TangoApplication.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tango
{
    /// <summary>
    /// Entry point of Tango applications, maintain the application handler.
    /// </summary>
    public class TangoApplication : MonoBehaviour 
    {
        public bool m_enableVio = true;
        public bool m_enableDepth = true;
        public bool m_vioAutoReset = true;
        public bool m_enableVideoOverlay = true;
        public VIOProvider.Mode m_operationMode = VIOProvider.Mode.VisualIntertialNavigationAndMapping;
        public string m_sparseMapPath = string.Empty;

        private static TangoApplication m_instance;
        private bool m_valid = false;
        private System.IntPtr m_applicationHandle = System.IntPtr.Zero;
        private DepthProvider m_depthProvider;
        private VIOProvider m_vioProvider;
        private VideoOverlayProvider m_videoOverlayProvider;

        /// <summary>
        /// Singleton instance of TangoApplication.
        /// </summary>
        /// <value> No setter.</value>
        public static TangoApplication Instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject owner = new GameObject();
                    m_instance = owner.AddComponent(typeof(TangoApplication)) as TangoApplication;
                }
                return m_instance;
            }
        }

        /// <summary>
        /// Property to get the Application handle.
        /// </summary>
        /// <value> No setter.</value>
        public System.IntPtr Handle
        {
            get
            {
                return m_applicationHandle;
            }
        }

        /// <summary>
        /// Property to check validity.
        /// </summary>
        /// <value> No setter.</value>
        public bool Valid
        {
            get
            {
                return m_valid;
            }
        }

        /// <summary>
        /// Checks to see if this application is initialized.
        /// </summary>
        /// <returns> True if the application is initialized.</returns>
        public bool IsInitialized()
        {
            return m_applicationHandle != System.IntPtr.Zero;
        }

        /// <summary>
        /// Initialize application and providers.
        /// </summary>
        public void InitApplication()
        {
            if (m_applicationHandle != System.IntPtr.Zero)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR,
                                                   "Duplicated application handler");
                return;
            }

            // let's hardcode depth image reoslution
            // but eventially we can get resolution using VideoMode interface 
            Resolution imageResolution = new Resolution();
            imageResolution.width = 320;
            imageResolution.height = 180;
            imageResolution.refreshRate = 5;
            Common.DepthFrameResolution = imageResolution;
            Common.DepthBufferSize = 320 * 180;

            m_applicationHandle = TangoApplicationAPI.ApplicationInitialize("[Superframes Small-Peanut]");
            if (m_applicationHandle == System.IntPtr.Zero)
            {
                ErrorHandler.instance.presentErrorMessage("Application initialization failed");
            }
            if (m_enableVio)
            {
                VIOProvider.SetAutoReset(m_vioAutoReset);
                VIOProvider.Init(m_operationMode, m_sparseMapPath);
            }
            if (m_enableDepth)
            {
                DepthProvider.Init();
            }
            if (m_enableVideoOverlay)
            {
                VideoOverlayProvider.Init();
            }
        }

        /// <summary>
        /// Shut down the providers and application.
        /// </summary>
        public void ShutDownApplication()
        {
            if (m_enableVio)
            {
                VIOProvider.ShutDown();
            }
            if (m_enableDepth)
            {
                DepthProvider.ShutDown();
            }
            if (m_enableVideoOverlay)
            {
                VideoOverlayProvider.Shutdown();
            }
            if (m_applicationHandle == System.IntPtr.Zero)
            {
                DebugLogger.GetInstance.WriteToLog(
                    DebugLogger.EDebugLevel.DEBUG_ERROR, "No application initialized");
                return;
            }
            TangoApplicationAPI.ApplicationShutdown(m_applicationHandle);
            m_applicationHandle = System.IntPtr.Zero;
        }

        /// <summary>
        /// Perform constructor logic.
        /// </summary>
        private void Awake()
        {
            if (m_instance != null)
            {
                Destroy(this);
                return;
            }
            m_instance = this;
            InitApplication();
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Perform updates for the next frame after the
        /// normal update has run. This loads next frame.
        /// </summary>
        private void LateUpdate()
        {
            if (m_applicationHandle != System.IntPtr.Zero)
            {
                Common.RetCodes retCode = (Common.RetCodes)TangoApplicationAPI.ApplicationDoStep(m_applicationHandle);
                if (retCode != Common.RetCodes.kCAPISuccess)
                {
                    DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR,
                                                       "Application do step error: " + retCode);
                }
                m_valid = retCode == Common.RetCodes.kCAPISuccess;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        
        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void OnDestroy()
        {
            if (m_applicationHandle != System.IntPtr.Zero)
            {
                TangoApplicationAPI.ApplicationShutdown(m_applicationHandle);
            }
        }

        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Interface for native function calls to Tango API.
        /// </summary>
        private struct TangoApplicationAPI
        {
            #if (UNITY_EDITOR)

            /// <summary>
            /// Initialize Tango API.
            /// </summary>
            /// <param name="dataSource"> Sparse file location.</param>
            /// <returns> Application handle.</returns>
            public static System.IntPtr ApplicationInitialize([MarshalAs(UnmanagedType.LPStr)] string dataSource)
            {
                return System.IntPtr.Zero;
            }
            
            /// <summary>
            /// Perform update on Tango API.
            /// </summary>
            /// <param name="applicationHandler"> Handle to the application context.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if the application was successful.</returns>
            public static int ApplicationDoStep(System.IntPtr applicationHandler)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Shutdown Tango API.
            /// </summary>
            /// <param name="applicationHandler"> Handle to the application context.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if the application was successful.</returns>
            public static int ApplicationShutdown(System.IntPtr applicationHandler)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern System.IntPtr ApplicationInitialize([MarshalAs(UnmanagedType.LPStr)] string dataSource);   
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int ApplicationDoStep(System.IntPtr applicationHandler);   
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int ApplicationShutdown(System.IntPtr applicationHandler); 
            #else
            #error platform is not supported
            #endif
        }
        #endregion // NATIVE_FUNCTIONS
    }
}
