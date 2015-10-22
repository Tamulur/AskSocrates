/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections;

public class OVRPlatformMenu : MonoBehaviour
{
	public GameObject cursorTimer;
	public Color cursorTimerColor = new Color(0.0f, 0.643f, 1.0f, 1.0f);	// set default color to same as native cursor timer
	public OVRCameraRig cameraRig = null;
	public float fixedDepth = 3.0f;

	private GameObject instantiatedCursorTimer = null;
	private Material cursorTimerMaterial = null;
	private float shortPressDelay = 0.25f;
	private float longPressDelay = 0.75f;

	enum eBackButtonAction
	{
		NONE,
		DOUBLE_TAP,
		SHORT_PRESS,
		LONG_PRESS
	};

	private int     downCount = 0;
	private float 	initialDownTime = -1.0f;
	private float   timerTime = -1.0f;
	private bool 	waitForUp = false;

	eBackButtonAction ResetAndSendAction( eBackButtonAction action )
	{
		print( "ResetAndSendAction( " + action + " );" );
		downCount = 0;
		initialDownTime = -1.0f;
        timerTime = -1.0f;
		waitForUp = false;
		ResetCursor();
		if ( action == eBackButtonAction.LONG_PRESS )
		{
			// since a long press triggers off of time and not an up,
			// wait for an up to happen before handling any more key state.
			waitForUp = true;
		}
		return action;
	}

	eBackButtonAction HandleBackButtonState() 
	{
		if ( waitForUp )
		{
			if ( !Input.GetKeyDown( KeyCode.Escape ) && !Input.GetKey( KeyCode.Escape ) )
			{
				waitForUp = false;
			}
			else
			{
				return eBackButtonAction.NONE;
			}
		}

        float timeSinceInitialDown = -1.0f;
        if ( downCount > 0 )
        {
            timeSinceInitialDown = Time.realtimeSinceStartup - initialDownTime;
        }

		if ( Input.GetKeyDown( KeyCode.Escape ) ) // only returns true on the frame that the key was pressed
		{
            // just came down
			downCount++;
			if ( downCount == 1 )
			{
                // initial down
				initialDownTime = Time.realtimeSinceStartup;
                timerTime = -1.0f;
			}
		}
        else if ( Input.GetKey( KeyCode.Escape ) )
        {
            // key is being held
            if ( timeSinceInitialDown > shortPressDelay )
            {
				// The gaze cursor timer should start unfilled once short-press time is exceeded
				// then fill up completely, so offset the times by the short-press delay.
				timerTime = ( timeSinceInitialDown - shortPressDelay ) / ( longPressDelay - shortPressDelay );
				UpdateCursor( timerTime );
            }
            if ( timeSinceInitialDown > longPressDelay )
            {
                // long-press time expired while holding, so issue a long-press
                // print( "Long-press: timeSinceInitialDown = " + timeSinceInitialDown + ", downCount = " + downCount );
                return ResetAndSendAction( eBackButtonAction.LONG_PRESS );
            }
        }
        else if ( downCount > 0 )
        {
            // key is up
            if ( timerTime >= 0.0f )
            {
                // any key up after the short-press delay has passed is an abort of a long-press
                // print( "Abort: timeSinceInitialDown = " + timeSinceInitialDown + ", downCount = " + downCount );
                return ResetAndSendAction( eBackButtonAction.NONE );
            }
            else if ( timeSinceInitialDown >= shortPressDelay )
            {
                timerTime = ( timeSinceInitialDown - shortPressDelay ) / ( longPressDelay - shortPressDelay );
                if ( downCount == 1 )   
                {
                    // key only went down once, this is a short-press
					// print( "Short-press: timeSinceInitialDown = " + timeSinceInitialDown + ", downCount = " + downCount );
					return ResetAndSendAction( eBackButtonAction.SHORT_PRESS );
                }
                else if ( downCount == 2 )
                {
                    // key went down twice, this is a double-tap
					// print( "double-tap: timeSinceInitialDown = " + timeSinceInitialDown + ", downCount = " + downCount );
					return ResetAndSendAction( eBackButtonAction.DOUBLE_TAP );
                }
            }
        }

		// down reset, but perform no action
		return eBackButtonAction.NONE;
	}

	/// <summary>
	/// Instantiate the cursor timer
	/// </summary>
	void Awake()
	{
		if (cameraRig == null)
		{
			Debug.LogError ("ERROR: missing camera controller object on " + name);
			enabled = false;
			return;
		}
		if ((cursorTimer != null) && (instantiatedCursorTimer == null)) 
		{
			//Debug.Log("Instantiating CursorTimer");
			instantiatedCursorTimer = Instantiate(cursorTimer) as GameObject;
			if (instantiatedCursorTimer != null)
			{
				cursorTimerMaterial = instantiatedCursorTimer.GetComponent<Renderer>().material;
				cursorTimerMaterial.SetColor ( "_Color", cursorTimerColor ); 
				instantiatedCursorTimer.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	/// <summary>
	/// Destroy the cloned material
	/// </summary>
	void OnDestroy()
	{
		if (cursorTimerMaterial != null)
		{
			Destroy(cursorTimerMaterial);
		}
	}

	/// <summary>
	/// Reset when resuming
	/// </summary>
	void OnApplicationFocus( bool focusState )
	{
		//Input.ResetInputAxes();
		//ResetAndSendAction( eBackButtonAction.LONG_PRESS );
	}

	/// <summary>
	/// Reset when resuming
	/// </summary>
	void OnApplicationPause( bool pauseStatus ) 
	{
		if ( !pauseStatus )
		{
			Input.ResetInputAxes();
		}
		//ResetAndSendAction( eBackButtonAction.LONG_PRESS );
	}

	/// <summary>
	/// Show the confirm quit menu
	/// </summary>
	void ShowConfirmQuitMenu()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log("[PlatformUI-ConfirmQuit] Showing @ " + Time.time);
		OVRManager.PlatformUIConfirmQuit();
#endif
	}

	/// <summary>
	/// Show the platform UI global menu
	/// </summary>
	void ShowGlobalMenu()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log("[PlatformUI-Global] Showing @ " + Time.time);
		OVRManager.PlatformUIGlobalMenu();
#endif
	}

	/// <summary>
	/// Tests for long-press and activates global platform menu when detected.
	/// as per the Unity integration doc, the back button responds to "mouse 1" button down/up/etc
	/// </summary>
	void Update()
	{
#if UNITY_ANDROID
		eBackButtonAction action = HandleBackButtonState();
		if ( action == eBackButtonAction.DOUBLE_TAP )
		{
			ResetCursor();
		}
		else if ( action == eBackButtonAction.SHORT_PRESS )
		{
			ResetCursor();
			ShowConfirmQuitMenu();
		}
		else if ( action == eBackButtonAction.LONG_PRESS )
		{
			ShowGlobalMenu();
		}
#endif
	}

	/// <summary>
	/// Update the cursor based on how long the back button is pressed
	/// </summary>
	void UpdateCursor(float timerRotateRatio)
	{
		timerRotateRatio = Mathf.Clamp( timerRotateRatio, 0.0f, 1.0f );
		if (instantiatedCursorTimer != null)
		{
			instantiatedCursorTimer.GetComponent<Renderer>().enabled = true;

			// Clamp the rotation ratio to avoid rendering artifacts
			float rampOffset = Mathf.Clamp(1.0f - timerRotateRatio, 0.0f, 1.0f);
			cursorTimerMaterial.SetFloat ( "_ColorRampOffset", rampOffset );
			//print( "alphaAmount = " + alphaAmount );

			// Draw timer at fixed distance in front of camera
			// cursor positions itself based on camera forward and draws at a fixed depth
			Vector3 cameraForward = Camera.main.transform.forward;
			Vector3 cameraPos = Camera.main.transform.position;
			instantiatedCursorTimer.transform.position = cameraPos + (cameraForward * fixedDepth);
			instantiatedCursorTimer.transform.forward = cameraForward;
		}
	}

	void ResetCursor()
	{
		if (instantiatedCursorTimer != null)
		{
			cursorTimerMaterial.SetFloat("_ColorRampOffset", 1.0f);
			instantiatedCursorTimer.GetComponent<Renderer>().enabled = false;
			//print( "ResetCursor" );
		}
	}
}
