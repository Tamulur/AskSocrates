using UnityEngine;

public class BallUserControl : MonoBehaviour
{
    private Ball ball;            // Reference to the ball controller.
    private Vector3 move;                   // The movement vector defined by the axis input.
    private bool jump;                      // The jump button.

	
	void Awake ()
	{
        // Set up the reference.
	    ball = GetComponent<Ball>();
	}
	
	
	void Update ()
    {
        // Get the axis and jump input.
        move = new Vector3(CrossPlatformInput.GetAxis("Horizontal"), 0f, CrossPlatformInput.GetAxis("Vertical"));
	    jump = CrossPlatformInput.GetButton("Jump");
    }


    void FixedUpdate ()
    {
        // Call the Move function of the ball controller 
        ball.Move(move, jump);
    }
}
