using System;
using System.Collections;
using UnityEngine;



public class GameManagerBase : MonoBehaviour
{
	# region fields

		public bool hasRecentered { get; private set; }
		public bool isGameStarted { get; private set; }

		bool showingRecenterMessage;

		enum FadeState {
			Normal,
			FadingIn,
			Darkness,
			FadingOut
		}
		FadeState fadeState = FadeState.Darkness;

		public delegate void OnGameStartCallback ( );
		public event OnGameStartCallback OnGameStart;

	# endregion



	protected virtual void Awake()
	{
		if ( false == Application.isEditor )
		{
			Screen.showCursor = false;
			Screen.lockCursor = true;
		}

		OVRManager.instance.usePositionTracking = false;
	}



	protected virtual void CheckKeyboardForRunningGame()
	{ }
	


	public void FadeIn( float duration=2, Action then = null )
	{
		if ( fadeState != FadeState.Darkness )
		{
			Debug.LogWarning("FadeIn called in state " + fadeState);
			return;
		}

		fadeState = FadeState.FadingIn;
		Singletons.player.FadeFromDark( duration:duration,
														onComplete: () => {
														fadeState = FadeState.Normal;
														if ( then != null )
															then();
														});
	}


		
	public void FadeOut( Action then = null )
	{
		if ( fadeState != FadeState.Normal )
		{
			Debug.LogWarning("FadeOut called in state " + fadeState);
			return;
		}

		fadeState = FadeState.FadingOut;
		Singletons.player.FadeToDark( onComplete: () => {
											fadeState = FadeState.Darkness;
											if ( then != null )
												then();
											});
	}



	void ResetOVR()
	{
		OVRManager.display.RecenterPose();
	}
	

	
	void Start()
	{
		Singletons.player.SetToDark();
	}



	protected virtual void StartGame()
	{
		isGameStarted = true;
		if ( OnGameStart != null )
			OnGameStart();
	}



	IEnumerator ResetAndStartGameNextFrame()
	{
		yield return null;

		ResetOVR();
		OVRManager.instance.usePositionTracking = true;
		FadeIn( duration: 2, then: StartGame );
	}



	protected virtual void Update()
	{
		bool isHSWShowing = OVRManager.isHSWDisplayed;

		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
		else if (Input.anyKeyDown && isHSWShowing)
			OVRManager.DismissHSWDisplay();

		if ( fadeState == FadeState.FadingIn || fadeState == FadeState.FadingOut )
			return;

		if ( false == hasRecentered && false == isHSWShowing && false == showingRecenterMessage )
		{
			Singletons.guiManager.ShowMessage("Press F5 to recenter", 0, notificationMode:GUIManager.NotificationMode.FadeOut);
			showingRecenterMessage = true;
		}


		//*** Check keyboard
		{
			if ( Input.GetKeyDown (KeyCode.F5))
			{
				if ( showingRecenterMessage )
				{
					if ( false == isHSWShowing && false == hasRecentered )
					{
						Singletons.guiManager.HideGUI();
						showingRecenterMessage = false;
						hasRecentered = true;
						StartCoroutine(ResetAndStartGameNextFrame());
					}
				}
				else
					ResetOVR();
			}

			if ( isGameStarted )
				CheckKeyboardForRunningGame();
		}
	}

}
