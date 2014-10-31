using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class SoundSource : MonoBehaviour
{
	#region fields
	
		public bool isMusic = false;
		public AudioClip[] clips;
		new AudioSource audio;
		float originalVolume;
	
		public bool isPlaying {
			get { return audio.isPlaying; }
		}
	
		public float volume {
			get { return audio.volume; }
			set { audio.volume = value; }
		}
	
		public float maxDistance {
			get { return audio.maxDistance; }
			set { audio.maxDistance = value; }
		}
	
	#endregion
	
	
	
	void Awake ()
	{
		audio = base.audio;
		originalVolume = audio.volume;
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
		Singletons.soundManager.OnVolumeChanged += OnVolumeChanged;
	}
	
	
	
	public bool IsPlaying()
	{
		return audio.isPlaying;
	}
	
	
	
	void OnDestroy ()
	{
		if ( Singletons.timeManager != null )
		{
			Singletons.timeManager.OnTimeWarpChangedEvent -= OnTimeWarpChanged;
			Singletons.soundManager.OnVolumeChanged -= OnVolumeChanged;
		}
	}
	
	
	
	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{
		audio.pitch = absoluteTimeWarp;
	}
	
	
	
	void OnVolumeChanged( float volume )
	{
		audio.volume = volume * originalVolume;
	}



	public void Play ()
	{
		if ( false == audio.enabled )
			return;
		
		if ( clips.Length > 0 )
			audio.clip = clips [ Random.Range (0, clips.Length-1) ];
		audio.pitch = Singletons.timeManager.timeWarp;
		audio.Play ();
	}
	
	
	
	public void Stop ()
	{
		audio.Stop ();
	}
	
}
