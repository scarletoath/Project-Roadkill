//-----------------------------------------------------------------------
// <copyright file="ShowVioStatus.cs" company="Google">
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
    /// This script places two text fields in the upper left corner of an app.  One reports the VIO status,
    /// and the other reports whether or not the system is relocalized. It also provides a button for sending
    /// a reset when the tango API reports a status error.
    /// </summary>
    [RequireComponent(typeof(VIOController))]
    public class ShowVioStatus : MonoBehaviour 
    {
        private const string RESET_BUTTON_STRING = "Reset";
        private const int FONT_SIZE = 100;
        private const int RESET_BUTTON_WIDTH_OFFSET = -300;
        private const int RESET_BUTTON_HEIGHT_OFFSET = -100;
        private const int RESET_BUTTON_WIDTH = 600;
        private const int RESET_BUTTON_HEIGHT = 200;
        private const float GUI_AREA_PERCENT_X = 0.0f;
        private const float GUI_AREA_PERCENT_Y = 0.75f;
        private const float GUI_AREA_WIDTH = 600.0f;
        private const float GUI_AREA_HEIGHT = 300.0f;

        private const string BEGIN_TEXT_SIZE = "<size=20>";
        private const string END_TEXT_SIZE = "</size>";

        private const string INITIALIZING_STATUS_STRING = "Initializing";
        private const string RUNNING_STATUS_STRING = "Running";
        private const string TRACKING_LOST_STATUS_STRING = "Tracking Lost";
        private const string RECOVERING_STATUS_STRING = "Recovering";
        private const string INTERNAL_ERROR_NO_STATUS_STRING = "Internal error: Cannot get status!";
        private const string INTERNAL_ERROR_UNKNOWN_STATUS_STRING = "Internal error: Unknown status (";
        private const string INTERNAL_ERROR_CANNOT_QUERY_STATUS_STRING = "Internal error: Cannot query VIO status!";
        private const string LOCALIZED_STRING = "Localized";
        private const string UNLOCALIZED_STRING = "Unlocalized";

        // Current VIO Status
        private VIOController m_vioController;

        /// <summary>
        /// Starts vio status with empty status.
        /// </summary>
        private void Start() 
        {
            m_vioController = GetComponent<VIOController>();
        }

        /// <summary>
        /// Shows a message in the upper left part of the screen.
        /// </summary>
        /// <param name="text"> Text to show.</param>
        /// <param name="color"> Color to use for text.</param>
        private void _ShowMessage(string text, Color color) 
        {
            GUI.color = color;
            GUILayout.Label(BEGIN_TEXT_SIZE + text + END_TEXT_SIZE);
        }

        /// <summary>
        /// Shows a Reset button in the middle of the screen, enabling the user to send a reset
        /// when tracking is lost.
        /// </summary>
        private void _ShowResetButton()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = FONT_SIZE;
            buttonStyle.normal.textColor = Color.white;

            if (GUI.Button(new Rect((Screen.width / 2) + RESET_BUTTON_WIDTH_OFFSET,
                                    (Screen.height / 2) + RESET_BUTTON_HEIGHT_OFFSET,
                                    RESET_BUTTON_WIDTH, RESET_BUTTON_HEIGHT),
                           RESET_BUTTON_STRING, buttonStyle)) 
            {
                Tango.VIOProvider.Reset();
            }
        }

        /// <summary>
        /// Actually shows messages based on current status, as well as displaying the Reset button if tracking
        /// has been lost.
        /// </summary>
        private void OnGUI() 
        {
            // Start the GUI layout
            GUILayout.BeginArea(new Rect(Screen.width * GUI_AREA_PERCENT_X,
                                         Screen.height * GUI_AREA_PERCENT_Y,
                                         GUI_AREA_WIDTH,
                                         GUI_AREA_HEIGHT));
            GUILayout.BeginVertical();

            switch (m_vioController.GetVIOStatusCode()) 
            {
            case VIOProvider.CAPIOdometryStatusCodes.kCAPIInitializing:
                _ShowMessage(INITIALIZING_STATUS_STRING, Color.grey);
                break;
            case VIOProvider.CAPIOdometryStatusCodes.kCAPIRunning:
                _ShowMessage(RUNNING_STATUS_STRING, Color.green);
                break;
            case VIOProvider.CAPIOdometryStatusCodes.kCAPITrackingLost:
                _ShowMessage(TRACKING_LOST_STATUS_STRING, Color.red);
                break;
            case VIOProvider.CAPIOdometryStatusCodes.kCAPIRecovering:
                _ShowMessage(RECOVERING_STATUS_STRING, Color.grey);
                break;
            case 0:
                _ShowMessage(INTERNAL_ERROR_NO_STATUS_STRING, Color.red);
                break;
            default:
                _ShowMessage(INTERNAL_ERROR_UNKNOWN_STATUS_STRING + m_vioController.GetVIOStatusCode() + ")", Color.red);
                break;
            }
            
            if (m_vioController.IsRelocalized()) 
            {
                _ShowMessage(LOCALIZED_STRING, Color.green);
            } 
            else 
            {
                _ShowMessage(UNLOCALIZED_STRING, Color.red);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            // This needs to happen outside of the GUILayout area.
            if (m_vioController.GetVIOStatusCode() == VIOProvider.CAPIOdometryStatusCodes.kCAPITrackingLost)
            {
                _ShowResetButton();
            }
        }
    }
}