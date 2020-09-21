using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

    void OnDrawGizmos() { // allows you to see location of GameObject without it being selected
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
