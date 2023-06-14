using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// FlyBehaviour inherits from GenericBehaviour. This class corresponds to the flying behaviour.
public class FlyBehaviour : GenericBehaviour
{
	public string flyButton = "Fly";              // Default fly button.
	public float flySpeed = 4.0f;                 // Default flying speed.
	public float sprintFactor = 2.0f;             // How much sprinting affects fly speed.
	public float flyMaxVerticalAngle = 60f;       // Angle to clamp camera vertical movement when flying.
	public float InitialHeight = 45f;
	public float MaximumHeight = 175f;
	public float RobotUpForce = 10;
	public float HorizantalSpeed = 0.01f;
	public float VerticalSpeed = 100f;
	public float VerticalLimit = 0.15f;
	public float TurnSmoothness = 1f;
	public float HorizantalJoystickSpeed = 0.1f;
	public ParticleSystem[] JetParticles;
	public GameObject MissileWeaponObject;

	private int flyBool;                          // Animator variable related to flying.
	[HideInInspector]
	public bool fly = false;                     // Boolean to determine whether or not the player activated fly mode.
	private CapsuleCollider col;                  // Reference to the player capsulle collider.
	private Rigidbody _rigidbody;
	private bool initialTakeOff = false;


	// Start is always called after any Awake functions.
	void Start()
	{
		// Set up the references.
		flyBool = Animator.StringToHash("Fly");
		col = this.GetComponent<CapsuleCollider>();
		_rigidbody = GetComponent<Rigidbody>();
		// Subscribe this behaviour on the manager.
		behaviourManager.SubscribeBehaviour(this);
		
	}

	public void Fly()
	{

		if (!behaviourManager.IsOverriding()
			&& !behaviourManager.GetTempLockStatus(behaviourManager.GetDefaultBehaviour))
		{
			initialTakeOff = true;
			fly = !fly;
			JetParticle(fly);
			// Force end jump transition.
			behaviourManager.UnlockTempBehaviour(behaviourManager.GetDefaultBehaviour);

			// Obey gravity. It's the law!
			behaviourManager.GetRigidBody.useGravity = !fly;

			// Player is flying.
			if (fly)
			{
				// Register this behaviour.
				behaviourManager.RegisterBehaviour(this.behaviourCode);
			}
			else
			{
				// Set collider direction to vertical.
				col.direction = 1;
				// Set camera default offset.
				//behaviourManager.GetCamScript.ResetTargetOffsets();

				// Unregister this behaviour and set current behaviour to the default one.
				behaviourManager.UnregisterBehaviour(this.behaviourCode);
			}
		}
	}

	// Update is used to set features regardless the active behaviour.
	void Update()
	{
		// Toggle fly by input, only if there is no overriding state or temporary transitions.
		if (Input.GetButtonDown(flyButton) && !behaviourManager.IsOverriding()
			&& !behaviourManager.GetTempLockStatus(behaviourManager.GetDefaultBehaviour))
		{
			fly = !fly;

			// Force end jump transition.
			behaviourManager.UnlockTempBehaviour(behaviourManager.GetDefaultBehaviour);

			// Obey gravity. It's the law!
			behaviourManager.GetRigidBody.useGravity = !fly;

			// Player is flying.
			if (fly)
			{
				// Register this behaviour.
				behaviourManager.RegisterBehaviour(this.behaviourCode);
			}
			else
			{
				// Set collider direction to vertical.
				col.direction = 1;
				// Set camera default offset.

				// Unregister this behaviour and set current behaviour to the default one.
				behaviourManager.UnregisterBehaviour(this.behaviourCode);
			}
		}

		// Assert this is the active behaviour
		fly = fly && behaviourManager.IsCurrentBehaviour(this.behaviourCode);

		// Set fly related variables on the Animator Controller.
		behaviourManager.GetAnim.SetBool(flyBool, fly);
	}

	// This function is called when another behaviour overrides the current one.
	public override void OnOverride()
	{
		// Ensure the collider will return to vertical position when behaviour is overriden.
		col.direction = 1;
	}

	// LocalFixedUpdate overrides the virtual function of the base class.
	public override void LocalFixedUpdate()
	{
		Vector3 newpos = transform.position;
		if (CustomInput.RobotDown && !behaviourManager.IsGrounded())
		{			
			newpos.y -= Time.deltaTime * RobotUpForce;
			transform.position = newpos;
			initialTakeOff = false;
		}
		else if (CustomInput.RobotUp)
		{
			if (transform.position.y < MaximumHeight)
			{
				newpos.y += Time.deltaTime * RobotUpForce;
				transform.position = newpos;
				initialTakeOff = false;
			}
		}

		// Set camera limit angle related to fly mode.
		//behaviourManager.GetCamScript.SetMaxVerticalAngle(flyMaxVerticalAngle);
		if (initialTakeOff)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, InitialHeight, transform.position.z), Time.deltaTime);

		// Call the fly manager.
		FlyManagement(behaviourManager.GetH, behaviourManager.GetV);
	}

	// Deal with the player movement when flying.
	Vector3 direction;
	void FlyManagement(float horizontal, float vertical)
	{
		// Add a force player's rigidbody according to the fly direction.
		//if (vertical > VerticalLimit)
		//      {
		//	direction = Rotating(horizontal * HorizantalSpeed, vertical * VerticalSpeed);
		//	behaviourManager.GetRigidBody.AddForce((direction * (behaviourManager.IsSprinting() ? sprintFactor : 1)), ForceMode.Acceleration);
		//}
		if (horizontal != 0 || vertical > 0)
        {
			direction = Rotating(horizontal * HorizantalSpeed, vertical * VerticalSpeed);
			direction.y = 0;
			behaviourManager.GetRigidBody.AddForce((direction * (behaviourManager.IsSprinting() ? sprintFactor : 1)), ForceMode.Acceleration);
		}else if (horizontal == 0 && vertical == 0 && behaviourManager.IsSprinting())
        {
			direction = Rotating(0, VerticalSpeed);
			direction.y = 0;
			behaviourManager.GetRigidBody.AddForce(direction * sprintFactor, ForceMode.Acceleration);
		}
	}

	// Rotate the player to match correct orientation, according to camera and key pressed.
	Vector3 Rotating(float horizontal, float vertical)
	{
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
		// Camera forward Y component is relevant when flying.
		//forward.y = 0.0f;
		forward = forward.normalized;

		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		Vector3 targetDirection;
		// Calculate target direction based on camera forward and direction key.

		if (vertical > 0)
			targetDirection = forward * vertical + right * horizontal;
		else if (vertical < 0)
			targetDirection = -right * horizontal * Time.deltaTime;
		else
			targetDirection = Vector3.zero;

		// Rotate the player to the correct fly position.
		if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
			targetRotation.x = 0; targetRotation.z = 0;
			Quaternion newRotation = Quaternion.Lerp(behaviourManager.GetRigidBody.rotation, targetRotation, Time.deltaTime * TurnSmoothness);
			//newRotation.x = 0; newRotation.z = 0;

			behaviourManager.GetRigidBody.MoveRotation(newRotation);
			behaviourManager.SetLastDirection(targetDirection);
		}
		return targetDirection;
	}

	private void JetParticle(bool start)
	{
		if (start)
		{
			MissileWeaponObject.SetActive(true);
			foreach (ParticleSystem a in JetParticles)
			{
				a.Play();
			}
		}
		else
		{
			MissileWeaponObject.SetActive(false);
			foreach (ParticleSystem a in JetParticles)
			{
				a.Stop();
			}
		}
	}
}
