using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    public DroneRaceManager raceManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain") || other.CompareTag("Building"))
        {
            if (raceManager != null)
                raceManager.CrashReset();
        }
    }
}
