using UnityEngine;

public class BottleScript : MonoBehaviour
{
    GameObject player;
    bool playerIsClose = false;
    public bool holdingBottle = false;

    //for bottle snapping
    public Transform bottleDropPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holdingBottle && playerIsClose)
            {
                PickupBottle();
            }
            else if (holdingBottle)
            {
                PutDownBottle();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !holdingBottle)
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

    void PickupBottle()
    {
        var ps = player.GetComponent<PlayerScript>();
        if (ps == null || !ps.CanHoldItem()) return;

        ps.HoldItem(gameObject);   // this snaps to holdPoint, the hand cube i added on Egg
        Debug.Log("holding bottle");
        holdingBottle = true;

        foreach (var c in GetComponents<Collider>()) c.enabled = false;

        playerIsClose = false;
    }

    void PutDownBottle()
    {
        transform.SetParent(null);

        if (bottleDropPoint != null)
        {
            transform.position = bottleDropPoint.position;
            transform.rotation = bottleDropPoint.rotation;
        }

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        foreach (var c in GetComponents<Collider>()) c.enabled = true; 

        var ps = player.GetComponent<PlayerScript>();
        if (ps) ps.DropItem();

        holdingBottle = false;
    }

}
