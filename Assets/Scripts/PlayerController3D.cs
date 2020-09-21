using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; //for Text text
using UnityEngine;

public class PlayerController3D : MonoBehaviour {
    public GameObject playerLaser;
    public GameObject playerLaserSide;
    public GameObject playerDestruction;
    public GameObject playerDamage;

    public Text healthText;
    public float maxHealth = 300f;
    public float shipSpeed = 10f;
    public float padding = 1f;
    public float tilt = 5f;
    public float firingRate = 1.2f;
    public static float chargeTime = 1f;
    public SimpleHealthBar healthBar;

    private float currentHealth;
    private float xmin;
    private float xmax;
    private GameController gameController;


    void Start (){
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null){
            Debug.Log("Cannot find 'GameController' script");
        }

        float distanceZ = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceZ)); //works with single camera, works out boundaries of playspace. "x" and "y" values between 0 and 1
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceZ)); // 1 is rightmost x location of camera
        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;
        currentHealth = maxHealth;
        healthBar.UpdateBar(currentHealth, maxHealth);
        //healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            InvokeRepeating("LaserShot", chargeTime, firingRate);   // Starting in 0.1 seconds, a projectile will be launched every 0.3 seconds
            InvokeRepeating("LaserSideShot", .01f, 0.3f);
        } else if (Input.GetKeyUp(KeyCode.Space)){
            CancelInvoke();   //Cancels all Invoke calls 
        }
    }

    void FixedUpdate (){
        ShipMovement();
    }

    void ShipMovement(){
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(moveHorizontal, 0f, 0f);
        GetComponent<Rigidbody>().velocity = movement * shipSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, xmin, xmax), transform.position.y, transform.position.z);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(-90.0f, GetComponent<Rigidbody>().velocity.x * -tilt, 0.0f);
    }

    void LaserShot(){
        Instantiate(playerLaser, new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
        //GetComponent<AudioSource>().PlayOneShot(mainLaser);
    }

    void LaserSideShot(){
        Instantiate(playerLaserSide, new Vector3(transform.position.x + 0.25f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        Instantiate(playerLaserSide, new Vector3(transform.position.x - 0.25f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        //GetComponent<AudioSource>().PlayOneShot(sideLaser);
    }

    void OnTriggerEnter(Collider col){
        EnemyLaser missile = col.gameObject.GetComponent<EnemyLaser>();
        EnemyBehavior enemy = col.gameObject.GetComponent<EnemyBehavior>();
        AsteroidBehavior asteroid = col.gameObject.GetComponent<AsteroidBehavior>();
        if (missile){
            // Shake the camera for a moment to make each hit more dramatic.
            StartCoroutine("ShakeCamera");
            currentHealth -= missile.GetDamage();
            healthBar.UpdateBar(currentHealth, maxHealth);
            if (currentHealth <= 0){
                gameController.GameOver();
                Instantiate(playerDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Destroy(gameObject);
            } else {
                Instantiate(playerDamage, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            }
        }
        if (enemy){
            StartCoroutine("ShakeCamera");
            currentHealth -= enemy.GetDamage();
            healthBar.UpdateBar(currentHealth, maxHealth);
            if (currentHealth <= 0){
                gameController.GameOver();
                Instantiate(playerDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Destroy(gameObject);
            }
        }
        if (asteroid){
            StartCoroutine("ShakeCamera");
            currentHealth -= asteroid.GetDamage();
            healthBar.UpdateBar(currentHealth, maxHealth);
            if (currentHealth <= 0){
                gameController.GameOver();
                Instantiate(playerDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ShakeCamera(){
        // Store the original position of the camera.
        Vector3 origPos = Camera.main.transform.position;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f){
            // Create a temporary vector2 with the camera's original position modified by a random distance from the origin.
            Vector3 tempVec = new Vector3 (origPos.x + 0.2f * Random.insideUnitCircle.x, origPos.y + 0.2f * Random.insideUnitCircle.y, origPos.z);

            // Apply the temporary vector.
            Camera.main.transform.position = tempVec;

            // Yield until next frame.
            yield return null;
        }

        // Return back to the original position.
        Camera.main.transform.position = origPos;
    }
}
