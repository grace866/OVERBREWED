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

    void Start()
    {
        contents = new List<int> { 0, 0 };
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

    public void addOatToMug()
    {
        Debug.Log(contents.Count);
        if (this != null && contents != null)
        {
            if (contents[1] != 2)
            {
                Debug.Log("milk amount: " + contents[0] + " and type of milk: " + contents[1]);
                contents[1] = 2;
                contents[0] = 1;
            }
            else
            {
                Debug.Log("incremented amount");
                contents[0]++;
            }
        }
        else
        {
            Debug.Log("not holding mug");
        }

    }

    public void addAlmondToMug()
    {
        Debug.Log(contents.Count);
        if (this != null && contents != null)
        {
            if (contents[1] != 1)
            {
                Debug.Log("milk amount: " + contents[0] + " and type of milk: " + contents[1]);
                contents[1] = 1;
                contents[0] = 1;
            }
            else
            {
                Debug.Log("incremented amount");
                contents[0]++;
            }
        }
        else
        {
            Debug.Log("not holding mug");
        }

    }

    public void addWholeToMug()
    {
        Debug.Log(contents.Count);
        if (this != null && contents != null)
        {
            if (contents[1] != 0)
            {
                Debug.Log("milk amount: " + contents[0] + " and type of milk: " + contents[1]);
                contents[1] = 0;
                contents[0] = 1;
            }
            else
            {
                Debug.Log("incremented amount");
                contents[0]++;
            }
        }
        else
        {
            Debug.Log("not holding mug");
        }

    }

}
