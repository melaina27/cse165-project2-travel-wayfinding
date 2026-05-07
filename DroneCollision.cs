using UnityEngine;

public class DroneCollision : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    public GameManager gameManager;

    void Update()
    {
        checkpointManager.TryReachCheckpoint(transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Vector3 respawnPos = checkpointManager.GetCheckpointPosition(
                checkpointManager.currentCheckpointIndex == 0
                ? 0
                : checkpointManager.currentCheckpointIndex - 1
            );
            gameManager.OnCrash(respawnPos);
        }
    }
}