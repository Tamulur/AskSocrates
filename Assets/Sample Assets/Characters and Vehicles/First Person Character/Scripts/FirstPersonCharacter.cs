﻿using UnityEngine;

public class FirstPersonCharacter : MonoBehaviour
{
	[SerializeField] private float runSpeed = 8f;                                       // The speed at which we want the character to move
	[SerializeField] private float strafeSpeed = 4f;                                    // The speed at which we want the character to be able to strafe
    [SerializeField] private float jumpPower = 5f;                                      // The power behind the characters jump. increase for higher jumps
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
    [SerializeField] private bool walkByDefault = true;									// controls how the walk/run modifier key behaves.
	[SerializeField] private float walkSpeed = 3f;                                      // The speed at which we want the character to move
#endif
    [SerializeField] private AdvancedSettings advanced = new AdvancedSettings();        // The container for the advanced settings ( done this way so that the advanced setting are exposed under a foldout

    [System.Serializable]
    public class AdvancedSettings                                                       // The advanced settings
    {
        public float gravityMultiplier = 1f;                                            // Changes the way gravity effect the player ( realistic gravity can look bad for jumping in game )
        public PhysicMaterial zeroFrictionMaterial;                                     // Material used for zero friction simulation
        public PhysicMaterial highFrictionMaterial;                                     // Material used for high friction ( can stop character sliding down slopes )
    }

    private CapsuleCollider capsule;                                                    // The capsule collider for the first person character
    private const float jumpRayLength = 0.7f;                                           // The length of the ray used for testing against the ground when jumping
	public bool grounded { get; private set; }
	private Vector2 input;

    void Awake ()
	{
        // Set up a reference to the capsule collider.
	    capsule = collider as CapsuleCollider;
		grounded = true;
	}

	
	public void FixedUpdate ()
	{
        // Read input
		float h = CrossPlatformInput.GetAxis("Horizontal");
		float v = CrossPlatformInput.GetAxis("Vertical");
		bool jump = CrossPlatformInput.GetButton("Jump");

		input = new Vector2( h, v );

		float speed = runSpeed;

		#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
		// On standalone builds, walk/run speed is modified by a key press.
		// We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
		bool walkOrRun =  Input.GetKey(KeyCode.LeftShift);
		speed = walkByDefault ? (walkOrRun ? runSpeed : walkSpeed) : (walkOrRun ? walkSpeed : runSpeed);
        #endif
		
		// On mobile, it's controlled in analogue fashion by the v input value, and therefore needs no special handling.


        
        // Ground Check:

		// Create a ray that points down from the centre of the character.
		Ray ray = new Ray(transform.position, -transform.up);
		
		// Raycast slightly further than the capsule (as determined by jumpRayLength)
		RaycastHit[] hits = Physics.RaycastAll(ray, capsule.height * jumpRayLength );

	       
        float nearest = Mathf.Infinity;
	
		if (grounded || rigidbody.velocity.y < 0.1f)
		{
			// Default value if nothing is detected:
			grounded = false;
            
            // Check every collider hit by the ray
			for (int i = 0; i < hits.Length; i++)
			{
				// Check it's not a trigger
				if (!hits[i].collider.isTrigger && hits[i].distance < nearest)
				{
					// The character is grounded, and we store the ground angle (calculated from the normal)
					grounded = true;
					nearest = hits[i].distance;
					//Debug.DrawRay(transform.position, groundAngle * transform.forward, Color.green);
				}
			}
		}

		Debug.DrawRay(ray.origin, ray.direction * capsule.height * jumpRayLength, grounded ? Color.green : Color.red );
		

            
            // normalize input if it exceeds 1 in combined length:
		if (input.sqrMagnitude > 1) input.Normalize();

		// Get a vector which is desired move as a world-relative direction, including speeds
		Vector3 desiredMove = transform.forward * input.y * speed + transform.right * input.x * strafeSpeed;

		// preserving current y velocity (for falling, gravity)
		float yv = rigidbody.velocity.y;

		// add jump power
		if (grounded && jump) {
			yv += jumpPower;
			grounded = false;
		}

		// Set the rigidbody's velocity according to the ground angle and desired move
		rigidbody.velocity = desiredMove + Vector3.up * yv;

        // Use low/high friction depending on whether we're moving or not
        if (desiredMove.magnitude > 0 || !grounded)
		{
            collider.material = advanced.zeroFrictionMaterial;
		} else {
			collider.material = advanced.highFrictionMaterial;
		}

		// add extra gravity
        rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));
	}

}
