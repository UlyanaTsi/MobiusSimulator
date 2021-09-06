using UnityEngine;

public class MovingSphereSnap : MonoBehaviour {
	[SerializeField] Transform playerInputSpace = default;

	[SerializeField, Range(0f, 100f)] 	float maxSpeed = 10f;
	[SerializeField, Range(0f, 100f)] 	float maxAcceleration = 10f;
	[SerializeField, Range(0f, 10f)] 	float jumpHeight = 3f;
	//[SerializeField, Range(0, 5)] 	int maxAirJumps = 0;
	//[SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f, maxStairsAngle = 50f;
	//[SerializeField, Range(0f, 180f)]    float maxClimbAngle = 180f;
	//[SerializeField, Range(0f, 100f)] 	float maxSnapSpeed = 100f;
	[SerializeField, Min(0f)] 	float probeDistance = 1f;
	//[SerializeField, Min(0f)] float probeDistanceInitial = 10f;
	[SerializeField] LayerMask probeMask = -1;//, stairsMask = -1, climbMask = -1;
											  //[SerializeField] Material normalMaterial = default, climbingMaterial = default;
	[SerializeField] float gravity = 10f;

	[SerializeField] Vector3 InitialDown = new Vector3(0f,-1f,0f);

	[SerializeField] float playerHeight = 0.5f;

	public static Vector3 upAxis;

	public Vector3 Test;

	Vector3 rightAxis, forwardAxis;


	Vector3 velocity, desiredVelocity;
	Vector3 contactNormal;//, steepNormal, climbNormal;

	Rigidbody body;
	bool desiredJump;
	int groundContactCount; //, steepContactCount, climbContactCount;
	bool OnGround => groundContactCount > 0;
	//bool OnSteep => steepContactCount > 0;

	//bool Climbing => climbContactCount > 0;
	//int jumpPhase;
	//float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

	//int stepsSinceLastGrounded, stepsSinceLastJump;

	MeshRenderer meshRenderer;



	
	void Awake()
	{
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		meshRenderer = GetComponent<MeshRenderer>();
		InitializePosition();
	}

	void Update()
	{
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		if (playerInputSpace)
		{
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
		}
		else
		{
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}
		desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		desiredJump |= Input.GetButtonDown("Jump");

		//meshRenderer.material = Climbing ? climbingMaterial : normalMaterial;

		// GetComponent<Renderer>().material.SetColor("_Color", Color.white * (groundContactCount * 0.5f) );
		// GetComponent<Renderer>().material.SetColor( "_Color", OnGround ? Color.black : Color.white 	);
	}

	void FixedUpdate()
	{
		UpdateState();
		AdjustVelocity();
		if (desiredJump)
		{
			desiredJump = false;
			Jump();
		}
		AdjustVerticalComponent();

		body.velocity = velocity;
		ClearState();
	}

	void InitializePosition()
    {
		upAxis = -InitialDown;
		AdjustVerticalComponent();
   	}

	void AdjustVerticalComponent()
    {
		if (Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask))
		{
			//upAxis = hit.normal;
			upAxis = 0.9f *upAxis + 0.1f * hit.normal;
			if (hit.distance>playerHeight)
            {
				velocity -= upAxis * gravity * Time.deltaTime;
			}
			//transform.position = hit.point + playerHeight * hit.normal;
			//return true;
		} 
	}

	void ClearState()
	{
		contactNormal = Vector3.zero;
		groundContactCount = 0;
	}

	void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void EvaluateCollision(Collision collision) 
	{
		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			contactNormal += normal;
			groundContactCount += 1;
		}
	}

	Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

	void AdjustVelocity()
	{
		Vector3 xAxis, zAxis;
		xAxis = rightAxis;
		zAxis = forwardAxis;
		
		xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
		zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

		float currentX = Vector3.Dot(velocity, xAxis);
		float currentZ = Vector3.Dot(velocity, zAxis);

		float acceleration = maxAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}

	void UpdateState()
	{
		velocity = body.velocity;
		/*
		if (SnapToGround())
		{
			contactNormal.Normalize();
		}
		else
		{
			contactNormal = upAxis;
		}
		*/
	}

	void Jump()
	{
		Vector3 jumpDirection;
		if (OnGround)
		{
			jumpDirection = contactNormal;
		}
		else
		{
			return;
		}
		float jumpSpeed = Mathf.Sqrt(2f * gravity * jumpHeight);
		jumpDirection = (jumpDirection + upAxis).normalized;
		float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
		if (alignedSpeed > 0f)
		{
			jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
		}
		velocity += jumpDirection * jumpSpeed;
	}

	

	/*
	bool SnapToGround()
	{
		float speed = velocity.magnitude;
		if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask))
		{
			return false;
		}
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f)
		{
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		upAxis = hit.normal;
		Test = upAxis;
		return true;
	}
	*/

	/*
	bool CheckClimbing()
	{
		if (Climbing)
		{
			groundContactCount = climbContactCount;
			contactNormal = climbNormal;
			return true;
		}
		return false;
	}
	*/
}
