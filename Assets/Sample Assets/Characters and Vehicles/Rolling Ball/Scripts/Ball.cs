using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] private float movePower = 5;               // The force added to the ball to move it.
    [SerializeField] private bool useTorque = true;             // Whether or not to use torque to move the ball.
    [SerializeField] private float maxAngularVelocity = 25;     // The maximum velocity the ball can rotate at.
    [SerializeField] private float jumpPower = 2;               // The force added to the ball when it jumps.
	
    
    private const float GroundRayLength = 1f;                   // The length of the ray to check if the ball is grounded.


    void Start()
	{
        // Set the maximum angular velocity.
		rigidbody.maxAngularVelocity = maxAngularVelocity;
	}


	public void Move (Vector3 move, bool jump)
    {
        // Set the move direction to be relative to the camera.
	    var moveDirection = Camera.main.transform.TransformDirection( move ).normalized;
	
        // If using torque to rotate the ball...
		if (useTorque) 
            // ... add torque around the axis defined by the move direction.
            rigidbody.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x) * movePower);
		else
            // Otherwise add force in the move direction.
			rigidbody.AddForce( moveDirection * movePower );

        // If on the ground and jump is pressed...
        if (Physics.Raycast(transform.position, -Vector3.up, GroundRayLength) && jump)
        {
            // ... add force in upwards.
            rigidbody.AddForce(Vector3.up*jumpPower, ForceMode.Impulse);
        }
	}
}
