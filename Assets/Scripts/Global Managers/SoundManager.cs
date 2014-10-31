using UnityEngine;

public class SoundManager : MonoBehaviour
{
	# region fields

		public delegate void OnVolumeChangedCallback ( float volume );
		public event OnVolumeChangedCallback OnVolumeChanged;

				float _volume;
		public float volume {
				get { return _volume; }
				set {
						_volume = value;
						if ( OnVolumeChanged != null )
							OnVolumeChanged( _volume );
					}
			}

		GoTween volumeTween;

	# endregion


	
	void Awake()
	{
		_volume = 1;
	}



	public void TweenVolumeTo( float newVolume, float duration )
	{
		if ( volumeTween != null )
			volumeTween.destroy();

		volumeTween = Go.to(this, duration, new GoTweenConfig()
										.floatProp("volume", newVolume)
										.onComplete( t => { volumeTween = null; } ));
	}

}
