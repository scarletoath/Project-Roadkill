//-----------------------------------------------------------------------
// <copyright file="NotificationErrorHandler.cs" company="Google">
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
    /// Notification class.
    /// </summary>
    public class NotificationErrorHandler : ErrorHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
        private string mErrorMessage = "";
        private bool mErrorOccurred = false;
        private string mTitle = "Error";
        #endregion // PRIVATE_MEMBER_VARIABLES
        
        
        #region PUBLIC_METHODS
        /// <summary>
        /// Output error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public override void presentErrorMessage (string message)
        {
            mErrorOccurred = true;
            mErrorMessage = message;
        }
        
        #endregion // PUBLIC_METHODS

        #region UNTIY_MONOBEHAVIOUR_METHODS
        /// <summary>
        /// Unity GUI call back.
        /// </summary>
        private void OnGUI ()
        {
            if (mErrorOccurred)
                GUI.Window (0, new Rect (0, 0, Screen.width, Screen.height), drawError, mTitle);
        }
        
        #endregion // UNTIY_MONOBEHAVIOUR_METHODS
        
        #region PRIVATE_METHODS
        /// <summary>
        /// Render draw error message.
        /// </summary>
        /// <param name="id">Error id.</param>
        private void drawError (int id)
        {
            GUI.Label (new Rect (10, 25, Screen.width - 20, Screen.height - 95), mErrorMessage);
            
            if (GUI.Button (new Rect (Screen.width / 2 - 75, Screen.height - 60, 150, 50), "Close"))
                Common.Quit();
        }
        
        #endregion // PRIVATE_METHODS
    }
}