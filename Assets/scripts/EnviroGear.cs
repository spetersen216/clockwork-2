using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviroGear : MonoBehaviour {
	public float maxAngularVelocity=90f;
	public float mass=1f;
	public float angularAcceleration=10f;
	public bool isMovable=false;

	private List<EnviroGear> neighbors=new List<EnviroGear>();
	private Transform gearTrans;
	float radius;
	private Rigidbody rigidBody;
	private static List<Collider> staticColliders=new List<Collider>();
	
	private float curAngularVelocity;
	private float momentOfIntertia { get { return 0.5f*mass*transform.localScale.y*transform.localScale.z; } }
	private float angularMomentum {
		get { return momentOfIntertia*curAngularVelocity; }
		set { curAngularVelocity = value/momentOfIntertia; }
	}
	
	void Start ()
	{
		// initialize vars
		radius = gameObject.GetComponent<Collider>().bounds.extents.x;
		rigidBody = GetComponent<Rigidbody>();
		
		// handle static gears
		if (!isMovable) {
			gameObject.layer = LayerMask.NameToLayer("StaticGear");

			// find the physical (non-trigger) collider
			/*Collider[] colliders = GetComponents<Collider>();
			Collider col=null;
			for (int i=0; i<colliders.Length; ++i)
				if (!colliders[i].isTrigger)
					col = colliders[i];

			// ignore collisions with other physical colliders
			for (int i=0; i<staticColliders.Count; ++i) {
				Physics.IgnoreCollision(col, staticColliders[i]);
				print("ignore collision between "+col.gameObject.name+" and "+staticColliders[i].gameObject.name);
				if (col.isTrigger)
					print("col is trigger!!!!!!!!!!!!!!!");
				if (staticColliders[i].isTrigger)
					print("staticColliders[i].isTrigger!!!!!!!!!!!");
				//print("colliders are triggers: "+(col.isTrigger?"col IS TRIGGER ":"col isn't trigger ")+
					//(staticColliders[i].isTrigger?"col IS TRIGGER ":"col isn't trigger "));
			}
			staticColliders.Add(col);*/
		}
	}
	
	void FixedUpdate ()
	{
		// rotate angularSpeed degrees every second
		transform.Rotate(Time.deltaTime*curAngularVelocity*Vector3.left);
		
		// add the angularAcceleration with an appropriate drag force
		/*if (angularAcceleration>0)
			curAngularVelocity += Time.fixedDeltaTime * (angularAcceleration - angularAcceleration*curAngularVelocity/maxAngularVelocity);
		else
			curAngularVelocity += Time.fixedDeltaTime * (angularAcceleration + angularAcceleration*curAngularVelocity/maxAngularVelocity);*/
		curAngularVelocity += Time.fixedDeltaTime * (angularAcceleration - angularAcceleration*Mathf.Abs(curAngularVelocity)/maxAngularVelocity);

		// average out angular speed of neighbors
		for (int i=0; i<neighbors.Count; ++i)
		{
			//print(angularMomentum+"-"+momentOfIntertia);
			// sum angularMomentum, distribute according to moment of inertia
			float totalAngularMomentum = angularMomentum - neighbors[i].angularMomentum;
			float totalMomentOfInertia = momentOfIntertia + neighbors[i].momentOfIntertia;
			angularMomentum = totalAngularMomentum*momentOfIntertia/totalMomentOfInertia;
			neighbors[i].angularMomentum = -totalAngularMomentum*neighbors[i].momentOfIntertia/totalMomentOfInertia;
		}
	}

	/// <summary>
	/// Returns the speed of an object rotating around this gear at the given point.
	/// </summary>
	public Vector3 GetVelAtPoint(Vector3 point)
	{
		// use the cross product, multiply by angularSpeed in radians
		Vector3 diff = point-transform.position;
		Vector3 result = Vector3.Cross(Vector3.forward, diff);
		return result*curAngularVelocity*Mathf.PI/180;
	}

	void OnTriggerEnter(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// add gear to neighbors if it isn't already a neighbor
		if (!gear.neighbors.Contains(this) && !neighbors.Contains(gear))
			neighbors.Add(gear);
	}

	void OnTriggerStay(Collider coll)
	{
		rigidBody.velocity = Vector3.zero;
	}

	void OnTriggerExit(Collider coll)
	{
		// find a gear, return if null
		EnviroGear gear = coll.gameObject.GetComponent<EnviroGear>();
		if (gear==null)
			return;

		// remove gear in neighbors if it's in the neighbors
		neighbors.Remove(gear);
	}
}
