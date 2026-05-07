using UnityEngine;
using UnityEngine.UI;

public class WayfindingManager : MonoBehaviour
{
    [Header("References")]
    public CheckpointManager checkpointManager;
    public Transform droneTransform;
    public Transform headCamera;

    [Header("Aid 1 - HUD Arrow (Head Coordinates)")]
    public RectTransform arrowImage;

    [Header("Arrow Vertical Settings")]
    public float maxVerticalOffset = 150f; // max pixels up/down on HUD
    public float maxVerticalDistance = 50f; // world distance that maps to max offset

    [Header("Aid 2 - World Beam")]
    public LineRenderer beamRenderer;

    void Start()
    {
        beamRenderer.startWidth = 0.3f;
        beamRenderer.endWidth = 0.3f;
        beamRenderer.positionCount = 2;
        beamRenderer.material = new Material(Shader.Find("Unlit/Color"));
        beamRenderer.material.color = new Color(1f, 1f, 0f, 0.6f);
    }

    void Update()
    {
        if (checkpointManager.IsRaceComplete())
        {
            arrowImage.gameObject.SetActive(false);
            beamRenderer.enabled = false;
            return;
        }

        Vector3 nextCheckpoint =
            checkpointManager.GetCurrentCheckpointPosition();

        UpdateHUDArrow(nextCheckpoint);
        UpdateBeam(nextCheckpoint);
    }

    void UpdateHUDArrow(Vector3 targetPosition)
    {
        // Direction from drone to checkpoint
        Vector3 worldDirection =
            (targetPosition - droneTransform.position).normalized;

        // Convert to head local space
        Vector3 localDirection =
            headCamera.InverseTransformDirection(worldDirection);

        // Horizontal rotation — spins arrow like compass
        float angle = Mathf.Atan2(localDirection.x, localDirection.y)
            * Mathf.Rad2Deg;
        arrowImage.localRotation = Quaternion.Euler(0, 0, -angle);

        // Vertical position — moves arrow up/down based on Y distance
        float verticalDiff = targetPosition.y - droneTransform.position.y;
        float normalizedVertical = Mathf.Clamp(
            verticalDiff / maxVerticalDistance, -1f, 1f
        );
        float verticalPixels = normalizedVertical * maxVerticalOffset;

        arrowImage.anchoredPosition = new Vector2(0, verticalPixels);
    }

    void UpdateBeam(Vector3 targetPosition)
    {
        beamRenderer.SetPosition(0, droneTransform.position);
        beamRenderer.SetPosition(1, targetPosition);
    }
}