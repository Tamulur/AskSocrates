using System;
using UnityEngine;
using System.Collections.Generic;



public class ScreenFader : MonoBehaviour
{
	#region fields

		CameraFader[] cameraFaders;
		readonly HashSet<CameraFader> pendingFaders = new HashSet<CameraFader>(); 

		enum State {
			FadedOut,
			FadedIn,
			FadingOut,
			FadingIn
		}
		State state = State.FadedOut;

		public static ScreenFader instance;

	#endregion


	void Awake()
	{
		instance = this;
	}



	public void FadeIn( float duration = 1, Action onComplete=null )
	{
		if ( state == State.FadedIn )
		{
			if ( onComplete != null )
				onComplete();
			return;
		}

		if ( cameraFaders == null )
			cameraFaders = FindObjectsOfType<CameraFader>();
		pendingFaders.Clear();
		foreach (CameraFader cameraFader in cameraFaders)
			pendingFaders.Add(cameraFader);

		state = State.FadingIn;
		foreach (CameraFader cameraFader in cameraFaders)
			cameraFader.FadeIn( duration, onComplete: fader => {
									pendingFaders.Remove(fader);
									if ( pendingFaders.Count == 0 )
									{
										state = State.FadedIn;
										if ( onComplete != null )
											onComplete();
									} });
	}



	public void FadeOut( float duration=0.3f, Action onComplete=null )
	{
		if ( state == State.FadedOut )
		{
			if ( onComplete != null )
				onComplete();
			return;
		}

		if ( cameraFaders == null )
			cameraFaders = FindObjectsOfType<CameraFader>();
		pendingFaders.Clear();
		foreach (CameraFader cameraFader in cameraFaders)
			pendingFaders.Add(cameraFader);

		state = State.FadingOut;
		foreach (CameraFader cameraFader in cameraFaders)
			cameraFader.FadeOut( duration, onComplete: fader => {
										pendingFaders.Remove(fader);
										if ( pendingFaders.Count == 0 )
										{
											state = State.FadedOut;
											if ( onComplete != null )
												onComplete();
										} });
	}


	public void SetToDark()
	{
		foreach (CameraFader cameraFader in cameraFaders)
			cameraFader.SetToDark();

		state = State.FadedOut;
	}





}
