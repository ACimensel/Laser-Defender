using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour {

    public GameObject enemyPrefab;
    public float width = 7f;
    public float height = 3f;
    public float shipSpeed = 5f;
    //float tilt = 5f;

    private float xmin;
    private float xmax;
    public static bool movingRight = true;
    private float spawnDelay = 0.2f;

    void Start (){
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)); //works with single camera, works out boundaries of playspace. "x" and "y" values between 0 and 1
        Vector3 rightBoundary = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera)); // 1 is rightmost x location of camera
        xmin = leftBoundary.x + width/2;
        xmax = rightBoundary.x - width/2;

        EnemySpawnUntilFull();
    }

    void Update(){
        if (AllMembersDead()){
            print("Hi");
            Invoke("EnemySpawnUntilFull", .5f); //adds a X sec delay to the respawn
            //LevelManager.LoadNextLevel();
        }
    }

    void FixedUpdate(){
        if (movingRight){
            transform.position += new Vector3(1 * shipSpeed * Time.deltaTime, 0.015f * Mathf.Sin(transform.position.x), 0); // Time.deltaTime is used to ensure the movement is smooth, speed does not change if you have fast or slow computer
        } else{
            transform.position += new Vector3(-1 * shipSpeed * Time.deltaTime, 0.015f * Mathf.Sin(transform.position.x), 0);
        }
        
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, xmin, xmax), transform.position.y, transform.position.z);
        
        if (transform.position.x <= xmin){
            movingRight = true;
        } else if (transform.position.x >= xmax){
            movingRight = false;
        }

        //EnemyRotator();
    }

    /*void EnemyRotator() {
        foreach (Transform childPositionGameObject in transform){
            Debug.Log(childPositionGameObject);
            foreach (Transform childEnemyGameObject in transform){
                Debug.Log(childEnemyGameObject);
                if (movingRight){
                    GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f * -tilt, 0.0f);
                } else if (!movingRight){
                    GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f * -tilt, 0.0f);
                }
            }
        }
    }*/

    /*void EnemySpawner() {
        foreach (Transform child in transform){
            //Instantiate(enemyPrefab, new Vector3(8, 10, 0), Quaternion.identity); //Instantiates as an object at 8, 10 //Quaternion deals with rotation of object. ".identity" for not having rotation effect
            GameObject enemy = Instantiate(enemyPrefab, child.transform.position, Quaternion.identity) as GameObject; // instantiates as GameObject under child object location
            //enemy.transform.parent = transform; // created enemy spawns under Enemy Formation dropdown (child)
            enemy.transform.parent = child; // created enemy spawns under Position (child of Enemy Formation). "Reparenting the enemy"
        }
    }*/

    void EnemySpawnUntilFull(){
        Transform freePosition = NextFreePosition();
        if (freePosition){
            GameObject enemy = Instantiate(enemyPrefab, freePosition.position, Quaternion.identity) as GameObject; // instantiates as GameObject under child object location
            enemy.transform.parent = freePosition; // created enemy spawns under Position (child of Enemy Formation). "Reparenting the enemy"
            Invoke("EnemySpawnUntilFull", spawnDelay);
        }
    }

    Transform NextFreePosition() {
        foreach (Transform childPositionGameObject in transform){
            if (childPositionGameObject.childCount == 0){
                return childPositionGameObject;
            }
        }
        return null;
    }

     bool AllMembersDead(){
        foreach (Transform childPositionGameObject in transform){
            if (childPositionGameObject.childCount > 0){
                return false;
            }
        }
        print("true");
        return true;
    }

    void OnDrawGizmos()
    { // allows you to see location of enemy formation without it being selected
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    }
}
