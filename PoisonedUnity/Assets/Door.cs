using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public float opentime = 5f;
    public Transform p1, p2;
    public Transform triggercenter;
    public float triggerradius = 3;

    bool opening = false;

    float t;


	// Use this for initialization
	void Start () {
        t = opentime;
	}
    void OnGUI() {
        if (!opening) return;
        Vector3 p = Camera.main.WorldToScreenPoint(triggercenter.position);
        GUI.Label(new Rect(p.x - 50, Screen.height-p.y - 50, 100, 50), "OPENING...");
    }
	
	// Update is called once per frame
	void Update () {

        if (Vector2.Distance(p1.position, triggercenter.position) < triggerradius && Vector2.Distance(p2.position, triggercenter.position) < triggerradius) {
            t -= Time.deltaTime;
            opening = true;
            if (t <= 0) Open();
        }
        else {
            t = opentime;
            opening = false;
        }
	
	}

    void Open() {
        Destroy(gameObject);
    }
}
