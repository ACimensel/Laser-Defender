using UnityEngine;
using System.Collections;

public class AsteroidBehavior : MonoBehaviour
{
    public GameObject asteroidDestruction;
    public float damage = 100f;
    private int scoreValue = 90;

    private float tumble = 3f;
    private float speed = -2.0f;
    private Vector3 bottomBoundary;
    private GameController gameController;

    void Start ()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null){
            Debug.Log("Cannot find 'GameController' script");
        }

        bottomBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
        GetComponent<Rigidbody>().velocity = transform.up * speed;
    }

    void Update(){
        if (transform.position.y <= bottomBoundary.y - 1.0f){
            Destroy(gameObject);
        }
    }

    public float GetDamage(){
        return damage;
    }

    void OnTriggerEnter(Collider other){
        gameController.AddScore(scoreValue);
        Instantiate(asteroidDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        Destroy(gameObject);
    }
}