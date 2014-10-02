//-----------------------------------------------------------------------
// <copyright file="OrientationManager.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;

namespace Tango
{
    /// <summary>
    /// Manages the orientation of the screen.
    /// </summary>
	public class OrientationManager
	{
        /// <summary>
        /// Gets the Screen orientation.
        /// </summary>
        /// <returns> Returns the current orientation of the screen. </returns>
		public static ScreenOrientation GetScreenOrientation()
        {
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX)
			if (Screen.width > Screen.height)
            {
				return ScreenOrientation.LandscapeLeft;
            }
			else
            {
				return ScreenOrientation.Portrait;
            }
		#elif (UNITY_IPHONE || UNITY_ANDROID)
			return Screen.orientation; 
		#else 
			#error not supported platform
		#endif
		}
		
        /// <summary>
        /// Get the current world rotation.
        /// </summary>
        /// <returns> Returns a Quaternion representing the current world rotation.</returns>
		public static Quaternion GetWorldRotation()
        {
			ScreenOrientation orientation = GetScreenOrientation();
			Quaternion transformation = Quaternion.identity;
			if (orientation == ScreenOrientation.LandscapeLeft)
            {
				transformation = Quaternion.identity;
			}
            else if (orientation == ScreenOrientation.LandscapeRight)
            {
				transformation = Quaternion.AngleAxis(180f, Vector3.forward);
			}
            else if (orientation == ScreenOrientation.PortraitUpsideDown)
            {
				transformation = Quaternion.AngleAxis(90f, Vector3.forward);
			}
            else if (orientation == ScreenOrientation.Portrait)
            {
				transformation = Quaternion.AngleAxis(-90f, Vector3.forward);
			}
			return transformation;
		}
	}
}