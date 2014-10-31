using UnityEngine;



public class TimeManager : MonoBehaviour
{
	#region fields
	
		const float kWarpOutDuration = 0.1f;
		const float kWarpInDuration = 0.2f;
		const float kSlowTime = 0.9f;
	
		float normalTime = 1;
	
				float _timeWarp = 1;
		public float timeWarp {
			get { return _timeWarp; }
			set {
				Time.timeScale = value;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
				_timeWarp = value;
				if ( OnTimeWarpChangedEvent != null )
					OnTimeWarpChangedEvent ( timeWarp01: Mathf.InverseLerp(kSlowTime, normalTime, _timeWarp), absoluteTimeWarp: _timeWarp );
			}
		}
		
		// 0: slowest time
		// 1: normal time
		public delegate void OnTimeWarpChangedCallback ( float timeWarp01, float absoluteTimeWarp );
		public event OnTimeWarpChangedCallback OnTimeWarpChangedEvent;
	
		public delegate void OnTimeWarpedInCallback ( );
		public event OnTimeWarpedInCallback OnTimeWarpedInEvent;
		
		public delegate void OnTimeWarpedOutCallback ( );
		public event OnTimeWarpedOutCallback OnTimeWarpedOutEvent;
		
		
		enum Status {
			NormalTime,
			WarpingIn,
			WarpedIn,
			WarpingOut
		}
		Status status = Status.NormalTime;
		
		
	#endregion
	
	
	void OnTimeWarpedIn ()
	{
		status = Status.WarpedIn;
		
		if ( OnTimeWarpedInEvent != null )
			OnTimeWarpedInEvent ();
	}
	
	
	
	void OnTimeWarpedOut ()
	{
		status = Status.NormalTime;
		
		if ( OnTimeWarpedOutEvent != null )
			OnTimeWarpedOutEvent ();
	}
	
	
	
	public void SetNormalTime ( float normalTime )
	{
		this.normalTime = normalTime;
	}
	
	
	
	public void WarpTimeIn ( System.Action onComplete = null )
	{
		if ( status == Status.WarpingIn || status == Status.WarpedIn )
		{
			if ( onComplete != null )
				onComplete();
			return;
		}
			
		status = Status.WarpingIn;
		
		Go.killAllTweensWithTarget( this );
		Go.to ( this, kWarpInDuration, new GoTweenConfig().floatProp ("timeWarp", kSlowTime)
					.onComplete ( t => {
												if ( onComplete != null )
													onComplete();
												OnTimeWarpedIn (); } ) );
	}
	
	
	
	public void WarpTimeOut ( System.Action onComplete = null )
	{
		if ( status == Status.WarpingOut || status == Status.NormalTime )
		{
			if ( onComplete != null )
				onComplete();
			return;
		}
				
		status = Status.WarpingOut;
		
		Go.killAllTweensWithTarget( this );
		Go.to ( this, kWarpOutDuration, new GoTweenConfig().floatProp ("timeWarp", normalTime)
					.onComplete ( t => {
												if ( onComplete != null )
													onComplete();
													OnTimeWarpedOut (); } ) );
	}
	
}
