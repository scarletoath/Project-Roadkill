//-----------------------------------------------------------------------
// <copyright file="Button.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// Base class Touchable is inherited.
/// Texture based effects added in this class.
/// </summary>
public class Button : TouchableObject
{
    public Texture2D m_onTouchTexture;
    public Texture2D m_outTouchTexture;
    protected string m_debugString = string.Empty;

    /// <summary>
    /// Touch event similar to key hold.
    /// Resets texture to show effect.
    /// </summary>
	protected override void OnTouch()
    {
        renderer.material.mainTexture = m_onTouchTexture;
	}

    /// <summary>
    /// Touch event similar to key down.
    /// Resets texture to show effect.
    /// </summary>
	protected override void OutTouch()
    {
        renderer.material.mainTexture = m_outTouchTexture;
	}

    /// <summary>
    /// Touch event similar to key up.
    /// Resets texture to show effect.
    /// </summary>
	protected override void TouchUp()
	{
        renderer.material.mainTexture = m_outTouchTexture;
	}
	
    /// <summary>
    /// Called every frame.
    /// </summary>
    protected override void Update()
	{
		base.Update();
	}
}
