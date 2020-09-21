using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour {

    public float laserSpeed = 5f;
    public float damage = 50f;
    private Vector3 bottomBoundary;

    void Start (){
        bottomBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        GetComponent<Rigidbody>().velocity = new Vector3(0, -laserSpeed, 0);
    }

    void Update(){
        if (transform.position.y <= bottomBoundary.y){
            Destroy(gameObject);
        }
    }

    public float GetDamage(){
        return damage;
    }
    
    void OnTriggerEnter(Collider col){
            Destroy(gameObject);
    }
}
