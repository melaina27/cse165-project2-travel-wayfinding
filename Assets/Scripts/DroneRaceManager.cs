using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneRaceManager : MonoBehaviour
{
    public Parse parser;
    public GameObject checkpointPrefab;
    public Transform drone;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI messageText;

    public float checkpointRadiusMeters = 30f / 3.28084f;
    public float countdownSeconds = 3f;

    private List<Vector3> checkpoints = new List<Vector3>();
    private List<GameObject> checkpointObjects = new List<GameObject>();
    private int currentIndex = 0;
    private float raceTime = 0f;
    private float countdown = 0f;
    private bool raceStarted = false;
    private bool raceFinished = false;
    private bool canMove = false;

    public bool CanMove => canMove && !raceFinished;

    void Start()
    {
        LoadCheckpoints();
        ResetToCheckpoint(0);
        StartCountdown();
    }

    void Update()
    {
        if (!raceStarted || raceFinished) return;

        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
            canMove = false;
            if (messageText) messageText.text = "Start in: " + Mathf.Ceil(countdown).ToString();

            if (countdown <= 0)
            {
                canMove = true;
                if (messageText) messageText.text = "Go!";
            }
            return;
        }

        raceTime += Time.deltaTime;
        if (timerText) timerText.text = "Time: " + raceTime.ToString("F2");

        CheckCheckpointReached();
    }

    void LoadCheckpoints()
    {
        if (parser == null)
        {
            Debug.LogError("Parser is not assigned.");
            return;
        }

        checkpoints = parser.ParseFile();

        for (int i = 0; i < checkpoints.Count; i++)
        {
            GameObject obj = Instantiate(checkpointPrefab, checkpoints[i], Quaternion.identity);
            obj.name = "Checkpoint_" + (i + 1);
            checkpointObjects.Add(obj);
        }
    }

    void CheckCheckpointReached()
    {
        if (currentIndex >= checkpoints.Count) return;

        float dist = Vector3.Distance(drone.position, checkpoints[currentIndex]);

        if (dist <= checkpointRadiusMeters)
        {
            if (checkpointObjects[currentIndex]) checkpointObjects[currentIndex].SetActive(false);
            currentIndex++;

            if (currentIndex >= checkpoints.Count)
            {
                raceFinished = true;
                canMove = false;
                if (messageText) messageText.text = "Finished! Final Time: " + raceTime.ToString("F2");
            }
            else
            {
                if (messageText) messageText.text = "Checkpoint reached!";
            }
        }
    }

    public Vector3 GetCurrentCheckpoint()
    {
        if (checkpoints.Count == 0) return Vector3.zero;
        if (currentIndex >= checkpoints.Count) return checkpoints[checkpoints.Count - 1];
        return checkpoints[currentIndex];
    }

    public void CrashReset()
    {
        int resetIndex = Mathf.Max(0, currentIndex - 1);
        ResetToCheckpoint(resetIndex);
        StartCountdown();
        if (messageText) messageText.text = "Crash! Resetting...";
    }

    void ResetToCheckpoint(int index)
    {
        if (checkpoints.Count == 0 || drone == null) return;

        drone.position = checkpoints[index] + Vector3.up * 2f;

        if (checkpoints.Count > 1)
        {
            Vector3 dir = checkpoints[Mathf.Min(index + 1, checkpoints.Count - 1)] - drone.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                drone.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }
    }

    void StartCountdown()
    {
        countdown = countdownSeconds;
        raceStarted = true;
        canMove = false;
    }
}
