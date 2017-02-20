using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;

	HashSet<IntractableItem> objectsHoveringOver = new HashSet<IntractableItem>();

	private IntractableItem close_Object;
	private IntractableItem Interacting_Object;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller == null) {
			Debug.Log("Controller not initialized");
			return;
		}

		if (controller.GetPressUp (gripButton)) {
			Debug.Log ("Grip button Pressed");
			// find the close object to the controller when there are multiple intractable objects around
			float minimun_distance = float.MaxValue;
			float distance;
			foreach (IntractableItem item in objectsHoveringOver) {
				distance = (item.transform.position - transform.position).sqrMagnitude;
				if (distance < minimun_distance) {
					minimun_distance = distance;
					close_Object = item;
				}

			}

			Interacting_Object = close_Object;
			close_Object = null;
			// Pass the object Interaction from one controller to other controller
			if (Interacting_Object) {
				if (Interacting_Object.IsInteracting()) {
					Interacting_Object.IntractionEnd (this);
				}
				Interacting_Object.IntractionBegin (this);
			}
		}

		if (controller.GetPressDown (gripButton) && Interacting_Object != null) {
			Debug.Log ("Grip button UnPressed");
			Interacting_Object.IntractionEnd (this);
		}

	}


	// Adds all colliding items to a HashSet for processing which is closest
	private void OnTriggerEnter(Collider collider) {
		Debug.Log("Entered");
		IntractableItem collidedObject = collider.GetComponent<IntractableItem> ();
		if (collidedObject) {
			objectsHoveringOver.Add (collidedObject);
		}
	}

	// Remove all items no longer colliding with to avoid further processing
	private void OnTriggerExit(Collider collider) {
		Debug.Log("Exit");
		IntractableItem collidedObject = collider.GetComponent<IntractableItem> ();
		if (collidedObject) {
			objectsHoveringOver.Remove (collidedObject);
		}
	}
}
