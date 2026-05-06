using UnityEngine;
using TMPro;

public class WaypointArrow : MonoBehaviour
{
    public DroneRaceManager raceManager;
    public Transform drone;
    public TextMeshProUGUI distanceText;

    void Update()
    {
        if (raceManager == null || drone == null) return;

        Vector3 target = raceManager.GetCurrentCheckpoint();
        Vector3 dir = target - drone.position;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.position = drone.position + Vector3.up * 2f + drone.forward * 3f;
            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }

        if (distanceText != null)
        {
            distanceText.text = "Next: " + dir.magnitude.ToString("F1") + " m";
        }
    }
}
