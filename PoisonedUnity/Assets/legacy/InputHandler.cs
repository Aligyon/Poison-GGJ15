using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
	
	float dot;
	Vector3 force;
	float v;
	
	public float maxspeed = 3f;
	public float acceleration = 20f;
	public float jumpstrength = 100f;
	
	public float strength = 5f;
	
	
	public Transform player;
	public Transform swinged;
	
	
	private RaycastHit FootingRayHit;
	public float footing = 1f;
	bool grounded = false;
	bool jump = false;
	
	//float fdelay = 0f;
	
	// Use this for initialization
	void Start () {
		dot = 0f;
		force = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		//fdelay = 1f+Time.fixedDeltaTime;
		
		Move();
		
		CheckFooting();
		
		Jump();
		
		Swing ();
	
	}
	
	void Update () {
		
		if(Input.GetButtonDown("Jump") && grounded) {
			jump=true;
		}		
		
		
	}
	
	void Move() {
		
			
			
	
		force = new Vector3( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical") );
		force = force*acceleration;
		force = Vector3.ClampMagnitude(force,acceleration);
		
		//Debug.Log (Time.fixedDeltaTime);
		
		
		dot = Vector3.Dot(force.normalized,player.rigidbody.velocity.normalized);
		
		v = player.rigidbody.velocity.magnitude;
		float dotv = dot*v;

		if (dotv < maxspeed ) {
			
			if(grounded==false) { force = force/3; }
			
				Debug.DrawRay(player.transform.position,force/5,Color.red);
				
				Vector3 rightstick = new Vector3(Input.GetAxis("RHorizontal"),0,Input.GetAxis("RVertical"));
				Debug.DrawRay(player.transform.position,rightstick,Color.blue);
				
				/*
					if(Input.GetButton("Fire1")) {
						player.renderer.material.SetColor("_Color",Color.red);
						force -= (force*dot*maxspeed)/5;
					}
					else { player.renderer.material.SetColor("_Color",Color.white); }
				*/
				
				player.rigidbody.AddForce(force, ForceMode.Acceleration);

		}
			
	
	
	}
	
	void CheckFooting() {
		
		//Ray footingray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(player.transform.position, Vector3.down, out FootingRayHit, 0.2f)) {
			player.renderer.material.SetColor("_Color",Color.white);
			//player.collider.material = new PhysicMaterial("Rubber");
			grounded = true;
		}
		else {
			player.renderer.material.SetColor("_Color",Color.red);
			//player.collider.material = new PhysicMaterial("Ice");
			grounded = false;
		}
		
	}
	
	void Jump() {
		
		if(jump) {
			player.rigidbody.AddForce(Vector3.up*jumpstrength);
			jump = false;
		}
		
	}

	void Swing() {
		
		Vector3 forcepoint = new Vector3(Input.GetAxis("RHorizontal"),0,Input.GetAxis("RVertical"));
		forcepoint += player.position;
		
        swinged.rigidbody.AddForce( (( forcepoint - swinged.rigidbody.position).normalized)*strength );		
		
	}
	
}
