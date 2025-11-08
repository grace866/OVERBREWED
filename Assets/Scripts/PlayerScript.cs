using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// using System.Threading.Tasks.Dataflow;
using System.Xml;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Animator animator;
    CharacterController controller;

    private GameObject heldItem = null;
    public Transform holdPoint; // Assign an empty GameObject as the hand position

    public Transform playerBody;

    [Header("Movement")]
    public float speed = 3f;
    public float acceleration = 12f;
    public float decelerationFactor = 1f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    public float interactionRange = 3f; // Max distance for interaction
    public LayerMask interactableLayer; // Assign Interactable layer in Inspector
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lastHighlightedRenderer;

    //private IInteractable nearestInteractable;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = playerBody ? playerBody.GetComponent<Animator>() : null;
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        //DetectInteractable();
        //if (nearestInteractable != null && Input.GetKeyDown(KeyCode.E))
        //{
        //    nearestInteractable.Interact();
        //}
    }

    void Walk()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 horiz = input * speed;          // m/s

        if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime; // gravity < 0

        Vector3 disp = new Vector3(-horiz.x, velocity.y, -horiz.z) * Time.deltaTime; // meters this frame
        controller.Move(disp);

        if (input != Vector3.zero) playerBody.forward = -input;

        // float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput = Input.GetAxis("Vertical");
        // Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        // transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;

        // // velocity.y += gravity * Time.deltaTime;

        // // controller.Move((direction + velocity) * Time.deltaTime);


    }
    // void Walk()
    // {
    //     float horizontalInput = Input.GetAxis("Horizontal");
    //     float verticalInput = Input.GetAxis("Vertical");

    //     float maxSpeed = speed, maxAcc = acceleration;

    //     Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);


    //     float len = direction.magnitude;
    //     bool isMoving = len > 0.1f;

    //     // Update Animator parameter
    //     if (animator != null)
    //     {
    //         animator.SetBool("isWalking", isMoving);
    //     }

    //     if (controller.isGrounded && velocity.y < 0)
    //     {
    //         velocity.y = -2f; // small downward force to keep grounded
    //     }

    //     if (isMoving)
    //     {
    //         Vector3 moveDir = direction * speed;
    //         // controller.Move(moveDir * Time.deltaTime)
    //         velocity.x = direction.x * speed;
    //         velocity.z = direction.z * speed;
    //         // Face movement direction
    //         if (direction != Vector3.zero)
    //             playerBody.forward = direction;
    //     }

    //     // Apply gravity
    //     velocity.y += gravity * Time.deltaTime;
    //     controller.Move(velocity * Time.deltaTime);

    // }

    public bool CanHoldItem()
    {
        return heldItem == null; // Only hold one item at a time
    }

    public void HoldItem(GameObject item)
    {

        if (heldItem != null || !holdPoint) return;
        heldItem = item;

        if (item.TryGetComponent<Rigidbody>(out var rb)) //key idea is to get the mug and then set its on motion and whatnot to 0 and just snap it to the hand
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;                                   // no physics so it cant move on its on
            rb.detectCollisions = false;                             // and disable collider 
            rb.interpolation = RigidbodyInterpolation.None;          // important
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
        if (item.TryGetComponent<Collider>(out var col)) col.enabled = false;

        heldItem.transform.SetParent(holdPoint, worldPositionStays: false);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        //heldItem.transform.localScale = Vector3.one;
        Vector3 targetScale = Vector3.one;
        if (item.GetComponent<BottleScript>() != null)
        {
            targetScale = new Vector3(6f, 6f, 6f);
        }
        else if (item.GetComponent<MugScript>() != null)
        {
            targetScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        heldItem.transform.localScale = targetScale;
    }

    /*public ItemScript GetItemHeld()
    {
        if (heldItem != null)
        {
            ItemScript objectInfo = heldItem.GetComponent<ItemScript>();
            if (objectInfo != null)
            {
                return objectInfo;
            }
            else
            {
                Debug.LogWarning("Held item does not have an iteminfo component!");
            }
        }
        return null;
    }*/

    public void DropItem()
    {

        if (!heldItem) return;

        heldItem.transform.SetParent(null, true);

        if (heldItem.TryGetComponent<Rigidbody>(out var rb)) // after dropping give back the kinetic motions to the object and allow for it to move on its own
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;   
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        if (heldItem.TryGetComponent<Collider>(out var col)) col.enabled = true;

        heldItem = null;
    }

    void Highlighting(Renderer objRenderer)
    {
        if (objRenderer != null)
        {
            // Restore the previous highlighted object if it's different
            if (lastHighlightedRenderer != null && lastHighlightedRenderer != objRenderer)
            {
                lastHighlightedRenderer.material = originalMaterial;
            }

            // Store original material & apply highlight
            if (lastHighlightedRenderer != objRenderer)
            {
                originalMaterial = objRenderer.material;
            }
            objRenderer.material = highlightMaterial;
            lastHighlightedRenderer = objRenderer;
            return;
        }

        // Restore the material if no object is highlighted
        if (lastHighlightedRenderer != null)
        {
            lastHighlightedRenderer.material = originalMaterial;
            lastHighlightedRenderer = null;
        }
    }

    //void DetectInteractable()
    //{
    //    RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, interactionRange, interactableLayer);
    //    float minDistance = float.MaxValue;
    //    IInteractable closest = null;
    //    Renderer closestRenderer = null;

    //    foreach (var hit in hits)
    //    {
    //        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
    //        Renderer renderer = hit.collider.GetComponent<Renderer>();
    //        if (interactable != null)
    //        {
    //            float distance = Vector3.Distance(transform.position, hit.point);
    //            if (distance < minDistance)
    //            {
    //                minDistance = distance;
    //                closest = interactable;
    //                closestRenderer = renderer;
    //            }
    //        }
    //    }

    //    nearestInteractable = closest;
    //    Highlighting(closestRenderer);
    //}
}


