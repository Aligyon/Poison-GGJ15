using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public Animator anim;
    public Transform model;

    public float hp = 100f;

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
    public LineRenderer linerenderer;

    float jumpcooldown = 0.15f;
    float jumpcd = 0;
    float forcecooldown = 0.3f;
    float forcecd = 0;


    public string jumpbutton;
    public string xaxis;
    public string ropebutton;
    public string forcebutton;
    public string grabbutton;
    public string drinkbutton;

    public float moveforce;
    public float forceforce;

    public float acceleration;
    public float maxSpeed;
    public float jumpstrength;
    public float drinktime = 3f;

    public bool hasAntidote = false;
    public Transform antidote;

    // // // // // // 
    Vector2 force;
    float dot;
    float v;
    bool grounded = false;
    bool jump = false;
    bool forcing = false;
    bool ropeactive = false;
    bool grabbing = false;
    bool drinking = false;

    Vector2 drinkpos;
    float drinkprogress = 0;

    Transform grabobject;
    Vector2 grabpoint;
    Vector2 grabpos;
    Vector2 grounddir;

	// Use this for initialization
	void Start () {
        drinkpos = new Vector2(0, 0);
        grounddir = new Vector2(0, 0);
        grabpos = new Vector2(0, 0);
        grabpoint = new Vector2(0, 0);
	}

    void Update() {

        UpdateAnimation();

        if (tie.enabled) UpdateRope(); else linerenderer.enabled = false;
        
        if (hp <= 0) Application.LoadLevel(0);

        jumpcd -= Time.deltaTime;
        forcecd -= Time.deltaTime;

        if (drinking) { drinkprogress += Time.deltaTime; grabbing = false; }

        if (Input.GetButtonDown(jumpbutton) && grounded && (jumpcd <= 0f)) {
            jump = true;
        }
        if (Input.GetButtonDown(forcebutton) && (forcecd <= 0f)) {
            forcing = true;
        }
        if (grabbing) {

            Vector2 worldpos = ((Vector2)grabobject.position) + grabpos;
            //Debug.DrawRay(player.position, worldpos-(Vector2)player.position , Color.red);


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

        CheckFooting();
        Drink();

        if (drinking) { return; }
        
        Rope();
        Grab();
        Force();
        Move();
        Jump();
        
	}

    void Move() {

        float xinput = Input.GetAxis(xaxis);
        anim.SetFloat("xaxis", xinput);

        if (xinput == 0)
            return;

        //force = new Vector2(Input.GetAxis(xaxis), 0);
        force = new Vector2(0, 0);
        if (grounddir.magnitude > 0) force = grounddir*xinput;
        else force.x = xinput;
        force = force * acceleration;

        hp -= (force.magnitude*Time.fixedDeltaTime*0.0016f);
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
            hp -= 0.4f;
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
            //Debug.DrawLine(player.position, otherplayer.transform.position, Color.blue);

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
                hp -= (Time.fixedDeltaTime * 0.1f);
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
                
                PlayerInput p = (PlayerInput)otherplayer.GetComponent(typeof(PlayerInput));
                if(p.hasAntidote) p.DropAntidote(dir*forceforce);
            }
            Transform effect;
            effect = (Transform)Instantiate(forceEffect, player.position, Quaternion.identity);

            forcecd = forcecooldown;
            forcing = false;

            hp -= 0.4f;
        }
    }
    void Drink() {

        if (drinking) {
            if (drinkprogress >= drinktime) Application.LoadLevel(0);
            if (Vector2.Distance(drinkpos, player.position) > 0.1f) {
                drinking = false;
                DropAntidote(player.rigidbody2D.velocity*20);
            }

        }
        if (Input.GetButton(drinkbutton) && hasAntidote) {
            if (player.rigidbody2D.velocity.magnitude < 0.2f) {
                drinkpos = player.position;
                drinking = true;
            }
        }


    }

    RaycastHit2D GrabRay(Vector2 pos, Vector2 dir) {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 0.65f, grablayer);
        //Debug.DrawRay(pos, dir * 0.8f, Color.cyan, 1f);
        return hit;
    }
    public void DropAntidote(Vector2 f) {

        antidote.position = player.position;

        antidote.gameObject.SetActive(true);
        
        //antidote.rigidbody2D.AddForce( (player.rigidbody2D.velocity.normalized + player.rigidbody2D.velocity/5)*100*antidote.rigidbody2D.mass );
        //antidote.rigidbody2D.velocity = (player.rigidbody2D.velocity.normalized + player.rigidbody2D.velocity / 5) * 100 * antidote.rigidbody2D.mass;
        //Debug.Log((player.rigidbody2D.velocity.normalized + player.rigidbody2D.velocity / 5) * 100 * antidote.rigidbody2D.mass);

        antidote.rigidbody2D.AddForce(f * antidote.rigidbody2D.mass * 2);

        Antidote a; a = (Antidote)antidote.GetComponent(typeof(Antidote));

        a.SetInactiveTime(1f);
        hasAntidote = false;

    }
    public void PickUpAntidote(Transform a) {
        antidote = a;
        a.gameObject.SetActive(false);
        hasAntidote = true;

    }
    void UpdateRope() {

        linerenderer.enabled = true;
        linerenderer.SetPosition(0, player.position);
        linerenderer.SetPosition(1, otherplayer.transform.position);

    }
    void UpdateAnimation() {

        float xax = Input.GetAxis(xaxis);

        anim.SetBool("grounded", grounded);
        anim.SetFloat("vspeed", player.rigidbody2D.velocity.y);

        if (grabbing) {
            anim.SetBool("grabbing", true);


            Vector3 worldpos = (grabobject.position) + (Vector3)grabpos;
            //Debug.DrawRay(player.position, worldpos-(Vector2)player.position , Color.red);

            Vector3 bajs = worldpos - player.position;
            Quaternion lookat = Quaternion.LookRotation(bajs, Vector3.up);
            Debug.Log(lookat);
            model.rotation = Quaternion.Lerp(model.rotation, lookat, Time.deltaTime * 5);

            return;

        }
        else anim.SetBool("grabbing", false);

        if (xax > 0.1f || xax < -0.1f) {
            Vector3 bajs = new Vector3(Input.GetAxis(xaxis), 0, 0);
            Quaternion lookat = Quaternion.LookRotation(bajs, Vector3.up);
            Debug.Log(lookat);
            model.rotation = Quaternion.Lerp(model.rotation, lookat, Time.deltaTime * 5);
        }
        
        

        //Quaternion.LookRotation
    }

}
