using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachineScript : MonoBehaviour
{
    public GameObject player;
    //public Slider progressBar;
    private bool isBrewing = false;
    private bool isCoffeeReady = false;
    private float brewingTime = 5f;
    private float brewingProgress = 0f;
    private bool isInRange = false;
    private bool isKeyHeld = false;
    public Renderer milkRenderer;

    public MugScript currentMug = null;
    public MeshRenderer liquid = null;
   
   
    [Header("Placement")]
    public Transform holdPoint; // point where mug is placed in the machine

    private void Start() {
        player = GameObject.FindWithTag("Player");
        //if (progressBar != null)
        //{
        //    progressBar.maxValue = brewingTime;
        //    progressBar.gameObject.SetActive(false);
        //    progressBar.value = 0f;
        //}
    }

    private void Update()
    {
        // player is in range & holding mug, coffee isn't already brewing
        if (isInRange && !isBrewing && !isCoffeeReady && Input.GetKeyDown(KeyCode.B) && player.GetComponentInChildren<MugScript>() != null) 
        {
            Debug.Log("in range with mug");
            var mugInHand = player.GetComponentInChildren<MugScript>();
            Debug.Log(mugInHand);
            Debug.Log(SnapMugToMachine(mugInHand));
            if (mugInHand != null && SnapMugToMachine(mugInHand))
            {
                StartBrewing();
                isKeyHeld = true;
            }
            
        }
        else if (isKeyHeld && Input.GetKeyUp(KeyCode.B))
        {
            StopBrewing();
            isKeyHeld = false;

        }
    }

    public void StartBrewing()
    {
        Debug.Log("brewing started");
        isBrewing = true;
        brewingProgress = 0f;
        //progressBar.gameObject.SetActive(true);
        //progressBar.value = 0f;
        StartCoroutine(BrewCoffee());
        // need to add snapping to machine

    }

    private void StopBrewing()
    {
        isBrewing = false;
        brewingProgress = 0f;
        //progressBar.gameObject.SetActive(false);
    }

    private IEnumerator BrewCoffee()
    {
        Debug.Log("in ienumerator");
        while (brewingProgress < brewingTime)
        {
            brewingProgress += Time.deltaTime;
            //progressBar.value = brewingProgress;
            yield return null;
        }
        isCoffeeReady = true;
        isBrewing = false;
        //currentMug.ChangeMugColor(Color.white);
        //progressBar.gameObject.SetActive(false);
        Debug.Log("Coffee is ready! Pick it up.");
        EjectMugFromMachine();
    }

    bool SnapMugToMachine(MugScript mug)
    {
        Debug.Log("in snapmugtomachine");
        // If the player script tracks held item, drop it first so it’s no longer parented to the hand
        var ps = player ? player.GetComponent<PlayerScript>() : null;
        if (ps) ps.DropItem();

        // Turn off physics while in machine
        var rb = mug.GetComponent<Rigidbody>();
        var cols = mug.GetComponents<Collider>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
        foreach (var c in cols) c.enabled = false;

        // Parent & snap
        mug.transform.SetParent(holdPoint, worldPositionStays: true);
        mug.transform.localPosition = Vector3.zero;
        mug.transform.localRotation = Quaternion.identity;
        Debug.Log($"Snapped {mug.name} to {holdPoint.name} at {holdPoint.position}");
        Debug.Log($"Mug in hand? {player.GetComponentInChildren<MugScript>() != null}");

        //mug.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        currentMug = mug;
        liquid = currentMug.transform.GetChild(1).GetComponent<MeshRenderer>();
        
        return true;
    }

    // Unparent and restore physics so the player can pick it up again
    void EjectMugFromMachine()
    {
        if (!currentMug) return;

        //Vector3 targetScale = Vector3.one;
        //targetScale = new Vector3(0.8f, 0.8f, 0.8f);
        //currentMug.transform.localScale = targetScale;
        currentMug.transform.SetParent(null);
        
        var rb = currentMug.GetComponent<Rigidbody>();
        var cols = currentMug.GetComponents<Collider>();

        Vector3 ejectionDirection = holdPoint.forward;
        float ejectionForce = 3.0f; // Adjust this value to control how far it pushes

        foreach (var c in cols) c.enabled = true;

        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = ejectionDirection * ejectionForce;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // small downward nudge so it settles
            //rb.linearVelocity = Vector3.down * 0.5f;
        }

        /*if (milkRenderer != null)
        {
            milkRenderer.material.color = 
        }*/

        liquid.material.color = new Color(0.31f, 0.216f, 0.2f, 1);
        currentMug = null;
    }

    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Mug")
            {
                isInRange = true;
                Debug.Log("is in range");
        }
            // currentMug = player.GetComponentInChildren<MugScript>();
        }


    private void OnTriggerStay(Collider other) // helps when colliders re-enable while overlapping
    {
        if (other.CompareTag("Player")) isInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInRange = false;
        }
    }
}