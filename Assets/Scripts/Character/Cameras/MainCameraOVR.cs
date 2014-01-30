using UnityEngine;

public class MainCameraOVR : MainCameraBase
{
	#region fields

		Camera leftCamera;
		Camera rightCamera;

		Vignetting leftVignette;
		Vignetting rightVignette;

		OVRCameraController ovrCameraController;


	
			float _vignette = 0;
		public override float vignette
		{
			get { return _vignette; }
			set
			{
				_vignette = value;

				leftVignette.intensity = rightVignette.intensity  = kMaxVignetting * _vignette;

				if ( leftVignette.enabled != _vignette > 0 )
					leftVignette.enabled = rightVignette.enabled = _vignette > 0;
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
				leftCamera.fieldOfView = rightCamera.fieldOfView = originalFov + (kNarrowFov - originalFov) * _zoom;
				Vector3 oldPos = transform.localPosition;
				transform.localPosition = new Vector3 ( oldPos.x, oldPos.y, originalZ + (kNarrowZ - originalZ) * _zoom);
			}
		}

	#endregion

	
	void Awake()
	{
		leftCamera = transform.Find("CameraLeft").camera;
		rightCamera = transform.Find("CameraRight").camera;
		
		leftVignette = leftCamera.GetComponent<Vignetting>();
		rightVignette = rightCamera.GetComponent<Vignetting>();

		ovrCameraController = GetComponent<OVRCameraController>();

		originalLayerMask = leftCamera.cullingMask;
		originalFov = leftCamera.fieldOfView;
		originalZ = transform.localPosition.z;
	}
	
	
	
	public override void Dispossess()
	{
		ovrCameraController.FollowOrientation = transform;
	}



	public override void HideHead()
	{
		leftCamera.cullingMask &= ~(Layers.Mask(Layers.Layer.PlayerHead));
		rightCamera.cullingMask &= ~(Layers.Mask(Layers.Layer.PlayerHead));
	}


	
	public override void InitializeForNewCharacter(Transform characterRootTransform)
	{
		ovrCameraController.FollowOrientation = characterRootTransform;
	}



	public override bool IsVR()
	{
		return true;
	}
	
	
	
	public override void ShowHead()
	{
		leftCamera.cullingMask = originalLayerMask;
		rightCamera.cullingMask = originalLayerMask;
	}
	
	
	
}
