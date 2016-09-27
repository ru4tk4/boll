using UnityEngine;
using System.Collections;

public class launcher : MonoBehaviour {
    public Transform target;
	public  GameObject B_bullet;
	public float B_speed;
	public float B_appearD =1;
	public float B_angle=360;
	public int B_ring_quantity = 12;
	public int B_loop_Qt = 6;
	public float B_CD = 1;
    float speed = 0;
	float Tt;
	float qr;
	Quaternion qq;
    Transform tttt;
	// Use this for initialization
	void Start () {

      
		


	}
	
	// Update is called once per frame
	void Update () {

        
        if (B_loop_Qt > 0 && Time.time > Tt) {
            //B_speed = Random.Range(0.2f, 0.8f);
            //int quantity = Random.Range(B_ring_quantity-12, B_ring_quantity+12);
            //Vector2 a = Random.insideUnitCircle * 2;
            //transform.position = new Vector3(transform.parent.position.x + a.x, transform.parent.position.y, transform.parent.position.z + a.y);
            //transform.LookAt(target);
            qr = B_angle / B_ring_quantity;
            target.Rotate(new Vector3(0, qr / 2, 0));          
            //qq.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + qr/2, transform.rotation.eulerAngles.z);
            for (int i = 0; i < B_ring_quantity; i++)
			{

                Fire(transform.position, target.rotation);
                target.Rotate(new Vector3(0, qr, 0));
                // qq.eulerAngles = new Vector3 (qq.eulerAngles.x, qq.eulerAngles.y + qr, qq.eulerAngles.z);

            }
			Tt = Time.time + B_CD;
			B_loop_Qt--;

		}

	}

	public void Fire(Vector3 pos , Quaternion rota){
		
		GameObject bullet1 = Instantiate (B_bullet, pos, rota) as GameObject;
       // bullet1.transform.Rotate(new Vector3(B_appearD, 0, 0));
        //bullet1.GetComponent<Rigidbody>().AddForce(bullet1.transform.forward * B_speed);
       /* bullet1.GetComponent<bullet>().speed = B_speed;
		*/
	}

}
