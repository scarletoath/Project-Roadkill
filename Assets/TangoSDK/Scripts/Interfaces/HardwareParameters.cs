//-----------------------------------------------------------------------
// <copyright file="HardwareParameters.cs" company="Google">
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
    /// Hardware parameters includes all the imported function 
    /// related to hardware.
    /// </summary>
    public class HardwareParameters
    {
        /// <summary>
        /// Stucture holding video mode attributes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VideoModeAttributes
        {
            public int width;
            public int height;
            public float frameRate;
            
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="w"> Width.</param>
            /// <param name="h"> Height.</param>
            /// <param name="fr"> Frame rate.</param>
            public VideoModeAttributes(int w, int h, float fr)
            {
                width = w;
                height = h;
                frameRate = fr;
            }
            
            /// <summary>
            /// Override for ToString().
            /// </summary>
            /// <returns> Custom string detailing member variables.</returns>
            public override string ToString()
            {
                return "NVideoModeAttributes:[" + width + "," + height + "," + frameRate + "]";
            }
        }

        /// <summary>
        /// Gets the video mode from the hardware.
        /// </summary>
        /// <param name="videoMode"> Reference to be filled out by the Tango API.</param>
        /// <returns> O if successful, otherwise it returns -1.</returns>
        static public int LookupVideoMode(ref VideoModeAttributes videoMode)
        { 
            int videoModeID = -1;
            IntPtr videoModePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VideoModeAttributes)));
            int numModes = Tango.HardwareParameters.HardwareParametersAPI.HardwareCameraNumberVideoModes();
            if (numModes > 0)
            {
                // choose the very first video mode
                Common.RetCodes res = (Common.RetCodes)Tango.HardwareParameters.HardwareParametersAPI.HardwareCameraGetVideoMode(0, videoModePtr);
                if (res == Common.RetCodes.kCAPISuccess)
                {
                    VideoModeAttributes supportedVideoMode = (VideoModeAttributes)Marshal.PtrToStructure(videoModePtr, typeof(VideoModeAttributes));
                    videoMode.frameRate = supportedVideoMode.frameRate;
                    videoMode.width = supportedVideoMode.width;
                    videoMode.height = supportedVideoMode.height;
                    videoModeID = 0;
                }
                else
                {
                    DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR, "Failed to get video mode with error:" + res);
                }
            }
            else
            {
                DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR, "Number of video modes:" + numModes);
            }
            Marshal.FreeHGlobal(videoModePtr);
            return videoModeID;
        }
        
        /// <summary>
        /// Returns the resolution.
        /// </summary>
        /// <param name="userVideoMode"> Contains video mode attributes.</param>
        /// <param name="screenResolution"> Resolution of the screen.</param>
        /// <returns> Resolution.</returns>
        public static Vector2 VisibleRect(VideoModeAttributes userVideoMode, Vector2 screenResolution)
        {
            float ha = (float)userVideoMode.width / screenResolution.x;
            float va = (float)userVideoMode.height / screenResolution.y;
            float windowAspect = screenResolution.x / screenResolution.y;
            
            Vector2 resolution = new Vector2();
            if (ha > va)
            {
                resolution.x = (float)userVideoMode.height * windowAspect;
                resolution.y = (float)userVideoMode.height;
            }
            else
            {
                resolution.x = (float)userVideoMode.width;
                resolution.y = (float)userVideoMode.width / windowAspect;
            }
            return resolution;
        }

        /// <summary>
        /// Gets the vertical field of view.
        /// </summary>
        /// <param name="visibleHeight"> TODO.</param>
        /// <returns> Vertical field of view.</returns>
        public static float VerticalFieldOfView(int visibleHeight)
        {
            // our calibration file has calibration for resolution 640x480 but not 1280x720
            // so lets hardcode fov for now
            return 39;

            // return HardwareParametersAPI.HardwareParametersCameraVerticalFieldOfView(visibleHeight);
        }
        
        /// <summary>
        /// Gets the horizontal field of view.
        /// </summary>
        /// <param name="visibleWidth"> TODO.</param>
        /// <returns> Horizontal field of view.</returns>
        public static float HorizontalFieldOfView(int visibleWidth)
        {
            return HardwareParametersAPI.HardwareCameraHorizontalFieldOfView(visibleWidth);
        }
        
        /// <summary>
        /// Sets the noise level of the hardware.
        /// This must be done before depth is started.
        /// </summary>
        /// <param name="noiseLevel"> A number x to x with x
        /// requesting the least noise.</param>
        /// <returns> True if the noise level was set.</returns>
        public bool SetDepthNoiseLevel(int noiseLevel)
        {
            return (int)Common.RetCodes.kCAPISuccess == HardwareParametersAPI.HardwareDepthSetNoiseLevel(noiseLevel);
        }
        
        /// <summary>
        /// Sets the depth confidence of the hardware.
        /// This must be done before depth is started.
        /// </summary>
        /// <param name="confidenceLevel">A number x to x with x
        /// requesting the most confidence.</param>
        /// <returns> True if the depth confidence was set.</returns>
        public bool SetDepthConfidenceLevel(int confidenceLevel)
        {
            return (int)Common.RetCodes.kCAPISuccess == HardwareParametersAPI.HardwareDepthSetConfidenceLevel(confidenceLevel);
        }
        
        /// <summary>
        /// Gets the current depth noise level.
        /// </summary>
        /// <returns> Integer value of the current depth noise level.</returns>
        public int GetDepthNoiseLevel()
        {
            int noiseLevel = 0;
            HardwareParametersAPI.HardwareDepthNoiseLevel(ref noiseLevel);
            return noiseLevel;
        }
        
        /// <summary>
        /// Gets the current depth confidence level.
        /// </summary>
        /// <returns>Integer value of the current depth confidence level.</returns>
        public int GetDepthConfidenceLevel()
        {
            int confidenceLevel = 0;
            HardwareParametersAPI.HardwareDepthConfidenceLevel(ref confidenceLevel);
            return confidenceLevel;
        }
        
        #region NATIVE_FUNCTIONS

        /// <summary>
        /// Interface for Hardware Parameters in the Tango API.
        /// </summary>
        public struct HardwareParametersAPI
        {
            #if (UNITY_EDITOR)
            /// <summary>
            /// Get the hardware camera horizontal field of view.
            /// </summary>
            /// <param name="visibleWidth"> TODO.</param>
            /// <returns> Hardware camera horizontal field of view.</returns>
            public static float HardwareCameraHorizontalFieldOfView(int visibleWidth)
            {
                return 0.0f;
            }
            
            /// <summary>
            /// Get the hardware camera vertical field of view.
            /// </summary>
            /// <param name="visibleHeight"> TODO.</param>
            /// <returns> Hardware camera vertical field of view.</returns>
            public static float HardwareCameraVerticalFieldOfView(int visibleHeight)
            {
                return 0.0f;
            }

            /// <summary>
            /// Hardware camera increment exposure.
            /// </summary>
            /// <param name="application"> Pointer to the application context to 
            /// obtain the hardware increment exposure.</param>
            /// <returns> Hardware camera increment exposure.</returns>
            public static float HardwareCameraIncrementExposure(System.IntPtr application)
            {
                return 0.0f;
            }

            /// <summary>
            /// Hardware camera decrement exposure.
            /// </summary>
            /// <param name="application"> Pointer to the application context to 
            /// obtain the hardware decrement exposure.</param>
            /// <returns> Hardware camera decrement exposure.</returns>
            public static float HardwareCameraDecrementExposure(System.IntPtr application)
            {
                return 0.0f;
            }

            /// <summary>
            /// Hardware camera size of video mode.
            /// </summary>
            /// <returns> TODO.</returns>
            public static int HardwareCameraSizeofVideoMode()
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Hardware camera number video modes.
            /// </summary>
            /// <returns> TODO.</returns>
            public static int HardwareCameraNumberVideoModes()
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Hardware camera get video mode.
            /// </summary>
            /// <param name="videoModeID"> TODO .</param>
            /// <param name="videoMode"> TODO +1.</param>
            /// <returns> TODO +2.</returns>
            public static int HardwareCameraGetVideoMode(int videoModeID, System.IntPtr videoMode)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Sets the depth confidence leve of the hardware.
            /// </summary>
            /// <param name="confidence_level"> A number x to x with x
            /// requesting the most confidence.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int HardwareDepthSetConfidenceLevel(int confidence_level)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Sets the Hardware depth noise level.
            /// </summary>
            /// <param name="noise_level"> A number x to x with x
            /// requesting the most confidence.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int HardwareDepthSetNoiseLevel(int noise_level)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            /// <summary>
            /// Gets the harware depth confidenct level.
            /// </summary>
            /// <param name="confidenceLevel"> Int ref to be filled out by the function.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int HardwareDepthConfidenceLevel(ref int confidenceLevel)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }

            /// <summary>
            /// Gets the harware depth noise level.
            /// </summary>
            /// <param name="noiseLevel"> Int ref to be filled out by the function.</param>
            /// <returns> Tango.Common.RetCodes.kCAPISuccess if successful.</returns>
            public static int HardwareDepthNoiseLevel(ref int noiseLevel)
            {
                return (int)Tango.Common.RetCodes.kCAPISuccess;
            }
            
            #elif (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_ANDROID)
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern float HardwareCameraHorizontalFieldOfView(int visibleWidth);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern float HardwareCameraVerticalFieldOfView(int visibleHeight);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern float HardwareCameraIncrementExposure(System.IntPtr application);

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern float HardwareCameraDecrementExposure(System.IntPtr application);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareCameraSizeofVideoMode();

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareCameraNumberVideoModes();

            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareCameraGetVideoMode(int videoModeID, System.IntPtr videoMode);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareDepthSetConfidenceLevel(int confidence_level);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareDepthSetNoiseLevel(int noise_level);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareDepthConfidenceLevel(ref int confidenceLevel);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int HardwareDepthNoiseLevel(ref int confidenceLevel);
            #else
            #error platform is not supported
            #endif
        }
        #endregion // NATIVE_FUNCTIONS
    }
}