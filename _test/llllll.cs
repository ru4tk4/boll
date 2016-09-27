using UnityEngine;
using System.Collections;

public class llllll : MonoBehaviour {
    public Transform target;
    public AnimationCurve ForwardSpeed;
    public AnimationCurve turnSpeed;
    public AnimationCurve rightSpeed;
    public float speed;
    public Vector3 speed1;
    public float radius;

    public float timeLength = 10f;
    public bool randomStartTime = false;
    float _startTime = 0;
    float _currentTime = 0;

    public float CurrentTime()
    {       
        return (((_currentTime / timeLength) + _startTime) % 1f);
    }
    // Use this for initialization
    void Start () {
        //radius = 80;
        transform.GetChild(0).transform.localPosition = new Vector3(0, radius, 0);
        Destroy(gameObject, timeLength);

        //GetComponentInChildren<Transform>().localPosition 
        //speed1 = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
	}
	
	// Update is called once per frame
	void Update () {
        _currentTime += Time.deltaTime;
        speed1 = new Vector3(ForwardSpeed.Evaluate(_currentTime)*speed, turnSpeed.Evaluate(_currentTime) * speed, rightSpeed.Evaluate(_currentTime) * speed);
        SphereClass.LineMove(transform, speed1,speed);
       /* transform.LookAt(target);
        transform.position += transform.right * speed;*/
	}
}
