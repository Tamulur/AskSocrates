using UnityEngine;

public class MainCamera : MainCameraBase
{
	#region fields

		Vignetting vignetting;

			float _vignette = 0;
		public override float vignette
		{
			get { return _vignette; }
			set
			{
				_vignette = value;

				vignetting.intensity = kMaxVignetting * _vignette;

				if (vignetting.enabled != _vignette > 0)
					vignetting.enabled = _vignette > 0;
			}
		}

		float _zoom = 0;
			public float zoom
			{
				get {
					return _zoom;
				}

				set {
					_zoom = value;
					camera.fieldOfView = originalFov + (kNarrowFov - originalFov) * _zoom;
					Vector3 oldPos = transform.localPosition;
					transform.localPosition = new Vector3 ( oldPos.x, oldPos.y, originalZ + (kNarrowZ - originalZ) * _zoom);
				}
			}

	#endregion

	
	void Awake()
	{
		originalLayerMask = camera.cullingMask;
		originalFov = camera.fieldOfView;
		originalZ = transform.localPosition.z;

		vignetting = GetComponent<Vignetting>();
	}
	
	
	
	public override void HideHead()
	{
		camera.cullingMask &= ~(Layers.Mask(Layers.Layer.PlayerHead));
	}
	
	
	
	public override bool IsVR()
	{
		return false;
	}
	
	
	
	public override void ShowHead()
	{
		camera.cullingMask = originalLayerMask;
	}
	
}
