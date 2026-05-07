using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public DroneController droneController;
    public CheckpointManager checkpointManager;
    public Transform droneTransform;

    [Header("HUD")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI messageText;
    public Image flashPanel;

    [Header("State")]
    bool raceStarted = false;
    bool raceComplete = false;
    float elapsedTime = 0f;

    void Start()
    {
        // Initialize HUD
        countdownText.gameObject.SetActive(true);
        timerText.text = "0.00";
        messageText.text = "";
        SetFlashAlpha(0f);

        // Place drone at first checkpoint facing second
        InitializeDronePosition();

        // Start countdown
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (raceStarted && !raceComplete)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = FormatTime(elapsedTime);
        }
    }

    void InitializeDronePosition()
    {
        Vector3 startPos = checkpointManager.GetCheckpointPosition(0);
        Vector3 nextPos = checkpointManager.GetCheckpointPosition(1);

        droneTransform.position = startPos;

        // Face second checkpoint
        Vector3 direction = (nextPos - startPos).normalized;
        if (direction != Vector3.zero)
        {
            droneController.yaw = Mathf.Atan2(direction.x, direction.z)
                * Mathf.Rad2Deg;
            droneController.pitch = 0f;
        }

        // Sync XR Origin
        droneController.xrOrigin.position = startPos;
        droneController.xrOrigin.rotation = Quaternion.Euler(
            0, droneController.yaw, 0
        );
    }

    IEnumerator StartCountdown()
    {
        droneController.canFly = false;

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
        raceStarted = true;
        droneController.EnableFlight();
    }

    public void OnCheckpointReached(int index, int total)
    {
        ShowMessage($"Checkpoint {index}/{total}!", 2f);

        if (index >= total)
            OnRaceComplete();
    }

    void OnRaceComplete()
    {
        raceComplete = true;
        droneController.DisableFlight();
        timerText.color = Color.green;
        ShowMessage($"Race Complete! Time: {FormatTime(elapsedTime)}", 0f);
    }

    public void OnCrash(Vector3 respawnPosition)
    {
        if (raceComplete) return;
        StartCoroutine(CrashSequence(respawnPosition));
    }

    IEnumerator CrashSequence(Vector3 respawnPosition)
    {
        // Disable flight
        droneController.DisableFlight();

        // Flash screen red
        yield return StartCoroutine(FlashScreen());

        // Move drone back to last checkpoint
        droneTransform.position = respawnPosition;
        droneController.xrOrigin.position = respawnPosition;

        // Countdown before regaining control
        // Timer keeps running during this
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        droneController.EnableFlight();
    }

    IEnumerator FlashScreen()
    {
        // Fade in red
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetFlashAlpha(Mathf.Lerp(0f, 0.7f, elapsed / duration));
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetFlashAlpha(Mathf.Lerp(0.7f, 0f, elapsed / duration));
            yield return null;
        }

        SetFlashAlpha(0f);
    }

    void ShowMessage(string msg, float duration)
    {
        StopCoroutine("ClearMessage");
        messageText.text = msg;
        if (duration > 0)
            StartCoroutine(ClearMessage(duration));
    }

    IEnumerator ClearMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        messageText.text = "";
    }

    void SetFlashAlpha(float alpha)
    {
        if (flashPanel == null) return;
        Color c = flashPanel.color;
        c.a = alpha;
        flashPanel.color = c;
    }

    string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        float seconds = time % 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }
}
