using UnityEngine;


public class AnimatorIKHandler : MonoBehaviour
{

	#region fields

		public delegate void AnimatorIKHandlerCallback();
		public AnimatorIKHandlerCallback OnAnimatorIKEvent;

	#endregion



	void OnAnimatorIK()
	{
		if ( OnAnimatorIKEvent != null )
			OnAnimatorIKEvent();
	}

}
