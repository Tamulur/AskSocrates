using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour
{
	#region fields
		public bool useNeckBone = true;
	
		public float avatarScale { get; private set; }
	
		public Vector3 initialHeadPos { get; private set; }
	
		public Vector3 initialNeckPos { get; private set; }
	
		public Vector3 initialLeftShoulderPos { get; private set; }
		public Vector3 initialRightShoulderPos { get; private set; }
	
		public Vector3 initialLeftEllbowPos { get; private set; }
		public Vector3 initialRightEllbowPos { get; private set; }
	
		public Vector3 initialLeftHandPos { get; private set; }
		public Vector3 initialRightHandPos { get; private set; }
	
		Animator animator;
	#endregion
	
	
	void Awake()
	{
		animator = MiscUtils.FindChildInHierarchy( transform.parent .gameObject, "PlayerModel").GetComponent<Animator>();
		
		//*** Store the initial local bone positions before the animation puts the avatar out of T-pose
		{
			Transform trans = transform.parent;
			
			initialHeadPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.Head ).position );
			
				if ( useNeckBone )
			initialNeckPos = trans.InverseTransformPoint(animator.GetBoneTransform(HumanBodyBones.Neck).position);
			
			initialLeftShoulderPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.LeftUpperArm ).position );
			initialRightShoulderPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.RightUpperArm ).position );
			
			initialLeftEllbowPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.LeftLowerArm ).position );
			initialRightEllbowPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.RightLowerArm ).position );
			
			initialLeftHandPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.LeftHand ).position );
			initialRightHandPos = trans.InverseTransformPoint( animator.GetBoneTransform( HumanBodyBones.RightHand ).position );
			
			Vector3 averageFootPosition = 0.5f * (animator.GetBoneTransform( HumanBodyBones.LeftFoot ).position +
												animator.GetBoneTransform( HumanBodyBones.RightFoot ).position );
			avatarScale = animator.GetBoneTransform( HumanBodyBones.Head ).position.y - averageFootPosition.y;
		}
	}
	
	
	
	public Transform GetAnchorTransform()
	{
		return animator.GetBoneTransform( useNeckBone	? HumanBodyBones.Neck
														: HumanBodyBones.Head );
	}
	
	
	
	public Vector3 GetAverageInitialShoulderPos()
	{
		return 0.5f * (initialLeftShoulderPos + initialRightShoulderPos);
	}
	
		
}
