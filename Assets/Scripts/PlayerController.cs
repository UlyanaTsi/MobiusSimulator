using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float walkSpeed = 6;
	public LayerMask groundedMask;
	
	Vector3 step;
	Vector3 smoothMove;
	Vector3 direction;
	Vector3 finalMove;

	Rigidbody rb;
	
	void Awake() {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update() {
		direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		finalMove = direction * walkSpeed;
		step = Vector3.SmoothDamp(step, finalMove, ref smoothMove, .15f);
	}

	void FixedUpdate() {
		Vector3 lStep = transform.TransformDirection(step) * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + lStep);
	}
}
 