//-----------------------------------------------------------------------
// <copyright file="DepthProvider.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tango
{
    /// <summary>
    /// Provide depth buffer (image) related functions.
    /// </summary>
    public class DepthProvider
    {
        private static bool m_isInit = false;

        /// <summary>
        /// Initialize the depth provider.
        /// </summary>
        public static void Init() 
        {
            m_isInit = true;
            TangoDepthAPI.DepthStartBuffering(TangoApplication.Instance.Handle);
        }

        /// <summary>
        /// Gets the latest depth frame.
        /// </summary>
        /// <param name="buffer"> Buffer to be filled out for depth.</param>
        /// <param name="timestamp"> Gets filled with the current depth timestamp.</param>
        /// <returns> True if the API returns success.</returns>
        public static bool GetLatestFrame(ref int[] buffer, ref double timestamp)
        {
            if (!m_isInit)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN,
                                                   "DepthProvider.GetLatestFrame : Not Initialized!");
                return false;
            }

            if (buffer.Length < Common.DepthBufferSize)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN, "Wrong depth buffer size");
                return false;
            }
            GCHandle bufferHandler = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Common.RetCodes retCode = (Common.RetCodes)TangoDepthAPI.DepthGetLatestFrame(TangoApplication.Instance.Handle, 
                                                                                         bufferHandler.AddrOfPinnedObject(),
                                                                                         Common.DepthBufferSize,
                                                                                         ref timestamp);
            if (retCode != Common.RetCodes.kCAPISuccess)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN, 
                                       "Failed to get depth buffer with error:" + retCode);
            }
            bufferHandler.Free();
            return retCode == Common.RetCodes.kCAPISuccess;
        }

        /// <summary>
        /// Shutdown the depth provider.
        /// </summary>
        public static void ShutDown()
        {
            m_isInit = false;
            TangoDepthAPI.DepthStopBuffering(TangoApplication.Instance.Handle);
        }

        /// <summary>
        /// Wraps depth functionality from libtango_api.
        /// </summary>
        private struct TangoDepthAPI
        {
            #if (UNITY_EDITOR)

            /// <summary>
            /// Gets the latest depth frame.
            /// </summary>
            /// <param name="application"> Pointer to the application context.</param>
            /// <param name="depth_image_buffer"> The buffer to be filled out with depth information.</param>
            /// <param name="bufferSize"> Size of the buffer allocated in managed memory.</param>
            /// <param name="timestamp"> Timestamp to be filled out by C API.</param>
            /// <returns> Integer flag containing status of this call.</returns>
            public static int DepthGetLatestFrame(
                System.IntPtr application, System.IntPtr depth_image_buffer, int bufferSize,
                [In, Out] ref double timestamp)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Tells the Tango API to begin depth buffering.
            /// </summary>
            /// <param name="application"> Pointer to the application context.</param>
            /// <returns> Integer flag containing status of this call.</returns>
            public static int DepthStartBuffering(System.IntPtr application)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Tells the Tango API to stop depth buffering.
            /// </summary>
            /// <param name="application"> Pointer to the application context.</param>
            /// <returns> Integer flag containing status of this call.</returns>
            public static int DepthStopBuffering(System.IntPtr application)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int DepthGetLatestFrame(System.IntPtr application, 
                                                         System.IntPtr depthBuffer, 
                                                         int bufferSize,
                                                         [In, Out] ref double timestamp);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int DepthStartBuffering(System.IntPtr application);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int DepthStopBuffering(System.IntPtr application);
            

            
            #else
            #error platform is not supported
            #endif
        }
    }
}
