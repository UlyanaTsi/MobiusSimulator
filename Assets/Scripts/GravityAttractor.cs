using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour {
	public float gravitySpeed;
	public float gravity = -10f;

	public void Attract(Rigidbody rb) {
        float distForward = Mathf.Infinity;
        RaycastHit hitForward;
        if (Physics.SphereCast(rb.transform.position, 0.25f, -rb.transform.up + rb.transform.forward, out hitForward, 5)) 
            distForward = hitForward.distance;
        
        float distDown = Mathf.Infinity;
        RaycastHit hitDown;
        if (Physics.SphereCast(rb.transform.position, 0.25f, -rb.transform.up, out hitDown, 5))
            distDown = hitDown.distance;

        float distBack = Mathf.Infinity;
        RaycastHit hitBack;
        if (Physics.SphereCast(rb.transform.position, 0.25f, -rb.transform.up + -rb.transform.forward, out hitBack, 5))
            distBack = hitBack.distance;

        if (distForward < distDown && distForward < distBack){
            rb.rotation = Quaternion.Lerp(rb.transform.rotation,
                Quaternion.LookRotation(Vector3.Cross(rb.transform.right, hitForward.normal), hitForward.normal), Time.deltaTime * 5.0f);
        } else if (distDown < distForward && distDown < distBack) {
            rb.rotation = Quaternion.Lerp(rb.transform.rotation,
                Quaternion.LookRotation(Vector3.Cross(rb.transform.right, hitDown.normal), hitDown.normal), Time.deltaTime * 5.0f);
        } else if (distBack < distForward && distBack < distDown) {
            rb.rotation = Quaternion.Lerp(rb.transform.rotation,
                Quaternion.LookRotation(Vector3.Cross(rb.transform.right, hitBack.normal), hitBack.normal), Time.deltaTime * 5.0f);
        }

        rb.AddForce(-rb.transform.up * Time.deltaTime * gravitySpeed);
    }
}
