using UnityEngine;

public class BottleScript : MonoBehaviour
{
    //public MugScript currentMug = null;
    // Steps: pick up bottle, have bottle enter mug collider and press button, mug fills with bottle
    GameObject player;
    bool playerIsClose = false;
    //public GameObject bottleHeld = null;
    bool holdingBottle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
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
        if (other.gameObject.tag == "Player")
        {
            playerIsClose = true;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player")) playerIsClose = true;
    //}

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
        holdingBottle = true;
    }

    void PutDownBottle()
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

        holdingBottle = false;
    }
}
