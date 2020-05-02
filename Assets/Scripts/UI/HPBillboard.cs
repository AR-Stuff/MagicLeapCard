using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBillboard : MonoBehaviour
{
	//public Transform cam;

    void LateUpdate()
    {
		transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
