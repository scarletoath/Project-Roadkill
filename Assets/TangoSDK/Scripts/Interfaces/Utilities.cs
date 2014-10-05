//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Google">
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
    /// Tango API utilities functions.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Data used for tracking debug statistics.
        /// </summary>
        public struct StatisticsData 
        {
            private int frames_tested;
            private int tears;
            private int jumps;
            private int stutters;
            private uint write_queue_backlog;
            private uint number_of_written_frames;
            private uint validation_result;
            private int exposure;
        }

        /// <summary>
        /// Get the version of Tango API.
        /// </summary>
        /// <returns> String representing the current version of the API.</returns>
        public static string GetVersionString()
        {
            IntPtr buf = IntPtr.Zero;
            string versionString = string.Empty;
            try
            {
                const int BufferSize = 1024;
                buf = Marshal.AllocHGlobal(BufferSize);
                TangoUtilAPI.UtilAPIVersionString(buf, BufferSize);
                versionString = Marshal.PtrToStringAnsi(buf);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
            finally 
            {
                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
            return versionString;
        }
        
        /// <summary>
        /// Print the API version number to the debug log.
        /// </summary>
        public static void PrintVersionString()
        {
            Debug.Log(GetVersionString());
        }

        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Interface to API utility functionality.
        /// </summary>
        public struct TangoUtilAPI
        {
            #if (UNITY_EDITOR)
            /// <summary>
            /// Convert pose data from VIO to Unity format.
            /// </summary>
            /// <param name="estimatorFormatRotation"> Raw rotation from VIO.</param>
            /// <param name="estimatorFormatTranslation"> Raw position from VIO.</param>
            /// <param name="unityFormatRotation"> Altered rotation for Unity.</param>
            /// <param name="unityFormatTranslation"> Altered position for Unity.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int UtilConvertPoseToUnityFormat(
                ref Quaternion estimatorFormatRotation, 
                ref Vector3 estimatorFormatTranslation,
                ref Quaternion unityFormatRotation,
                ref Vector3 unityFormatTranslation)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <returns> Not implemented.</returns>
            public static int UtilStatisticsSizeofStatsData()
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <param name="data"> Not implemented.</param>
            /// <returns> Not implemented.</returns>
            public static int UtilGetStatistics(ref StatisticsData data)
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Get API version.
            /// </summary>
            /// <param name="version_buffer"> Buffer to fill with API version.</param>
            /// <param name="maxSize"> Buffer size.</param>
            public static void UtilAPIVersionString(System.IntPtr version_buffer, int maxSize)
            {
                return;
            }
            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int UtilConvertPoseToUnityFormat(
                ref Quaternion estimatorFormatRotation, 
                ref Vector3 estimatorFormatTranslation,
                ref Quaternion unityFormatRotation,
                ref Vector3 unityFormatTranslation);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int UtilStatisticsSizeofStatsData();

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int UtilGetStatistics(ref StatisticsData data);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern void UtilAPIVersionString(System.IntPtr version_buffer, int maxSize);

            #else
            #error platform is not supported
            #endif
        }
        #endregion // NATIVE_FUNCTIONS
    }
}
