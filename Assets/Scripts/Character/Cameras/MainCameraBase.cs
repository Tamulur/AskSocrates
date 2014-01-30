using UnityEngine;

public abstract class MainCameraBase : MonoBehaviour
{
	#region fields

		protected const float kMaxVignetting = 10;

		abstract public float vignette { get; set; }

		protected const float kFovChangeDuration = 1;
		protected const float kNarrowFov = 55; 
		protected const float kNarrowZ = 0.02f;
		protected int originalLayerMask;
		protected float originalZ;
		protected float originalFov;

		GoTween zoomTween;
	
	#endregion

	
	public virtual void Dispossess()
	{}

	
	public abstract void HideHead();



	public virtual void InitializeForNewCharacter(Transform characterRootTransform)
	{}



	public abstract bool IsVR();
	
	
	public void Narrower ()
	{
		if ( zoomTween != null )
			zoomTween.destroy ();

		zoomTween = Go.to ( this, kFovChangeDuration, new GoTweenConfig ().floatProp ("zoom", 1).onComplete ( t => OnNarrowed ()));
	}



	void OnNarrowed ()
	{
		zoomTween = null;
	}


	void OnWidened ()
	{
		zoomTween = null;
	}


	
	public abstract void ShowHead();
	
	
	
	public void Wider ()
	{
		if ( zoomTween != null )
			zoomTween.destroy ();

		zoomTween = Go.to ( this, kFovChangeDuration, new GoTweenConfig ().floatProp ("zoom", 0).onComplete ( t => OnWidened ()));
	}


}
