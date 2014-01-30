using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
	#region fields
		
		public Transform targetBodyTransform;
		public Transform[] pointsOfInterest;
		
		const float kBodyWeight = 0.1f;
	
		Transform ownEyeLeftTransform;
		Transform ownEyeRightTransform;
		Transform eyesRootTransform;

		Head targetHead;

		CameraControlTwoPerspectives cameraControlTwoPerspectives;
		Animator anim;
		Vector3 eyeLookTarget;
		Vector3 headLookTarget;
		Vector3 eyeLookDirection;
		bool isSocrates;
		bool targetIsPlayer;
		Transform lookTargetTransform;

		float bodyWeight;
		float ikWeight;
		float currentTargetLookRestTime;
		float playerLookingAtMeTime;
		float lookAtPlayerDeadTime;

	#endregion



	void Awake()
	{
		anim = GetComponent<Animator>();

		//*** Eyes
		{
			GameObject leftEyeGO = MiscUtils.FindChildInHierarchy(gameObject, "EyeLeft");
			if (leftEyeGO)
			{
				ownEyeLeftTransform = leftEyeGO.transform;
				ownEyeRightTransform = MiscUtils.FindChildInHierarchy(gameObject, "EyeRight").transform;

				// The eyesRootTransform will be the reference transform to move the eyes in.
				// We could use the head bone, but that might have a weird orientation
				eyesRootTransform = new GameObject(name + "_eyesRoot").transform;
					eyesRootTransform.position = 0.5f * (ownEyeLeftTransform.position + ownEyeRightTransform.position);
					eyesRootTransform.rotation = ownEyeLeftTransform.rotation;
					eyesRootTransform.parent = ownEyeLeftTransform.parent;
				ownEyeLeftTransform.parent = ownEyeRightTransform.parent = eyesRootTransform;
			}
		}

		cameraControlTwoPerspectives = MiscUtils.GetComponentSafely<CameraControlTwoPerspectives>("CameraControls");

		targetHead = targetBodyTransform.GetComponentInChildren<Head>();
		isSocrates = transform.parent.name == "Socrates";
	}


	
	float GetEyeTurnSpeed()
	{
		return 20;
	}
	
	
	
	float GetHeadTurnSpeed()
	{
		return targetIsPlayer ? 1 : 0.25f;
	}
	
	
	
	public Vector3 GetOwnEyeCenterPosition()
	{
		return ownEyeLeftTransform != null ? eyesRootTransform.position
															: transform.position;
	}



	Vector3 GetOwnLookDirection()
	{
		return ownEyeLeftTransform != null ? 0.5f * (ownEyeLeftTransform.forward + ownEyeRightTransform.forward)
															: transform.forward;
	}



	public float GetStareAngleMeAtTarget()
	{
		return Vector3.Angle(GetOwnLookDirection(), targetHead.eyeCenter - GetOwnEyeCenterPosition());
	}



	public float GetStareAngleTargetAtMe()
	{
		return Vector3.Angle(targetHead.lookDirection, GetOwnEyeCenterPosition() - targetHead.eyeCenter);
	}



	void OnAnimatorIK()
	{
			Vector3 localPos = transform.InverseTransformPoint(headLookTarget);
		anim.SetLookAtPosition(transform.TransformPoint(localPos / transform.localScale.x));

			bodyWeight = Mathf.Lerp(bodyWeight, kBodyWeight, Time.deltaTime * 1);
			ikWeight = Mathf.Lerp(ikWeight, isSocrates ? 1 : 0.7f, Time.deltaTime);
		anim.SetLookAtWeight(ikWeight, bodyWeight, 0.63f);
	}



	void OnEnable()
	{
		StartLookingAtSomePOI();
	}



	void StartLookingAtPlayer(float playerLookingAtMeTime01)
	{
		targetIsPlayer = true;
		currentTargetLookRestTime = (isSocrates ? 10 : 5) + Random.value * playerLookingAtMeTime01 * (isSocrates ? 20: 5);
		lookAtPlayerDeadTime = currentTargetLookRestTime + (isSocrates ? 50 : 100) + Random.value * 30;
	}



	void StartLookingAtSomePOI()
	{
		targetIsPlayer = false;
		currentTargetLookRestTime = 20 + Random.value * 60;
			int poiIndex = Random.Range(0, pointsOfInterest.Length);
		lookTargetTransform = pointsOfInterest[ poiIndex ];
	}



	void Update()
	{
		//*** Look back at player?
		{
			if ( lookAtPlayerDeadTime > 0 )
				lookAtPlayerDeadTime -= Time.deltaTime;

			if ( lookAtPlayerDeadTime <= 0 )
			{
						float playerLookingAtMeAngle = GetStareAngleTargetAtMe();
						bool isPlayerLookingAtMe = playerLookingAtMeAngle < 30;
					playerLookingAtMeTime = isPlayerLookingAtMe		? Mathf.Min(10, playerLookingAtMeTime + Mathf.Cos(Mathf.Deg2Rad * playerLookingAtMeAngle) * Time.deltaTime)
																							: Mathf.Max(0, playerLookingAtMeTime - Time.deltaTime);
				if ( playerLookingAtMeTime > 5 )
				{
							float lookTime01 = (Mathf.Min(10, playerLookingAtMeTime) - 5) / 5.0f;
						bool lookBack = Random.value > lookTime01 * ( isSocrates ? 0.05f : 0.001f );
					if ( lookBack )
						StartLookingAtPlayer(lookTime01);
				}
			}
		}

		//*** Finished looking at current target?
		{
				currentTargetLookRestTime -= Time.deltaTime;
			if ( currentTargetLookRestTime <= 0 )
				StartLookingAtSomePOI();
		}

		//*** Update head and eye targets depending on current look target
		{
				Vector3 lookTargetPos = targetIsPlayer	? cameraControlTwoPerspectives.GetLookTarget()
																			: lookTargetTransform.position;
			headLookTarget = Vector3.Lerp(headLookTarget, lookTargetPos, Time.deltaTime * GetHeadTurnSpeed());
			eyeLookTarget = Vector3.Lerp(eyeLookTarget, lookTargetPos, Time.deltaTime * GetEyeTurnSpeed());
		}

		UpdateEyesToLookAtTarget();
	}



	void UpdateEyesToLookAtTarget()
	{
			Vector3 leftLookAtLocalEuler = (Quaternion.Inverse(eyesRootTransform.rotation) * Quaternion.LookRotation(eyeLookTarget - ownEyeLeftTransform.position, eyesRootTransform.up)).eulerAngles;
			Vector3 normalizedLocalEuler = new Vector3(MiscUtils.NormalizedDegAngle(leftLookAtLocalEuler.x),
																				MiscUtils.NormalizedDegAngle(leftLookAtLocalEuler.y),
																				MiscUtils.NormalizedDegAngle(leftLookAtLocalEuler.z));
			Vector3 clampedLocalEuler = new Vector3(Mathf.Clamp(normalizedLocalEuler.x, -10, 10),
																			Mathf.Clamp(normalizedLocalEuler.y, -25, 25),
																			Mathf.Clamp(normalizedLocalEuler.z, -10, 10));
			ownEyeLeftTransform.localRotation = Quaternion.Euler(clampedLocalEuler);

			Vector3 rightLookAtLocalEuler = (Quaternion.Inverse(eyesRootTransform.rotation) * Quaternion.LookRotation(eyeLookTarget - ownEyeRightTransform.position, eyesRootTransform.up)).eulerAngles;
			normalizedLocalEuler = new Vector3(MiscUtils.NormalizedDegAngle(rightLookAtLocalEuler.x),
																	MiscUtils.NormalizedDegAngle(rightLookAtLocalEuler.y),
																	MiscUtils.NormalizedDegAngle(rightLookAtLocalEuler.z));
			clampedLocalEuler = new Vector3(Mathf.Clamp(normalizedLocalEuler.x, -10, 10),
																Mathf.Clamp(normalizedLocalEuler.y, -25, 25),
																Mathf.Clamp(normalizedLocalEuler.z, -10, 10));
			ownEyeRightTransform.localRotation = Quaternion.Euler(clampedLocalEuler);
	}



}
