using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public AudioClip mainLaser;
    public AudioClip sideLaser;

    public GameObject playerLaser;
    public GameObject playerLaserSide;
    public GameObject laserCharge;
    public GameObject playerDestruction;
    public float shipSpeed = 10f;
    public float padding = 1f;
    public static float firingRate = 0.6f;
    public static float chargeTime = 0.6f;
    public float health = 300f;

    private float xmin;
    private float xmax;

    void Start (){
        float distanceZ = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceZ)); //works with single camera, works out boundaries of playspace. "x" and "y" values between 0 and 1
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceZ)); // 1 is rightmost x location of camera
        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;
    }
	
	void Update (){
        if (Input.GetKeyDown(KeyCode.Space)){
            LaserCharge();
            InvokeRepeating("LaserShot", chargeTime, firingRate);   // Starting in 0.1 seconds, a projectile will be launched every 0.3 seconds
            InvokeRepeating("LaserSideShot", .01f, 0.25f);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            CancelInvoke();   //Cancels all Invoke calls 
        }
    }

    void FixedUpdate(){
        ShipMovement();
    }

    void ShipMovement() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(moveHorizontal, 0f, 0f);
        GetComponent<Rigidbody2D>().velocity = movement * shipSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, xmin, xmax), transform.position.y, transform.position.z);
        
        /*
        if (Input.GetKey(KeyCode.RightArrow)){
            //transform.position += new Vector3(shipSpeed * Time.deltaTime, 0, 0);
            transform.position += Vector3.right * shipSpeed * Time.deltaTime; //Time.deltaTime makes it frame 
        }

        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.position += Vector3.left * shipSpeed * Time.deltaTime;
        }
        //restrict the player to the gamespace
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, xmin, xmax), transform.position.y, transform.position.z);
        */
    }

    void LaserCharge(){
        Instantiate(laserCharge, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
    }

    void LaserShot(){
        Instantiate(playerLaser, new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
        GetComponent<AudioSource>().PlayOneShot(mainLaser);
    }

    void LaserSideShot(){
        Instantiate(playerLaserSide, new Vector3(transform.position.x + 0.25f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        Instantiate(playerLaserSide, new Vector3(transform.position.x - 0.25f, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        GetComponent<AudioSource>().PlayOneShot(sideLaser);
    }

    void OnCollisionEnter2D(Collision2D col){
        EnemyLaser missile = col.gameObject.GetComponent<EnemyLaser>();
        if (missile){
            health -= missile.GetDamage();
            if (health <= 0){
                Instantiate(playerDestruction, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}