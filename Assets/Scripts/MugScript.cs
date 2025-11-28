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
    GameObject sugar;
    bool filling = false;
    public Transform milkLiquid;
    public float fillSpeed = 0.5f;
    public float maxFillHeight = 1.0f;
    bool holdingBottle = false;

    void Start()
    {
        contents = new List<int> { 0, 0, 0 };
        contents[1] = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player found: " + player); // this should not be null
        originalPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // --- Sugar insert (press E while holding a sugar cube near the mug) ---
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            // NOTE: assumes PlayerScript exposes heldItem (as used elsewhere in this file).
            var ps = player ? player.GetComponent<PlayerScript>() : null;
            var held = (ps != null) ? ps.heldItem : null;

            bool holdingSugar = held != null && (held.CompareTag("sugar"));

            if (holdingSugar)
            {
                // If your contents[1] is a sugar flag (0/1), set it here.
                if (contents != null)
                {
                    if (contents.Count < 2) contents.Add(0);     // safety if list is too short
                    if (contents[1] == 0)
                    {
                        contents[1] = 1;
                        Debug.Log("Sugar added to mug.");
                    }
                    else
                    {
                        Debug.Log("Mug already has sugar.");
                    }
                }

                // Clear the player's held item and remove the cube
                // (DropItem may momentarily re-enable physics; destroying right after is fine)
                ps.DropItem();
                Destroy(held);

                // IMPORTANT: stop further E-processing this frame so we don't also pick up/drop the mug
                return;
            }
        }

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
                     && ((player.GetComponent<PlayerScript>().heldItem.CompareTag("Whole")) ||
                     (player.GetComponent<PlayerScript>().heldItem.CompareTag("Almond")) ||
                     (player.GetComponent<PlayerScript>().heldItem.CompareTag("Oat")));
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

        if (Input.GetKeyDown(KeyCode.C)) Debug.Log($"[Mug] contents=[{string.Join(",", contents)}]");
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
        if (other.gameObject.tag == "Almond")
        {
            bottle = other.gameObject;
        }
        if (other.gameObject.tag == "Oat")
        {
            bottle = other.gameObject;
        }
        if (other.gameObject.tag == "sugar")
        {
            sugar = other.gameObject;
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
        if (!bottle)
        {
            Debug.LogWarning("No bottle to pour from");
            return;
        }
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

    void AddSugar()
    {
        Debug.Log("added sugar");
        if (sugar.tag == "sugar")
        {
            contents[1] = 1;
        }
    }

}
