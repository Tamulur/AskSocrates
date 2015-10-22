using System;
using UnityEngine;


public class Player : MonoBehaviour
{
	
	#region fields
		
		public Animator animator { get; private set; }
		public Transform charRootTransform { get; private set; }
		public Head head { get; private set; }
		public MainCameraBase playerCamera { get; set; }

		const float kTimeWarpChangeDuration = 0.5f;
		const float kBulletTime = 0.025f;
		const float kChangeCharacterDuration = 0.2f;
	
		CameraControlTwoPerspectives cameraControl;
		HoloGrid darkness;

		readonly Transform[] characters = new Transform[2];
		int characterIndex;

		AnimatorIKHandler animatorIKHandler;
		HandsController handsController;

		Notepad notepad;
		
		Transform cameraTransform;
		Transform cameraPivotTransform;

		public enum PlayerEvent
		{
			Dispossess,
			Possess
		}
		public delegate void PlayerCallback ( PlayerEvent playerEvent );
		public PlayerCallback OnPlayerEvent;

		enum State {
			InCharacter,
			ZoomingOutOfCharacter,
			ChangingCharacter,
			ZoomingIntoCharacter
		}
		State state = State.ZoomingIntoCharacter;

	
	#endregion
	
	
	

	void Awake()
	{
		characters[0] = GameObject.Find("Carl").transform;
		characters[1] = GameObject.Find("Socrates").transform;

			GameObject cameraPivotGO = GameObject.Find("CameraPivot_TwoPerspectives");
		cameraPivotTransform = cameraPivotGO.transform;

		cameraControl = cameraPivotTransform.GetComponentInChildren<CameraControlTwoPerspectives>();
			cameraControl.OnPerspectiveChange += OnPerspectiveChange;

		cameraTransform = cameraControl.transform.Find("OVRCameraRig");
		playerCamera = cameraTransform.GetComponent<MainCameraBase> ();
		darkness = GameObject.Find("OVRCameraRig").transform.Find("TrackingSpace/CenterEyeAnchor/Darkness").GetComponent<HoloGrid>();
	}

	


	public void Dispossess()
	{
		if ( OnPlayerEvent != null )
			OnPlayerEvent ( PlayerEvent.Dispossess );

		charRootTransform.GetComponentInChildren<AI>().enabled = true;
		notepad = null;
		
		head.Dispossess();
			head = null;
		
		handsController.holdNotepad = false;
			handsController = null;
			
		Destroy(animatorIKHandler);
	}
	
	

	public void FadeFromDark( Action onComplete, float duration=2 )
	{
		darkness.SetUseTransparentShader( true );

		Singletons.soundManager.TweenVolumeTo(1, duration);
		darkness.TweenTo( 0, duration: duration, onComplete: () => {	darkness.Deactivate();
																										onComplete(); });
	}



	public void FadeToDark( Action onComplete )
	{
		darkness.SetUseTransparentShader( true );

				const float kDuration = 2;
		Singletons.soundManager.TweenVolumeTo(0, kDuration);
		darkness.TweenTo( 1, duration: kDuration, onComplete: () => {	darkness.SetUseTransparentShader( false );
																										onComplete(); });
	}



	public void InitializeForNewCharacter( int characterIndex )
	{
		this.characterIndex = characterIndex;
		charRootTransform = characters[ characterIndex ];
		charRootTransform.GetComponentInChildren<AI>().enabled = false;
		animator = charRootTransform.GetComponentInChildren<Animator>();
		animatorIKHandler = animator.gameObject.AddComponent<AnimatorIKHandler>();

		handsController = charRootTransform.GetComponentInChildren<HandsController>();
			handsController.holdNotepad = true;
			
		head = charRootTransform.GetComponentInChildren<Head>();
			head.Possess();
		
		notepad = charRootTransform.Find("Notepad").GetComponent<Notepad>();
			notepad.text = "";
		
		if ( OnPlayerEvent != null )
			OnPlayerEvent ( PlayerEvent.Possess );
	}
	


	void OnPerspectiveChange(CameraControlTwoPerspectives.PerspectiveEvent perspectiveEvent)
	{
			bool finishedIntoFirst = perspectiveEvent == CameraControlTwoPerspectives.PerspectiveEvent.ZoomedIntoFirstPerson;
			bool finishedIntoThird = perspectiveEvent == CameraControlTwoPerspectives.PerspectiveEvent.ZoomedIntoThirdPerson;
			bool changedCharacter = perspectiveEvent == CameraControlTwoPerspectives.PerspectiveEvent.SwooshedToCharacter;

		if ( state == State.ZoomingOutOfCharacter && finishedIntoThird )
		{
				Transform otherCharacterCameraAnchor = characters[ characterIndex == 0 ? 1 : 0 ].GetComponentInChildren<CameraAnchor>().transform;
			SwooshToCharacter( otherCharacterCameraAnchor );
		}
		else if ( state == State.ChangingCharacter && changedCharacter )
		{
			InitializeForNewCharacter( characterIndex == 0 ? 1 : 0 );
			ZoomIntoCharacter();
		}
		else if ( state == State.ZoomingIntoCharacter && finishedIntoFirst )
		{
			notepad.inputEnabled = true;
			state = State.InCharacter;
		}
	}



	public void SetToDark()
	{
		darkness.Activate( 1 );
		darkness.SetUseTransparentShader( false );
		Singletons.soundManager.volume = 0;
	}	



	void Start()
	{
		InitializeForNewCharacter( 0 );

		ZoomIntoCharacter();
	}
	


	void SwooshToCharacter( Transform otherCharacterCameraAnchor )
	{
		Dispossess();
		state = State.ChangingCharacter;

			bool swooshingToSocrates = characterIndex == 0;
		cameraControl.is3rdPersonOffsetLeft = swooshingToSocrates;
		cameraControl.SwooshToCharacter( otherCharacterCameraAnchor );
	}



	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Singletons.textManager.AddEntry( notepad.text );
			Application.Quit();
		}
		else if ( Input.GetKeyDown(KeyCode.Return) && cameraControl.CanSwitchCharacter() && notepad.text.Length > 0 )
			ZoomOutOfCharacter();
	}



	void ZoomIntoCharacter()
	{
		state = State.ZoomingIntoCharacter;
		cameraControl.ZoomIntoFirstPerson();
	}



	void ZoomOutOfCharacter()
	{
		Singletons.textManager.AddEntry( notepad.text );
		notepad.inputEnabled = false;
		state = State.ZoomingOutOfCharacter;
		cameraControl.ZoomIntoThirdPerson();
	}


}
