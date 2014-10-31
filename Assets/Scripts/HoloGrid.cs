using UnityEngine;



public class HoloGrid : MonoBehaviour
{
	#region fields

		public Shader nonTransparentShader;

		float fadeSpeed;
		float targetOpacity;

		public float opacity {
				get { return _opacity; }
				set { _opacity = value;
						mat.color = new Color(1,1,1,_opacity); }
					}
					float _opacity;

        Material mat;

		enum State {
			Normal,
			Tweening,
			ConstantSpeed
		}
		State state = State.Normal;

		Shader transparentShader;

		GoTween fadeTween;

	#endregion



	public void Activate( float opacity = 0)
	{
		gameObject.SetActive( true );
		targetOpacity = this.opacity = opacity;
	}



    void Awake()
    {
        mat = renderer.material;
		transparentShader = mat.shader;
		fadeSpeed = 1;
    }



	public void ConstantSpeedTo( float targetOpacity, float fadeSpeed )
	{
		this.targetOpacity = targetOpacity;
		this.fadeSpeed = fadeSpeed;
		state = State.ConstantSpeed;

		if ( fadeTween != null )
		{
			fadeTween.destroy();
			fadeTween = null;
		}
	}



	public void Deactivate()
	{
		gameObject.SetActive( false );
	}



	public bool HasFadedInFully()
	{
		return opacity >= 1;
	}



	public void SetUseTransparentShader( bool useTransparentShader )
	{
		renderer.material.shader = useTransparentShader	? transparentShader
																					: nonTransparentShader;
	}



	public void TweenTo( float targetOpacity, float duration, float delay= 0, GoEaseType easeType=GoEaseType.Linear, System.Action onComplete=null )
	{
		Activate( opacity );
		state = State.Tweening;

		if ( fadeTween != null )
			fadeTween.destroy();
		
		fadeTween = Go.to( this, duration, new GoTweenConfig()
								.floatProp("opacity", targetOpacity)
								.setEaseType( easeType )
								.setDelay( delay )
								.onComplete( t => {
																fadeTween = null;
																state = State.Normal;
																if ( onComplete != null )
																	onComplete();
																}));
	}



	void Update()
	{
		if ( state == State.ConstantSpeed )
		{
			if ( targetOpacity > opacity )
				opacity = Mathf.Min(targetOpacity, opacity + Time.deltaTime * fadeSpeed);
			else if ( targetOpacity < opacity )
				opacity = Mathf.Max(0, opacity - Time.deltaTime * fadeSpeed);
			else
				state = State.Normal;
		}
	}


}
