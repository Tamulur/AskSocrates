using System;
using UnityEngine;



public class GameManagerBase : MonoBehaviour
{
	# region fields

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
		FadeIn( duration: 2, then: StartGame );
	}



	protected virtual void StartGame()
	{
		isGameStarted = true;
		if ( OnGameStart != null )
			OnGameStart();
	}



	protected virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		if ( fadeState == FadeState.FadingIn || fadeState == FadeState.FadingOut )
			return;

		//*** Check keyboard
		{
			if ( Input.GetKeyDown (KeyCode.F5))
				ResetOVR();

			if ( isGameStarted )
				CheckKeyboardForRunningGame();
		}
	}

}
