using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachineScript : MonoBehaviour
{
    public GameObject player;
    public Slider progressBar;
    private bool isBrewing = false;
    private bool isCoffeeReady = false;
    private float brewingTime = 5f;
    private float brewingProgress = 0f;
    private bool isInRange = false;
    private bool isKeyHeld = false;

    public MugScript currentMug = null;
    [Header("Placement")]
    public Transform holdPoint; // point where mug is placed in the machine

    private void Start() {
        player = GameObject.FindWithTag("Player");
        if (progressBar != null)
        {
            progressBar.maxValue = brewingTime;
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }
    }

    private void Update()
    {
        // player is in range & holding mug, coffee isn't already brewing
        if (isInRange && !isBrewing && !isCoffeeReady && Input.GetKeyDown(KeyCode.E)) 
        {
                var mugInHand = player ? player.GetComponentInChildren<MugScript>() : null;
                if (mugInHand != null && SnapMugToMachine(mugInHand))
                {
                    StartBrewing();
                    isKeyHeld = true;
                }
                StartBrewing();
                isKeyHeld = true;
            
        }
        else if (isKeyHeld && Input.GetKeyUp(KeyCode.E))
        {
            StopBrewing();
            isKeyHeld = false;
        }
    }

    public void StartBrewing()
    {
        isBrewing = true;
        brewingProgress = 0f;
        progressBar.gameObject.SetActive(true);
        progressBar.value = 0f;
        StartCoroutine(BrewCoffee());
        // need to add snapping to machine

    }

    private void StopBrewing()
    {
        isBrewing = false;
        brewingProgress = 0f;
        progressBar.gameObject.SetActive(false);
    }

    private IEnumerator BrewCoffee()
    {
        while (brewingProgress < brewingTime)
        {
            brewingProgress += Time.deltaTime;
            progressBar.value = brewingProgress;
            yield return null;
        }

        isCoffeeReady = true;
        isBrewing = false;
        currentMug.ChangeMugColor(Color.red);
        progressBar.gameObject.SetActive(false);
        Debug.Log("Coffee is ready! Pick it up.");
        EjectMugFromMachine();
    }

    bool SnapMugToMachine(MugScript mug)
    {
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
        mug.transform.SetParent(holdPoint, worldPositionStays: false);
        mug.transform.localPosition = Vector3.zero;
        mug.transform.localRotation = Quaternion.identity;
        mug.transform.localScale = Vector3.one;

        currentMug = mug;
        return true;
    }

    // Unparent and restore physics so the player can pick it up again
    void EjectMugFromMachine()
    {
        if (!currentMug) return;

        currentMug.transform.SetParent(null, true);

        var rb = currentMug.GetComponent<Rigidbody>();
        var cols = currentMug.GetComponents<Collider>();

        foreach (var c in cols) c.enabled = true;

        if (rb)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // small downward nudge so it settles
            rb.linearVelocity = Vector3.down * 0.5f;
        }

        currentMug = null;
    }

    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                isInRange = true;
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