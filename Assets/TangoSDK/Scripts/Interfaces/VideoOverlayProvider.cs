//-----------------------------------------------------------------------
// <copyright file="VideoOverlayProvider.cs" company="Google">
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
    /// Video Overlay Provider class provide video functions
    /// to get frame textures.
    /// </summary>
    public class VideoOverlayProvider
    {
        private static bool m_isInit = false;

        #region NESTED
        /// <summary>
        /// Rendering quality mode
        /// Defined background screen texture resolution.
        /// </summary>
        public enum Mode
        {
            QualityOptimized,
            PerformanceAndQuality,
            PerformanceOptimized,
        }
        #endregion
        
        /// <summary>
        /// Initialize the video overlay provider.
        /// </summary>
        public static void Init()
        {
            m_isInit = true;
            TangoVideoOverlayAPI.VideoOverlayInitialize(TangoApplication.Instance.Handle);
        }

        /// <summary>
        /// Render the latest frame.
        /// </summary>
        /// <param name="textureID"> Texture handle.</param>
        /// <param name="width"> Width of the texture.</param>
        /// <param name="height"> Height of the texture.</param>
        /// <param name="timestamp"> Timestamp to be filled out by the Tango API.</param>
        public static void RenderLatestFrame(int textureID, int width, int height, ref double timestamp)
        {
            if (!m_isInit)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN,
                                                   "VideoOveralyProvider.RenderLatestFrame : Not Initialized!");
                return;
            }

            TangoVideoOverlayAPI.VideoOverlayRenderLatestFrame(TangoApplication.Instance.Handle,
                                                               textureID, 
                                                               width, 
                                                               height, 
                                                               ref timestamp);
        }

        /// <summary>
        /// Shutdown the video overlay provider.
        /// </summary>
        public static void Shutdown()
        {
            m_isInit = false;
            TangoVideoOverlayAPI.VideoOverlayShutdown(TangoApplication.Instance.Handle);
        }

        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Video overlay native function import.
        /// </summary>
        private struct TangoVideoOverlayAPI
        {
            #if (UNITY_EDITOR)
            /// <summary>
            /// Initialize the video overlay.
            /// </summary>
            /// <param name="applicationHandler"> Pointer to the application context.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VideoOverlayInitialize(System.IntPtr applicationHandler)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Fill out latest RGB frame.
            /// </summary>
            /// <param name="videoOverlayHandler"> Pointer to application context.</param>
            /// <param name="textureID"> Texture handle.</param>
            /// <param name="width"> Width of the texture.</param>
            /// <param name="height"> Height of the texture.</param>
            /// <param name="timestamp"> Timestamp filled out by Tango API.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VideoOverlayRenderLatestFrame(System.IntPtr videoOverlayHandler,
                                                            int textureID,
                                                            int width,
                                                            int height,
                                                            ref double timestamp)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Shutdown video overlay.
            /// </summary>
            /// <param name="videoOverlayHandler"> Pointer to application context.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VideoOverlayShutdown(System.IntPtr videoOverlayHandler)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VideoOverlayInitialize(System.IntPtr applicationHandler);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VideoOverlayRenderLatestFrame(
                System.IntPtr videoOverlayHandler, int textureID, int width, int height, ref double timestamp);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VideoOverlayShutdown(System.IntPtr videoOverlayHandler);
            #else

            #error platform is not supported
            #endif
            #endregion
        }
    }
}
