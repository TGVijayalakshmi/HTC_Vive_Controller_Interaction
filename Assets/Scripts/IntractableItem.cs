using UnityEngine;
using System.Collections;

public class IntractableItem : MonoBehaviour {
	public Rigidbody rigidbody;
	private bool current_interact;
	private Vector3 posvector;
	private HandController attachedController;
	private Transform interactionPoint;

	private float rotation_factor = 300f;
	private float velocity_factor = 18000f;
	private Quaternion rotation;
	private float angle;
	private Vector3 axis;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		interactionPoint = new GameObject ().transform;

		// rotation_factor and velocity_factor dependent on mass of the object
		rotation_factor /= rigidbody.mass;
		velocity_factor /= rigidbody.mass;
	}
	
	// Update is called once per frame
	void Update () {
		if (attachedController && current_interact) {
			posvector = attachedController.transform.position - interactionPoint.position;
			this.rigidbody.velocity = posvector * velocity_factor * Time.fixedDeltaTime;
		
			rotation = attachedController.transform.rotation * Quaternion.Inverse (interactionPoint.rotation);
			rotation.ToAngleAxis (out angle, out axis);

			if (angle > 180) {
				angle -= 360;
			}

			this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotation_factor;
		}
	}

	public void IntractionBegin(HandController Hand){
		attachedController = Hand;
		interactionPoint.position = Hand.transform.position;
		interactionPoint.rotation = Hand.transform.rotation;
		interactionPoint.SetParent (transform, true);
		current_interact = true;

	}

	public void IntractionEnd(HandController Hand){
		if (Hand == attachedController) {
			attachedController = null;
			current_interact = false;
		}
	}

	public bool IsInteracting(){
		return current_interact;
	}

	public void OnDestroy(){
	// destroy the empty game object
		if (interactionPoint) {
			Destroy (interactionPoint.gameObject);
		}
	}
}
