using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    [Header("Setup")]
    public Parse parser;
    public GameObject checkpointPrefab;
    public GameManager gameManager;

    [Header("State")]
    public int currentCheckpointIndex = 0;
    List<Vector3> checkpointPositions = new List<Vector3>();
    List<GameObject> checkpointObjects = new List<GameObject>();

    const float CHECKPOINT_RADIUS = 9.144f;

    void Start()
    {
        LoadTrack();
    }

    public void LoadTrack()
    {
        foreach (var cp in checkpointObjects)
            Destroy(cp);
        checkpointObjects.Clear();
        currentCheckpointIndex = 0;

        checkpointPositions = parser.ParseFile();

        if (checkpointPositions.Count == 0)
        {
            Debug.LogError("No checkpoints loaded!");
            return;
        }

        SpawnCheckpoints();
    }

    void SpawnCheckpoints()
    {
        for (int i = 0; i < checkpointPositions.Count; i++)
        {
            GameObject cp = Instantiate(
                checkpointPrefab,
                checkpointPositions[i],
                Quaternion.identity
            );
            cp.name = $"Checkpoint_{i + 1}";
            checkpointObjects.Add(cp);
            SetCheckpointAlpha(cp, i == 0 ? 0.6f : 0.15f);
        }
    }

    void SetCheckpointAlpha(GameObject cp, float alpha)
    {
        Renderer r = cp.GetComponent<Renderer>();
        if (r == null) return;
        Color c = r.material.color;
        c.a = alpha;
        r.material.color = c;
    }

    public bool TryReachCheckpoint(Vector3 dronePosition)
    {
        if (currentCheckpointIndex >= checkpointPositions.Count)
            return false;

        float dist = Vector3.Distance(
            dronePosition,
            checkpointPositions[currentCheckpointIndex]
        );

        if (dist <= CHECKPOINT_RADIUS)
        {
            OnCheckpointReached();
            return true;
        }
        return false;
    }

    void OnCheckpointReached()
    {
        Debug.Log($"Checkpoint {currentCheckpointIndex + 1} reached!");
        SetCheckpointAlpha(checkpointObjects[currentCheckpointIndex], 0.05f);
        currentCheckpointIndex++;

        // Notify GameManager
        if (gameManager != null)
            gameManager.OnCheckpointReached(
                currentCheckpointIndex,
                checkpointPositions.Count
            );

        if (currentCheckpointIndex < checkpointPositions.Count)
        {
            SetCheckpointAlpha(checkpointObjects[currentCheckpointIndex], 0.6f);
            Debug.Log($"Next: Checkpoint {currentCheckpointIndex + 1}");
        }
        else
        {
            Debug.Log("Race complete!");
        }
    }

    public Vector3 GetCheckpointPosition(int index)
    {
        if (index < checkpointPositions.Count)
            return checkpointPositions[index];
        return Vector3.zero;
    }

    public Vector3 GetCurrentCheckpointPosition()
    {
        if (currentCheckpointIndex < checkpointPositions.Count)
            return checkpointPositions[currentCheckpointIndex];
        return Vector3.zero;
    }

    public bool IsRaceComplete()
    {
        return currentCheckpointIndex >= checkpointPositions.Count;
    }
}