using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    public float laserSpeed = 25f;
    public float damage = 100f;

    private Vector3 topBoundary;

    void Start(){
        topBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        GetComponent<Rigidbody>().velocity = new Vector3(0, laserSpeed, 0);
    }

    void Update(){
        if (transform.position.y >= topBoundary.y){
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
