using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    public int keys = 0;
    public float speed = 20.0f;
    // public GameObject door;

    public int requiredKeys = 3; //number of keys required to open the door

    public Text keyAmount;
    public Text youWin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
             transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, speed * Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, -speed * Time.deltaTime, 0);
        }

        // if(keys == 3)
        // {
        //     Destroy(door);
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Keys")
        {
            Debug.Log("Key HIT!!!");
            keys++;
            // keyAmount.text = "Keys: " + keys;
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Door") // silver
        {
            // Destroy(collision.gameObject);
            // youWin.text = "YOU WIN!!!";
            if (keys >= requiredKeys)
            {
                Destroy(collision.gameObject);
                // youWin.text = "YOU WIN!!!";
                Debug.Log("YOU WIN!!!");
            }
            else
            {
                Debug.Log("Collect more keys to open the door!");
            }
        }

        if(collision.gameObject.tag == "Enemies")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(collision.gameObject.tag == "Walls")
        {
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
                Debug.Log("Left");
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
                Debug.Log("Right");
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(0, -speed * Time.deltaTime, 0);
                Debug.Log("Up");
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
                Debug.Log("Down");
            }
        }
    }
}
