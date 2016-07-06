using System;
using UnityEngine;


public class CameraFader : MonoBehaviour
{
	#region fields

		public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

				float _alpha;
		public float alpha {
			get { return _alpha;  }
			set {
					_alpha = value;
					Color color = fadeColor;
					color.a = _alpha;
					fadeMaterial.color = color;
				}
			}


		private Material fadeMaterial;
		private bool isFading;

		GoTween fadeTween;

	#endregion



	void Awake()
	{
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
		alpha=1;
	}



	public void FadeIn( float duration=2, Action<CameraFader> onComplete = null )
	{
		if ( fadeTween != null )
			fadeTween.destroy();

		isFading = true;
		alpha = 1;
		fadeTween = Go.to(this, duration, new GoTweenConfig()
								.floatProp("alpha", 0)
								.setEaseType(GoEaseType.SineOut)
								.onComplete( t => {
											isFading = false;
											fadeTween = null;
											if ( onComplete != null )
												onComplete(this);
											} ));
	}



	public void FadeOut( float duration=2, Action<CameraFader> onComplete = null )
	{
		if ( fadeTween != null )
			fadeTween.destroy();

		isFading = true;
		fadeTween = Go.to(this, duration, new GoTweenConfig()
								.floatProp("alpha", 1f)
								.setEaseType(GoEaseType.SineIn)
								.onComplete( t => {
											isFading = false;
											fadeTween = null;
											if ( onComplete != null )
												onComplete(this);
											} ));
	}



	void OnDestroy()
	{
		if (fadeMaterial != null)
		{
			Destroy(fadeMaterial);
		}
	}



	void OnPostRender()
	{
		if (isFading || alpha > 0)
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}



	public void SetToDark()
	{
		if ( fadeTween != null )
			fadeTween.destroy();

		fadeTween = null;
		isFading = false;

		alpha = 1;
	}




}
