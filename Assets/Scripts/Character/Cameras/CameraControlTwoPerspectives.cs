using UnityEngine;


public class CameraControlTwoPerspectives : MonoBehaviour
{
	#region fields
	
			float _zoom;
		public float zoom
		{
			get
			{
				return _zoom;
			}
			set
			{
				_zoom = value;
				if ( head != null )
					head.followOVRCameraFactor = 1 - _zoom;
			}
		}
	
		public Vector3 thirdPersonOffset = new Vector3(0.5f, -0.2f, -1.5f);
		public float sensitivityX = 1.0f;
		public float sensitivityY = 1.0f;
		public bool is3rdPersonOffsetLeft { get; set; }
		
		const float kZoomDuration = 0.5f;
		const float kSwooshDuration = 1.5f;
	
		const float kMinY = -90;
		const float kMaxY = 80;
		
		const float kMinX = -70;
		const float kMaxX = 70;
		
		Transform ownTransform;
		Transform cameraLeftTransform;
		Transform cameraRightTransform;
		Transform cameraAnchorRootTransform;
		Transform cameraMainPivotTransform;
		Transform cameraVerticalPivotTransform;
		
		Vector3 originalLocalPosition;
		Quaternion originalLocalRotation;
		Quaternion originalCameraVerticalPivotLocalRotation;
		Vector3 originalCameraPivotPosition;
		Quaternion originalCameraPivotRotation;
		AudioSource swooshSound;
	
		MainCameraBase mainCamera;
		Head head;

		float rotationY;
	
		public float stateTime;
	
		public enum State
		{
			FirstPerson,
			ZoomingIntoFirstPerson,
			ZoomingIntoThirdPerson,
			ThirdPerson,
			SwooshingToCharacter
		}
		public State state { get; private set; }
		
		public enum PerspectiveEvent
		{
			ZoomToThirdPerson,
			ZoomedIntoThirdPerson,
			ZoomToFirstPerson,
			ZoomedIntoFirstPerson,
			SwooshToCharacter,
			SwooshedToCharacter
		}
		
		public delegate void PerspectiveCallback ( PerspectiveEvent perspectiveEvent );
		public PerspectiveCallback OnPerspectiveChange;
	
	
	#endregion
	
	
	
	void Awake()
	{
		ownTransform = transform;
		cameraVerticalPivotTransform = ownTransform.parent;
		cameraMainPivotTransform = cameraVerticalPivotTransform.parent;
		
		Singletons.player .OnPlayerEvent += OnPlayerEvent;
		
		Transform ovrControllerTransform = ownTransform.Find("OVRCameraController");
		mainCamera = ovrControllerTransform.GetComponent<MainCameraOVR>();
		cameraLeftTransform = ovrControllerTransform.Find("CameraLeft");
		cameraRightTransform = ovrControllerTransform.Find("CameraRight");
		swooshSound = ownTransform.Find("SwooshSound").GetComponent<AudioSource>();
		
		zoom = 0;
		ownTransform.localPosition = Vector3.zero;
		state = State.FirstPerson;
	}



	public bool CanSwitchCharacter()
	{
		return state == State.FirstPerson || state == State.ThirdPerson;
	}



	void ControlCameraVR(float deltaTime)
	{
		Vector3 targetLocalPosition;
		Quaternion targetLocalRotation;
		Quaternion targetCameraVerticalPivotLocalRotation;
		Vector3 targetCameraMainPivotPosition;
		Quaternion targetCameraMainPivotRotation;

		//*** Determine the target positions and rotations
		{
			targetLocalRotation = Quaternion.identity;
			targetCameraVerticalPivotLocalRotation = Quaternion.identity;
			targetCameraMainPivotPosition = cameraAnchorRootTransform.position;
			targetCameraMainPivotRotation = cameraAnchorRootTransform.rotation;
		
			if (state == State.FirstPerson || state == State.ZoomingIntoFirstPerson)
				targetLocalPosition = Vector3.zero;
			else
			{
				targetLocalPosition = thirdPersonOffset;
				if ( is3rdPersonOffsetLeft )
					targetLocalPosition.x = -thirdPersonOffset.x;
			}
		}

		if (state == State.ZoomingIntoFirstPerson || state == State.ZoomingIntoThirdPerson)
		{
			float t = (state == State.ZoomingIntoFirstPerson)
							? (1 - zoom)
							: zoom;
			cameraMainPivotTransform.position = Vector3.Lerp(originalCameraPivotPosition, targetCameraMainPivotPosition, t);
			cameraMainPivotTransform.rotation = Quaternion.Slerp(originalCameraPivotRotation, targetCameraMainPivotRotation, t);
			cameraVerticalPivotTransform.localRotation = Quaternion.Slerp(originalCameraVerticalPivotLocalRotation, targetCameraVerticalPivotLocalRotation, t);
			ownTransform.localPosition = Vector3.Lerp(originalLocalPosition, targetLocalPosition, t);
			ownTransform.localRotation = Quaternion.Slerp(originalLocalRotation, targetLocalRotation, t);
		}
		else
		{
			cameraMainPivotTransform.position = targetCameraMainPivotPosition;
			cameraMainPivotTransform.rotation = targetCameraMainPivotRotation;
			cameraVerticalPivotTransform.localRotation = targetCameraVerticalPivotLocalRotation;
			ownTransform.localPosition = Vector3.Lerp(ownTransform.localPosition, targetLocalPosition, deltaTime * 10);
			ownTransform.localRotation = targetLocalRotation;
		}
	}



	public Vector3 GetLookTarget ()
	{
		Vector3 lookTarget = Vector3.zero;
		
			bool lookAtCameras = state == State.FirstPerson;
		if ( lookAtCameras )
			lookTarget = 0.5f * ( cameraLeftTransform.position + cameraRightTransform.position );
		else
			lookTarget = head .eyeCenter;
			
		return lookTarget;			
	}	
				
			
	
	void OnPlayerEvent ( Player.PlayerEvent playerEvent )
	{
		switch ( playerEvent )
		{
			case Player.PlayerEvent.Dispossess:
				cameraAnchorRootTransform = null;
				mainCamera.Dispossess();
				break;

			case Player.PlayerEvent.Possess:
					GameObject playerParentGO = Singletons.player.charRootTransform .gameObject;
				head = MiscUtils.FindChildInHierarchy( playerParentGO, "Head").GetComponent<Head>();
				cameraAnchorRootTransform = MiscUtils.FindChildInHierarchy( playerParentGO, "CameraAnchor_TwoPerspectives").transform;
				mainCamera.InitializeForNewCharacter(Singletons.player.charRootTransform);
				if ( state == State.FirstPerson )
					mainCamera.HideHead();
			break;
		}
	}
	
		
		
	void OnSwooshedToCharacter()
	{
		stateTime = 0;
		state = State.ThirdPerson;

		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.SwooshedToCharacter );
	}



	void OnZoomedIntoFirstPerson()
	{
		stateTime = 0;
		state = State.FirstPerson;
		mainCamera.HideHead();
		
		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.ZoomedIntoFirstPerson );
	}
	
	
	
	void OnZoomedIntoThirdPerson()
	{
		stateTime = 0;
		state = State.ThirdPerson;
		
		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.ZoomedIntoThirdPerson );
	}
	
	
	
	void SaveOriginalPositionsAndRotations()
	{
		originalCameraPivotPosition = cameraMainPivotTransform.position;
		originalCameraPivotRotation = cameraMainPivotTransform.rotation;
		originalCameraVerticalPivotLocalRotation = cameraVerticalPivotTransform.localRotation;
		originalLocalPosition = ownTransform.localPosition;
		originalLocalRotation = ownTransform.localRotation;
	}
	
	

	public void SwooshToCharacter( Transform cameraAnchorRootTransform )
	{
		swooshSound.Play();

		stateTime = 0;
		state = State.SwooshingToCharacter;

		Go.to ( cameraMainPivotTransform, kSwooshDuration, new GoTweenConfig().
																position(cameraAnchorRootTransform.position).
																rotation(cameraAnchorRootTransform.rotation).
																setEaseType( GoEaseType.SineInOut).
																onComplete( t => OnSwooshedToCharacter()));

			Vector3 targetLocalPosition = thirdPersonOffset;
				if ( is3rdPersonOffsetLeft )
					targetLocalPosition.x = -thirdPersonOffset.x;
		Go.to( ownTransform, kSwooshDuration, new GoTweenConfig().
														localPosition( targetLocalPosition ).
														setEaseType(GoEaseType.SineInOut));
		
		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.SwooshToCharacter );
	}



	void Update()
	{
			if ( state == State.SwooshingToCharacter )
				return;

			float deltaTime = Time.deltaTime;
		stateTime += deltaTime;

		ControlCameraVR( deltaTime );
	}

	
	

	public void ZoomIntoFirstPerson()
	{
		stateTime = 0;
		state = State.ZoomingIntoFirstPerson;
		SaveOriginalPositionsAndRotations ();
		Go.to ( this, kZoomDuration, new GoTweenConfig().
														floatProp ( "zoom", 0 ).
														setEaseType( GoEaseType.Linear).
														onComplete( t => OnZoomedIntoFirstPerson()));
		
		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.ZoomToFirstPerson );
	}
	
	
	
	public void ZoomIntoThirdPerson()
	{
		stateTime = 0;
		state = State.ZoomingIntoThirdPerson;
		SaveOriginalPositionsAndRotations ();
		Go.to ( this, kZoomDuration, new GoTweenConfig().
														floatProp ( "zoom", 1 ).
		       											setEaseType( GoEaseType.Linear).
														onComplete( t => OnZoomedIntoThirdPerson()));
		mainCamera.ShowHead();
		
		if ( OnPerspectiveChange != null )
			OnPerspectiveChange( PerspectiveEvent.ZoomToThirdPerson );
	}
	
}
