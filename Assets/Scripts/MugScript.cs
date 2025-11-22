using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugScript : MonoBehaviour
{
    public List<int> contents; // index 0 is type of milk (0 - 3), index 1 is sugar added (0 - 1), index 2 is espresso added (0 - 1)
    GameObject player;
    bool playerIsClose = false;
    public GameObject mugHeld = null;
    bool holdingMug = false;
    public Renderer mugRenderer;
    // Start is called before the first frame update
    private Vector3 originalPosition;
    //bool holdingBottle = false;
    GameObject bottle;
    bool filling = false;
    public Transform milkLiquid;
    public float fillSpeed = 0.5f;
    public float maxFillHeight = 1.0f;
    bool holdingBottle = false;

    void Start()
    {
        contents = new List<int> { 0, 0, 0 };
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player found: " + player); // this should not be null
        originalPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holdingMug && playerIsClose)
            {
                PickupMug();
            }
            else if (holdingMug) 
            {
                PutDownMug();
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("P pressed");
            //Debug.Log(holdingBottle);
            Debug.Log(playerIsClose);
            holdingBottle = player.GetComponent<PlayerScript>().heldItem != null
                     && (player.GetComponent<PlayerScript>().heldItem.CompareTag("Whole"));
            if (playerIsClose && holdingBottle)
            {
                Debug.Log("milk poured");
                PourMilk();
            }
        }

        //Debug.Log(Input.GetKey(KeyCode.P) + "P is being held");
        //Debug.Log(holdingBottle + "bottle is being held");
        //Debug.Log(playerIsClose + "player is close");
        
        filling = Input.GetKey(KeyCode.P) && holdingBottle && playerIsClose;


        if (filling)
        {
            Debug.Log("filling");
            Vector3 scale = milkLiquid.localScale;
            scale.y = Mathf.Clamp(scale.y + (fillSpeed * Time.deltaTime), 0, maxFillHeight);
            milkLiquid.localScale = scale;
        }
    }

    public void ChangeMugColor(Color newColor)
    {
        if (mugRenderer != null)
        {
            mugRenderer.material.color = newColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsClose = true;
        }
        if (other.gameObject.tag == "Whole")
        {
            Debug.Log("holding bottle");
            bottle = other.gameObject;
            //holdingBottle = true;
        }
    }
    
    private void OnTriggerStay(Collider other)  
{
    if (other.CompareTag("Player")) playerIsClose = true;
}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsClose = false;
        }
    }

    void PickupMug()
    {
        Debug.Log("in pick up mug");
        var ps = player.GetComponent<PlayerScript>();
        if (ps == null || !ps.CanHoldItem()) return;

        ps.HoldItem(gameObject);   // this snaps to holdPoint, the hand cube i added on Egg
        holdingMug = true;
    }

    void PutDownMug()
    {
        transform.SetParent(null);

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        foreach (var c in GetComponents<Collider>()) c.enabled = true;  // re-enable

        var ps = player.GetComponent<PlayerScript>();
        if (ps) ps.DropItem();

        holdingMug = false;
    }

    void PourMilk()
    {
        Debug.Log("added milk");
        if (bottle.tag == "Whole")
        {
            contents[0] = 1;
        } else if (bottle.tag == "Almond")
        {
            contents[0] = 2;
        } else if (bottle.tag == "Oat")
        {
            contents[0] = 3;
        }
    }

}
