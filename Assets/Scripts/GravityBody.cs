using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour {
	GravityAttractor strip;
	Rigidbody rb;

	void Awake(){
		strip = GameObject.FindGameObjectWithTag("Sphere").GetComponent<GravityAttractor>();
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate(){
		strip.Attract(rb);
	}
}
