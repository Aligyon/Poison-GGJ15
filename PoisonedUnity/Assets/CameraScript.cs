using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public Camera cam;

    public PlayerInput p1, p2;

    public float camspeed = 1;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float z = -( Vector2.Distance(p1.transform.position, p2.transform.position)*0.6f+12 );

        Vector3 from = new Vector3();
        Vector3 to = new Vector3();

        Vector3 averagePos = (p1.transform.position+p2.transform.position)/2;
        averagePos.z = z;

        cam.transform.position = Vector3.Lerp(cam.transform.position,averagePos, Time.deltaTime * camspeed);

	
	}
    void OnGUI() {
        GUI.Label(new Rect(50,50,100,50),"P1hp: "+p1.hp);
        GUI.Label(new Rect(Screen.width-200, 50, 100, 50), "P2hp: " + p2.hp);
    }
}
