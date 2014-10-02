//-----------------------------------------------------------------------
// <copyright file="VIOProvider.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace Tango
{
    /// <summary>
    /// VIO (Visual Inertial Odometry) Provider provides pose data get from VIO.
    /// </summary>
    public class VIOProvider 
    {
        private static bool m_isInit = false;

#pragma warning disable 0414
        private static float MOUSE_LOOK_SENSITIVITY = 120; // Mouse sensitivity when using tango flycam
        private static float TRANSLATION_SPEED = 3f; // Translation speed to be used when using tango flycam
#pragma warning restore 0414

        #region NESTED
        /// <summary>
        /// VIO Mode.
        /// VisualIntertialNavigation: without sparse mapping and loop closure method.
        /// VisualIntertialNavigationAndMapping: enable sparse mapping and loop closure.
        /// </summary>
        public enum Mode
        {
            VisualIntertialNavigation = 0,
            VisualIntertialNavigationAndMapping = 1
        }

        /// <summary>
        /// CAPI return status.
        /// </summary>
        public enum CAPIOdometryStatusCodes
        {
            kCAPIInitializing = 500,
            kCAPIRunning = 501,
            kCAPITrackingLost = 502,
            kCAPIRecovering = 503,
            kCAPIUnknown = -1
        }

        /// <summary>
        /// VIO pose data, timestamp, and relocalizated flags.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  
        public struct VIOStatus 
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.Struct)]
            public Quaternion rotation;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
            public Vector3 translation;

            public double timestamp;
            public int relocalized;
            public CAPIOdometryStatusCodes status_code;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="relocalizedStatus">Relocalized status.</param>
            public VIOStatus(int relocalizedStatus)
            {
                rotation = Quaternion.identity;
                translation = Vector3.zero;
                timestamp = 0.0;
                relocalized = relocalizedStatus;
                status_code = CAPIOdometryStatusCodes.kCAPIUnknown;
            }
        }
        #endregion
        
        #region PUBLIC_METHODS
        /// <summary>
        /// Init VIO service.
        /// </summary>
        /// <param name="operationMode">Operation Mode.</param>
        /// <param name="sparseMapPath">Sparse map path. 
        /// If it's null, the application will run in loop closure mode.</param>
        public static void Init(Mode operationMode, string sparseMapPath)
        {
            int withSparseMapping = 
                (operationMode == Mode.VisualIntertialNavigationAndMapping) ? 1 : 0;
            Common.RetCodes retCode = (Common.RetCodes)TangoVIOAPI.VIOInitialize(TangoApplication.Instance.Handle, 
                                                                                 withSparseMapping, 
                                                                                 sparseMapPath);
            if (retCode != Common.RetCodes.kCAPISuccess)
            {
                Debug.Log("VIO initialization failed: " + retCode);
            }
            else
            {
                m_isInit = true;
            }
        }

        /// <summary>
        /// Set VIO Auto reset flag. If true, VIO will start to recover from a
        /// fatal tracking failure immediately.
        /// </summary>
        /// <param name="autoReset">Enable auto reset.</param>
        public static void SetAutoReset(bool autoReset)
        {
            TangoVIOAPI.VIOSetAutoReset (autoReset ? 1 : 0);
        }

        /// <summary>
        /// Save current recorded sparse map to file.
        /// </summary>
        /// <param name="sparseMapPath">Absolute location on phone.</param>
        /// <returns>If the function call succeed.</returns>
        public static bool SaveSparseMap(string sparseMapPath)
        {
            if (!m_isInit)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN,
                                                   "VIOProvider.SaveSparseMap : Not Initialized!");
                return false;
            }

            Common.RetCodes ret_code = 
                (Common.RetCodes)TangoVIOAPI.VIOSaveSparseMap(
                    TangoApplication.Instance.Handle, sparseMapPath);
            if (ret_code != Common.RetCodes.kCAPISuccess)
            {
                ErrorHandler.instance.presentErrorMessage(
                    "Application initialization failed" + ret_code); 
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get latest vioStatus.
        /// </summary>
        /// <param name="vioStatus">Reference to vioStatus, 
        /// need caller to allocate.</param>
        /// <returns>If the function call succeed.</returns>
        public static bool GetLatestPose(ref VIOStatus vioStatus)
        {
            if (!m_isInit)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN, "VIOProvider.GetLatestPose : Not Initialized!");
                return false;
            }

            Common.RetCodes ret_code = 
                (Common.RetCodes)TangoVIOAPI.VIOGetLatestPose(
                    TangoApplication.Instance.Handle, ref vioStatus);
            if (ret_code != Common.RetCodes.kCAPISuccess)
            {
                Debug.Log("get latest vio pose failed:" + ret_code);
                return false;
            }

            // Convert the estimator format to unity format
            Utilities.TangoUtilAPI.UtilConvertPoseToUnityFormat(
                ref vioStatus.rotation, ref vioStatus.translation,
                ref vioStatus.rotation, ref vioStatus.translation);
            return true;
        }

        /// <summary>
        /// Get vioStatus based on time.
        /// </summary>
        /// <param name="timestamp">Target timestamp.</param>
        /// <param name="maxDelta">Max timestamp delta between
        /// the target time and real time.</param>
        /// <param name="vioStatus">Reference to vioStatus, 
        /// need caller to allocate.</param>
        /// <returns>If the function call succeed.</returns>
        public static bool GetClosestPoseToTime(double timestamp, double maxDelta, VIOStatus vioStatus)
        {
            if (!m_isInit)
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_WARN,
                                                   "VIOProvider.GetClosestPoseToTime : Not Initialized!");
                return false;
            }

            Common.RetCodes ret_code = 
                (Common.RetCodes)TangoVIOAPI.VIOGetClosestPoseToTime(
                    TangoApplication.Instance.Handle, timestamp, maxDelta, ref vioStatus);
            if (ret_code != Common.RetCodes.kCAPISuccess)
            {
                Debug.Log("get latest vio pose failed:" + ret_code);
                return false;
            }

            // Convert the estimator format to unity format
            Utilities.TangoUtilAPI.UtilConvertPoseToUnityFormat(
                ref vioStatus.rotation, ref vioStatus.translation,
                ref vioStatus.rotation, ref vioStatus.translation);
            return true;
        }

        /// <summary>
        /// Reset VIO tracking.
        /// </summary>
        public static void Reset()
        {
            TangoVIOAPI.VIOReset(TangoApplication.Instance.Handle);
        }
		
        /// <summary>
        /// Shut down VIO service.
        /// </summary>
        public static void ShutDown()
        {
            m_isInit = false;
            TangoVIOAPI.VIOShutdown(TangoApplication.Instance.Handle);
        }

        #endregion
        
        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Imported native functions.
        /// </summary>
        private struct TangoVIOAPI
        {
            #if (UNITY_EDITOR)

            /// <summary>
            /// Initializes VIO (pose estimation).
            /// </summary>
            /// <param name="applicationHandler"> Pointer to application context.</param>
            /// <param name="withParseMappping"> True if this application use sparse mapping.</param>
            /// <param name="mapPath"> Path to the sparse map to be used.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOInitialize(System.IntPtr applicationHandler, 
                                            int withParseMappping, 
                                            [MarshalAs(UnmanagedType.LPStr)] string mapPath)
            {
                return (int)Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Gets the latest pose estimation from VIO. If run in Unity Editor Simulates TangoFlyCam Experience.
            /// </summary>
            /// <param name="vioHandler"> Pointer to application context.</param>
            /// <param name="vioStatus"> Data filled out by Tango API.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOGetLatestPose(System.IntPtr vioHandler, ref VIOStatus vioStatus)
            {
                Vector3 position = vioStatus.translation;
                Quaternion rotation;
                Vector3 directionForward, directionRight, directionUp;
                float rotationX;
                float rotationY;

                rotationX = vioStatus.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * MOUSE_LOOK_SENSITIVITY * Time.deltaTime;
                rotationY = vioStatus.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * MOUSE_LOOK_SENSITIVITY * Time.deltaTime;
                Vector3 eulerAngles = new Vector3(rotationX,rotationY,0);
                vioStatus.rotation.eulerAngles = eulerAngles;
                rotation = Quaternion.Euler(eulerAngles);

                directionForward = rotation * Vector3.forward;
                directionRight =  rotation * Vector3.right;
                directionUp = rotation * Vector3.up;
                position = position + Input.GetAxis("Vertical") * directionForward * TRANSLATION_SPEED * Time.deltaTime;
                position = position + Input.GetAxis("Horizontal") * directionRight * TRANSLATION_SPEED * Time.deltaTime;
                if(Input.GetKey(KeyCode.R)) // Go Up
                {
                    position += directionUp * TRANSLATION_SPEED * Time.deltaTime;
                }
                if(Input.GetKey(KeyCode.F))  // Go Down
                {
                    position -= directionUp * TRANSLATION_SPEED * Time.deltaTime;
                }
                vioStatus.translation = position;
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Gets the pose estimation closest to the timestamp
            /// given to the function.
            /// </summary>
            /// <param name="vioHandle"> Pointer to the application context.</param>
            /// <param name="timestamp"> Time that will be used to recover a pose estimation.</param>
            /// <param name="maxDelta"> Max difference between the given timestamp and returned
            /// pose estimation.</param>
            /// <param name="status"> Data to be filled out by Tango API.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOGetClosestPoseToTime(System.IntPtr vioHandle,
                                                      double timestamp,
                                                      double maxDelta,
                                                      ref VIOStatus status)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Set the auto-reset functionality in VIO.
            /// </summary>
            /// <param name="autoReset"> Not sure.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOSetAutoReset(int autoReset)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Resets VIO.
            /// </summary>
            /// <param name="vioHandle"> Pointer to application context.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOReset(System.IntPtr vioHandle)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Save a recorded sparse map.
            /// </summary>
            /// <param name="applicationHandle"> Pointer to the application context.</param>
            /// <param name="mapPath"> Path where new sparse map should be saved.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOSaveSparseMap(System.IntPtr applicationHandle, 
                                               [MarshalAs(UnmanagedType.LPStr)] string mapPath)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <param name="anchorId"> Not implemented.</param>
            /// <returns> Not implemented.</returns>
            public static int VIORegisterNewAnchor(ref char anchorId)
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <param name="numIds"> Not implemented.</param>
            /// <param name="registeredIds"> Not implemented.</param>
            /// <returns> Not implemented.</returns>
            public static int VIORegisterExistingAnchors(int numIds, ref char registeredIds)
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Not Implemented.
            /// </summary>
            /// <param name="registeredID"> Not implemented.</param>
            /// <param name="pose_g_T_A_transformation_matrix_Unity"> Not implemented.</param>
            /// <returns> Not implemented.</returns>
            public static int VIOQueryAnchorPoseUnity(ref char[] registeredID, 
                                                      ref float pose_g_T_A_transformation_matrix_Unity)
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <param name="num_ids"> Not implemented.</param>
            /// <param name="registered_ids"> Not implemented.</param>
            /// <returns> Not implemented.</returns>
            public static int VIOClearAnchors(int num_ids, ref char registered_ids)
            {
                return (int)Tango.Common.RetCodes.kCAPINotImplemented;
            }

            /// <summary>
            /// Shutdown VIO.
            /// </summary>
            /// <param name="vioHandler"> Pointer to the application context.</param>
            /// <returns> Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int VIOShutdown(System.IntPtr vioHandler)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOInitialize(
                System.IntPtr application_handler, 
                int withParseMappping, 
                [MarshalAs(UnmanagedType.LPStr)] string mapPath);  

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOGetLatestPose(System.IntPtr vioHandle, ref VIOStatus vioStatus);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOGetClosestPoseToTime(
                System.IntPtr vioHandle, double timestamp,
                double maxDelta, ref VIOStatus status);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOSetAutoReset(int autoReset);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOReset(System.IntPtr vioHandle);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOSaveSparseMap(
                System.IntPtr application_handler, [MarshalAs(UnmanagedType.LPStr)] string mapPath);  

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIORegisterNewAnchor(ref char anchor_id);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIORegisterExistingAnchors(int num_ids, ref char registered_ids);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOQueryAnchorPoseUnity(
                ref char[] registered_ID, ref float pose_g_T_A_transformation_matrix_Unity);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOClearAnchors(int num_ids, ref char registered_ids);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int VIOShutdown(System.IntPtr vioHandler); 
            #else
            #error platform is not supported
            #endif
        }
        #endregion // NATIVE_FUNCTIONS
    }
}