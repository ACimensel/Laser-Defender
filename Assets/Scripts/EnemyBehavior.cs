using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

    public GameObject enemyLaser;
    public GameObject enemyDestruction;
    public GameObject enemyDamage;
    public float health = 100f;
    public float shotsPerSecond = 0.8f; //frequency of shots
    public float damage = 100f;
    public int scoreValue = 150;

    public float tilt = 5f;
    public float dodge = 18f;
    public float smoothing = 5f;
    public Vector2 startWait = new Vector2(0.5f, 1f);
    public Vector2 maneuverTime = new Vector2(1f, 2f);
    public Vector2 maneuverWait = new Vector2(1f, 2f);
    
    private float speed = -1.0f;
    private float currentSpeed;
    private float targetManeuver;
    private Vector3 bottomLeftBoundary;
    private Vector3 topRightBoundary;
    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null){
            Debug.Log("Cannot find 'GameController' script");
        }

        bottomLeftBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        topRightBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        currentSpeed = GetComponent<Rigidbody>().velocity.y;

        StartCoroutine(Evade());
    }

    void Update(){
        float probability = Time.deltaTime * shotsPerSecond; // probability = time * frequency
        if (Random.value < probability) {
            Invoke("Fire", .1f);
        }

        if (transform.position.y <= bottomLeftBoundary.y - 1.0f){
            Destroy(gameObject);
        }
    }

    void FixedUpdate(){
        float newManeuver = Mathf.MoveTowards(GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);
        GetComponent<Rigidbody>().velocity = new Vector3(newManeuver, currentSpeed, 0.0f);
        /*GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, bottomLeftBoundary.x, topRightBoundary.x),
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, bottomLeftBoundary.y, topRightBoundary.y),
            0.0f
        );*/

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(-90, GetComponent<Rigidbody>().velocity.x * -tilt, 0);
    }

        IEnumerator Evade(){
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));
        while (true){
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
            yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
        }
    }

    void OnTriggerEnter(Collider col){
        PlayerLaser missile = col.gameObject.GetComponent<PlayerLaser>();
        PlayerController3D player = col.gameObject.GetComponent<PlayerController3D>();
        if (missile){
            health -= missile.GetDamage();
            if (health <= 0){
                gameController.AddScore(scoreValue);
                Instantiate(enemyDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Destroy(gameObject);
            }
            else {
                Instantiate(enemyDamage, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            }
        }
        if (player){
            gameController.AddScore(scoreValue);
            Instantiate(enemyDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public float GetDamage(){
        return damage;
    }

    void Fire(){
        Instantiate(enemyLaser, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
    }
}
