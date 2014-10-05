//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Google">
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
	/// Runs plugin tasks.
	/// </summary>
	public class Plugin
	{
		public const int EVENTS_START_POINT = 0xfe;

        /// <summary>
        /// Supported plugin events.
        /// </summary>
		public enum PluginEvents
		{
            InitBackgroundEvent = EVENTS_START_POINT,
			RenderFrameEvent,
			ReleaseBackgroundEvent
		}
		
        /// <summary>
        /// Issues a GL plugin event.
        /// </summary>
        /// <param name="eventid"> Event to perform.</param>
		public static void IssuePluginEvent(PluginEvents eventid)
		{
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX)
			GL.IssuePluginEvent((int)eventid);
		#endif
		}

        /// <summary>
        /// Loads a shared library.
        /// </summary>
        /// <param name="libName"> Library name. </param>
		public static void LoadSharedLibrary(string libName)
		{
			AndroidJavaClass systemClass = new AndroidJavaClass("java.lang.System"); 
			systemClass.CallStatic("loadLibrary", libName);
		}
	}
}