using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public PhysicsMaterial2D groundedmat;
    public PhysicsMaterial2D airbornemat;

    public Transform player;
    public Rigidbody2D rb;
    public LayerMask playerlayer;
    public LayerMask grablayer;
    public DistanceJoint2D tie;
    public SpringJoint2D grab;
    public PlayerInput otherplayer;
    public Transform forceEffect;

    float jumpcooldown = 0.15f;
    float jumpcd = 0;
    float forcecooldown = 0.3f;
    float forcecd = 0;


    public string jumpbutton;
    public string xaxis;
    public string ropebutton;
    public string forcebutton;
    public string grabbutton;

    public float moveforce;
    public float forceforce;

    public float acceleration;
    public float maxSpeed;
    public float jumpstrength;

    // // // // // // 
    Vector2 force;
    float dot;
    float v;
    bool grounded = false;
    bool jump = false;
    bool forcing = false;
    bool ropeactive = false;
    bool grabbing = false;

    Transform grabobject;
    Vector2 grabpoint;
    Vector2 grabpos;
    Vector2 grounddir;

	// Use this for initialization
	void Start () {
        grounddir = new Vector2(0, 0);
        grabpos = new Vector2(0, 0);
        grabpoint = new Vector2(0, 0);
	}

    void Update() {

        jumpcd -= Time.deltaTime;
        forcecd -= Time.deltaTime;

        if (Input.GetButtonDown(jumpbutton) && grounded && (jumpcd <= 0f)) {
            jump = true;
        }
        if (Input.GetButtonDown(forcebutton) && (forcecd <= 0f)) {
            forcing = true;
        }
        if (grabbing) {
            
            Debug.Log(grabpos);
            Vector2 worldpos = ((Vector2)grabobject.position) + grabpos;
            Debug.DrawRay(player.position, worldpos-(Vector2)player.position , Color.red);

            

            player.position = Vector2.Lerp(player.position,worldpos,Time.deltaTime*7);
            player.rigidbody2D.velocity = new Vector2(0,0);

            Debug.DrawLine(player.position, ((Vector2)grabobject.position + grabpoint), Color.cyan);

            if (Vector2.Distance(player.position, worldpos) > 0.6f)
                grabbing = false;

            //return;

        }


    }
	
	// Update is called once per frame
	void FixedUpdate () {

        Rope();
        Grab();
        Force();
        CheckFooting();
        Move();
        Jump();
        
	}

    void Move() {

        float xinput = Input.GetAxis(xaxis);

        if (xinput == 0)
            return;

        //force = new Vector2(Input.GetAxis(xaxis), 0);
        force = new Vector2(0, 0);
        if (grounddir.magnitude > 0) force = grounddir*xinput;
        else force.x = xinput;
        force = force * acceleration;
        //force = Vector3.ClampMagnitude(force, acceleration);

        //Debug.Log (Time.fixedDeltaTime);

        dot = Vector3.Dot(force.normalized, rb.velocity.normalized);

        v = rb.velocity.magnitude;
        float dotv = dot * v;

        if (dotv < maxSpeed) {

            if (grounded == false) { force = force / 5; }

            Debug.DrawRay(player.position, force / 6, Color.red);

            //Vector3 rightstick = new Vector3(Input.GetAxis(xaxis), 0);
            //Debug.DrawRay(player.position, rightstick, Color.blue);

            /*
                if(Input.GetButton("Fire1")) {
                    player.renderer.material.SetColor("_Color",Color.red);
                    force -= (force*dot*maxspeed)/5;
                }
                else { player.renderer.material.SetColor("_Color",Color.white); }
            */

            rb.AddForce(force*rb.mass);

        }

    }
    void Jump() {

        if (jump) {
            rb.AddForce(Vector2.up * jumpstrength * rb.mass);
            jump = false;
            jumpcd = jumpcooldown;
        }

    }

    void CheckFooting() {

        Vector2 rayorigin = new Vector2(0, 0);
        rayorigin = player.position;
        bool rayL, rayR;
        /*
        RaycastHit2D hit = Physics2D.Raycast(rayorigin, -Vector2.up, 0.8f, playerlayer);
        
        if (hit.collider != null) {
            Debug.DrawRay(rayorigin, -Vector2.up*0.8f, Color.green);
            rayC = true;
            //grounded = true;
            //rb.collider2D.sharedMaterial = groundedmat;
        }
        else {
            Debug.DrawRay(rayorigin, -Vector2.up*0.8f, Color.red);
            rayC = false;
            //grounded = false;
            //rb.collider2D.sharedMaterial = airbornemat;
        }*/

        rayorigin.x = rayorigin.x + 0.25f;
        RaycastHit2D hit1 = Physics2D.Raycast(rayorigin, -Vector2.up, 0.8f, playerlayer);

        if (hit1.collider != null) {
            Debug.DrawRay(rayorigin, -Vector2.up * 0.8f, Color.green);
            rayL = true;


        }
        else {
            Debug.DrawRay(rayorigin, -Vector2.up * 0.8f, Color.red);
            rayL = false;

        }

        rayorigin.x = rayorigin.x - 0.5f;
        RaycastHit2D hit2 = Physics2D.Raycast(rayorigin, -Vector2.up, 0.8f, playerlayer);

        if (hit2.collider != null) {
            Debug.DrawRay(rayorigin, -Vector2.up * 0.8f, Color.green);
            rayR = true;

        }
        else {
            Debug.DrawRay(rayorigin, -Vector2.up * 0.8f, Color.red);
            rayR = false;

        }
        
       // grounddir = grounddir.normalized;
        //Debug.Log(grounddir);
        Debug.DrawRay(player.position, grounddir, Color.blue);

        if (rayR || rayL) {
            if (rayR & rayL) {
                grounddir = hit1.point - hit2.point;
                rb.collider2D.sharedMaterial = groundedmat;
            }
            grounded = true;

        }
        else {
            grounddir.x = 0; grounddir.y = 0;
            grounded = false;
            rb.collider2D.sharedMaterial = airbornemat;
        }


        /*
        //Ray footingray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(player.transform.position, Vector3.down, out FootingRayHit, 0.2f)) {
            player.renderer.material.SetColor("_Color", Color.white);
            //player.collider.material = new PhysicMaterial("Rubber");
            grounded = true;
        }
        else {
            player.renderer.material.SetColor("_Color", Color.red);
            //player.collider.material = new PhysicMaterial("Ice");
            grounded = false;
        }*/

    }

    void Rope() {

        if (Input.GetButton(ropebutton)) {
            if (!ropeactive) {
                tie.distance = Vector2.Distance(player.position, otherplayer.transform.position);
                tie.enabled = true;
                ropeactive = true; 
            }
            Debug.DrawLine(player.position, otherplayer.transform.position, Color.blue);

        }
        else {
            if (ropeactive) {
                ropeactive = false;
                tie.enabled = false;
            }
        }
    }

    void Grab() {
        if (Input.GetButton(grabbutton)) {

            if (grabbing) {
                return;
            }

            Vector2 dir = new Vector2(0,1);
            RaycastHit2D[] hits; hits = new RaycastHit2D[8];
            int bestgrab = 10;

            for (int i = 7; i >= 0; i--) {

                hits[i] = GrabRay(player.position, dir);

                if (hits[i].collider != null) {
                    if (bestgrab == 10) {
                        bestgrab = i;
                    }
                    else if (hits[i].point.y > hits[bestgrab].point.y) {
                        bestgrab = i;
                    }
                }

                dir = Quaternion.AngleAxis(-45, Vector3.forward) * dir;

            }
            if (bestgrab != 10) {
                grabbing = true;
                grabobject = hits[bestgrab].collider.transform;
                grabpos = (Vector2)player.position - (Vector2)hits[bestgrab].collider.transform.position;
                grabpoint = hits[bestgrab].point - (Vector2)hits[bestgrab].collider.transform.position;
                
                //grabpoint = hits[bestgrab].point;
                //grabpos = player.position;
            }
            

            /*
            Collider2D grabbable = Physics2D.OverlapCircle(player.position, 0.5f, grablayer);
            if (grabbable != null) {
                Debug.Log(grabbable.name);
                //grab.enabled = true;
                grabbing = true;
                grabpos = player.position;
            }
                */
        }
        else if (grabbing) {
            //grab.enabled = false;
            grabbing = false;
        }

    }
    void Force() {
        if (forcing) {
            if (Vector2.Distance(otherplayer.transform.position, player.position) < 2f) {
                Vector2 dir = otherplayer.transform.position - player.position;
                otherplayer.rigidbody2D.AddForce(dir*forceforce);
            }
            Transform effect;
            effect = (Transform)Instantiate(forceEffect, player.position, Quaternion.identity);

            forcecd = forcecooldown;
            forcing = false;
        }
    }

    RaycastHit2D GrabRay(Vector2 pos, Vector2 dir) {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 0.65f, grablayer);
        //Debug.DrawRay(pos, dir * 0.8f, Color.cyan, 1f);
        return hit;
    }

}
