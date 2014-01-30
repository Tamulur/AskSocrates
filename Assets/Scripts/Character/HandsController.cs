using UnityEngine;

public class HandsController : MonoBehaviour
{

	#region fields
		
			bool _holdNotepad;
		public bool holdNotepad {
			get { return _holdNotepad; }
			set {
				_holdNotepad = value;
				notebookTransform.gameObject.SetActive ( _holdNotepad );
		}
		}
		
		Transform notebookTransform;
		Transform leftHandNotebookTargetTransform;
		Transform leftHandSofaTargetTransform;
		Transform rightHandTargetTransform;
		Animator animator;
	#endregion



	void Awake()
	{
		animator = GetComponent<Animator>();
		notebookTransform = transform.parent.Find("Notepad");
		leftHandNotebookTargetTransform = notebookTransform.Find("HandTarget");
		leftHandSofaTargetTransform = transform.parent.Find("LeftHandIKTarget");
		rightHandTargetTransform = transform.parent.Find("RightHandIKTarget");
	}
	


	void OnAnimatorIK()
	{
		UpdateHand( AvatarIKGoal.LeftHand, holdNotepad ?  leftHandNotebookTargetTransform
																				:	leftHandSofaTargetTransform );
		
		UpdateHand( AvatarIKGoal.RightHand, rightHandTargetTransform );
	}



	void UpdateHand(AvatarIKGoal goal, Transform targetTransform)
	{
		animator.SetIKPositionWeight(goal, 1);
		animator.SetIKPosition(goal, targetTransform.position);
		
		animator.SetIKRotationWeight(goal, 1);
		animator.SetIKRotation(goal, targetTransform.rotation);
	}

}
