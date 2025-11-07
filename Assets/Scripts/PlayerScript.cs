using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{

    Animator animator;
    CharacterController controller;

    public Transform playerBody;

    [Header("Movement")]
    public float speed = 3f;
    public float acceleration = 12f;
    public float decelerationFactor = 1f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    private void Start()
    {
        controller = playerBody.GetComponent<CharacterController>();
        animator = playerBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
    }

    void Walk()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float maxSpeed = speed, maxAcc = acceleration;

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);


        float len = direction.magnitude;
        bool isMoving = len > 0.1f;

        // Update Animator parameter
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to keep grounded
        }

        if (isMoving)
        {
            Vector3 moveDir = direction * speed;
            controller.Move(moveDir * Time.deltaTime);

            // Face movement direction
            if (direction != Vector3.zero)
                playerBody.forward = direction;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHold : MonoBehaviour
{
    private GameObject heldItem = null;
    public Transform holdPoint; // Assign an empty GameObject as the hand position

    public bool CanHoldItem()
    {
        return heldItem == null; // Only hold one item at a time
    }

    public void HoldItem(GameObject item)
    {
        if (heldItem == null)
        {
            heldItem = item;
            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }
    }

    public ItemScript GetItemHeld()
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
    }

    public void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // Max distance for interaction
    public LayerMask interactableLayer; // Assign Interactable layer in Inspector
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lastHighlightedRenderer;

    private IInteractable nearestInteractable;
    //public Transform playerBody;

    void Update()
    {
        DetectInteractable();
        if (nearestInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            nearestInteractable.Interact();
        }
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

    void DetectInteractable()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, interactionRange, interactableLayer);
        float minDistance = float.MaxValue;
        IInteractable closest = null;
        Renderer closestRenderer = null;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = interactable;
                    closestRenderer = renderer;
                }
            }
        }

        nearestInteractable = closest;
        Highlighting(closestRenderer);
    }
}



