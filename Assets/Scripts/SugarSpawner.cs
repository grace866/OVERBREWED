using UnityEngine;

public class SugarSpawner : MonoBehaviour
{
    public GameObject sugarCubePrefab;  // assign your SugarCube prefab
    public Transform spawnPoint;        // empty child at jar mouth
    private GameObject player;
    private bool playerIsClose = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!playerIsClose || player == null) return;

        // Press E near the jar to spawn + hand the cube to the player
        if (Input.GetKeyDown(KeyCode.E))
        {
            var ps = player.GetComponent<PlayerScript>();
            if (ps != null && ps.CanHoldItem())
            {
                var cube = Instantiate(sugarCubePrefab, spawnPoint.position, spawnPoint.rotation);

                // hand it to the player (turn off physics while held)
                if (cube.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                if (cube.TryGetComponent<Collider>(out var col)) col.enabled = false;

                ps.HoldItem(cube);
            }
        }
    }

    // jar trigger
    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerIsClose = true; }
    void OnTriggerStay(Collider other)  { if (other.CompareTag("Player")) playerIsClose = true; }
    void OnTriggerExit(Collider other)  { if (other.CompareTag("Player")) playerIsClose = false; }
}