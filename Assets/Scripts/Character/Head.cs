using UnityEngine;


public class Head : MonoBehaviour
{

	#region fields

		public Quaternion lookRotation { get { return eyeCenterTransform.rotation; }}
		public Quaternion averageRotation { get; private set; }
		public Transform eyeCenterTransform { get; private set; }
		public Vector3 neckCorrectionEuler;
		public Vector3 lookDirection  { get; private set; }
		public Vector3 eyeCenter { get; private set; }
		public bool followOVRCameras { get; set; }
		public float followOVRCameraFactor { get; set; }

		Transform anchorBoneTransform;
		
		int originalLayer;

		Quaternion headCorrectionRot;
		Transform ovrXform;
		GameObject eyeLeftGO;
		GameObject eyeRightGO;

	#endregion



	void Awake ()
	{
		ovrXform = FindObjectOfType<OVRCameraRig>().transform;
		eyeCenterTransform = GameObject.Find("CenterEyeAnchor").transform;
		eyeLeftGO = MiscUtils.FindChildInHierarchy(transform.parent.gameObject, "EyeLeft");
		eyeRightGO = MiscUtils.FindChildInHierarchy(transform.parent.gameObject, "EyeRight");
	}
	


	public void Dispossess()
	{
		gameObject.layer = originalLayer;
		if ( eyeLeftGO != null )
			eyeLeftGO.layer = eyeRightGO.layer = gameObject.layer;
		enabled = false;
		followOVRCameras = false;
	}



	public void Possess()
	{
		originalLayer = gameObject.layer;
		gameObject.layer = (int) Layers.Layer.PlayerHead;
		if ( eyeLeftGO != null )
			eyeLeftGO.layer = eyeRightGO.layer = gameObject.layer;

		enabled = true;
		followOVRCameras = true;
		followOVRCameraFactor = 1;
	}



	void LateUpdate()
	{
		lookDirection = eyeCenterTransform.forward;
		eyeCenter = eyeCenterTransform.position;

		if ( followOVRCameras )
			anchorBoneTransform.rotation = transform.rotation * Quaternion.Inverse( ovrXform.rotation ) * lookRotation * headCorrectionRot;
		
	}


	
	void Start()
	{
			CameraAnchor cameraAnchor = transform.parent.parent.Find ( "CameraAnchor_TwoPerspectives").GetComponent<CameraAnchor>();
		anchorBoneTransform = cameraAnchor.GetAnchorTransform();
		headCorrectionRot = Quaternion.Inverse(Quaternion.LookRotation(transform.forward)) * anchorBoneTransform.rotation;
	}
	
	
}
