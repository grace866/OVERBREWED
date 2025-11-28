using UnityEngine;

public class SugarScript : MonoBehaviour
{
    public float mugCheckRadius = 0.3f;   // how close to a mug counts as "over the mug"

    void Update()
    {
        // Only handle E when the cube is currently held (parented under the player’s holdPoint)
        if (transform.parent == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // If there’s a mug very close, let MugScript handle insertion & destroy us
            bool nearMug = false;
            var hits = Physics.OverlapSphere(transform.position, mugCheckRadius);
            foreach (var h in hits) { if (h.GetComponent<MugScript>() != null) { nearMug = true; break; } }
            if (nearMug) return;

            // Otherwise: drop to world (like your mugs)
            transform.SetParent(null, true);

            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if (TryGetComponent<Collider>(out var col)) col.enabled = true;
        }
    }
}