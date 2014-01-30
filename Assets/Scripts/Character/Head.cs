using UnityEngine;


public class Head : MonoBehaviour
{

	#region fields

		public Quaternion averageRotation { get; private set; }
		public Vector3 neckCorrectionEuler;
		public Vector3 lookDirection  { get; private set; }
		public Vector3 eyeCenter { get; private set; }
		public bool followOVRCameras { get; set; }
		public float followOVRCameraFactor { get; set; }

		Transform ovrControllerTransform;
		Transform leftCameraTransform;
		Transform rightCameraTransform;
		Transform anchorBoneTransform;
		
		Transform lookTargetEyeLeftTransform;
		Transform lookTargetEyeRightTransform;
		Transform lookTargetHeadTransform;

		int originalLayer;

		OVRCameraController ovrCameraController;

	#endregion



	void Awake ()
	{
		//*** Head movement
		{
			ovrCameraController = MiscUtils.GetComponentSafely<OVRCameraController>("OVRCameraController");
			ovrControllerTransform = ovrCameraController.transform;
			leftCameraTransform = ovrControllerTransform.Find("CameraLeft");
			rightCameraTransform = ovrControllerTransform.Find("CameraRight");
		}

		//*** Look target
		{
				GameObject leftEyeGO = MiscUtils.FindChildInHierarchy( transform.parent.gameObject, "EyeLeft" );
			if ( leftEyeGO )
			{
				lookTargetEyeLeftTransform = leftEyeGO.transform;
				lookTargetEyeRightTransform = MiscUtils.FindChildInHierarchy( transform.parent.gameObject, "EyeRight" ) .transform;
			}
			else
				lookTargetHeadTransform = transform.parent.GetComponent<Animator>() .GetBoneTransform ( HumanBodyBones.Head );
		}
	}
	


	public void Dispossess()
	{
		gameObject.layer = originalLayer;
		enabled = false;
		followOVRCameras = false;
	}



	public void Possess()
	{
		originalLayer = gameObject.layer;
		gameObject.layer = (int) Layers.Layer.PlayerHead;

		enabled = true;
		followOVRCameras = true;
		followOVRCameraFactor = 1;
	}



	void LateUpdate()
	{
		if ( followOVRCameras )
		{
				averageRotation = Quaternion.Slerp( leftCameraTransform.rotation, rightCameraTransform.rotation, 0.5f );
				Quaternion targetRotation = averageRotation * Quaternion.Euler (neckCorrectionEuler);
			anchorBoneTransform.rotation = Quaternion.Slerp( anchorBoneTransform.rotation, targetRotation, followOVRCameraFactor );
		}
		
		//*** lookDirection and eyeCenter
		{
			if (lookTargetHeadTransform)
				lookDirection = lookTargetHeadTransform.forward;
			else
				lookDirection = 0.5f * (lookTargetEyeLeftTransform.forward + lookTargetEyeRightTransform.forward);

			if (lookTargetHeadTransform)
				eyeCenter = lookTargetHeadTransform.position;
			else
				eyeCenter = 0.5f * (lookTargetEyeLeftTransform.position + lookTargetEyeRightTransform.position);
		}
	}


	
	void Start()
	{
			CameraAnchor cameraAnchor = transform.parent.parent.Find ( "CameraAnchor_TwoPerspectives").GetComponent<CameraAnchor>();
		anchorBoneTransform = cameraAnchor.GetAnchorTransform();
	}
	
	
}
