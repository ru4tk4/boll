using UnityEngine;
using System.Collections;

public class SphereClass : MonoBehaviour {

	static public void LineMove(Transform target , Vector3 direction, float speed)
    {
        target.Rotate(direction*speed);
    }
}
