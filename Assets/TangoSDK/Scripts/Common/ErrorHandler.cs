//-----------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Google">
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
    /// Error handler class.
    /// </summary>
	public class ErrorHandler : MonoBehaviour
	{
		private static ErrorHandler m_errorHandler;

        /// <summary>
        /// Singleton instance.
        /// </summary>
		public static ErrorHandler instance
        {
			get
            {
                if (m_errorHandler == null)
                {
                    GameObject errorHandlerObject = new GameObject("ErrorHandler");
                    m_errorHandler = errorHandlerObject.AddComponent<ErrorHandler>();
				}

                return m_errorHandler;
			}
		}

        /// <summary>
        /// Output error message.
        /// </summary>
        /// <param name="message">The error message.</param>
		public virtual void presentErrorMessage (string message)
		{
			DebugLogger.GetInstance.WriteToLog(DebugLogger.EDebugLevel.DEBUG_ERROR, " :Tango error:" + message);
		}
        
        /// <summary>
        /// Unity awake callback and init the error handler.
        /// </summary>
        private void Awake()
        {
            m_errorHandler = this;
        }
	}
}