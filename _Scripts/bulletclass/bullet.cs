using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {
	Rigidbody obj;
	public float speed;
	public float m_RotaSmooth;
    public float m_AngleSpeed=135;
    Transform playertran;
	// Use this for initialization
	void Start () {
        
		obj = GetComponent<Rigidbody> ();
        Destroy(gameObject, 5);
//		playertran = GameObject.FindGameObjectWithTag ("Gost");
	}
	
	// Update is called once per frame
	float yVelocity = 0.0F;
	void Update () {

        obj.AddForce(transform.forward*speed, ForceMode.Force);
		//transform.position += transform.forward*speed;
		transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y,transform.eulerAngles.y +m_AngleSpeed, ref yVelocity, m_RotaSmooth), 0);
	}
}
