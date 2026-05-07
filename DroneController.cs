using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class DroneController : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;

    [Header("Flight Settings")]
    public float maxSpeed = 10f;
    public float deadZoneRadius = 0.05f;
    public float maxHandDistance = 0.3f;

    [Header("Rotation Settings")]
    public float maxRotationSpeed = 90f;
    public float rotationDeadZone = 0.05f;
    public float maxRotationDistance = 0.3f;

    [Header("State")]
    public bool canFly = false;

    XRHandSubsystem subsystem;

    // Left hand
    Vector3 leftNeutralPosition;
    bool leftNeutralSet = false;

    // Right hand
    Vector3 rightNeutralPosition;
    bool rightNeutralSet = false;

    // Drone heading
    public float yaw = 0f;
    public float pitch = 0f;

    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0)
        {
            subsystem = subsystems[0];
            if (!subsystem.running) subsystem.Start();
        }

        yaw = xrOrigin.eulerAngles.y;
        pitch = xrOrigin.eulerAngles.x;
    }

    void OnDestroy()
    {
        subsystem = null;
    }

    void Update()
    {
        if (subsystem == null) return;

        HandleLeftHand();
        HandleRightHand();
    }

    void HandleLeftHand()
    {
        if (subsystem == null || !subsystem.running) return;

        var leftHand = subsystem.leftHand;
        if (!leftHand.isTracked) return;

        if (!leftHand.GetJoint(XRHandJointID.Wrist)
            .TryGetPose(out Pose wristPose)) return;

        Vector3 currentHandPos = wristPose.position;

        if (!leftNeutralSet)
        {
            leftNeutralPosition = currentHandPos;
            leftNeutralSet = true;
        }

        if (!canFly) return;

        Vector3 offset = currentHandPos - leftNeutralPosition;
        Vector3 localVelocity = Vector3.zero;

        if (Mathf.Abs(offset.x) > deadZoneRadius)
            localVelocity.x = CalculateSpeed(offset.x, maxHandDistance);

        if (Mathf.Abs(offset.y) > deadZoneRadius)
            localVelocity.y = CalculateSpeed(offset.y, maxHandDistance);

        if (Mathf.Abs(offset.z) > deadZoneRadius)
            localVelocity.z = CalculateSpeed(offset.z, maxHandDistance);

        // Full 3D movement — pitch affects movement direction
        Vector3 worldVelocity = Quaternion.Euler(pitch, yaw, 0) * localVelocity;

        transform.position += worldVelocity * Time.deltaTime;

        if (xrOrigin != null)
            xrOrigin.position = transform.position;
    }

    void HandleRightHand()
    {
        if (subsystem == null || !subsystem.running) return;

        var rightHand = subsystem.rightHand;
        if (!rightHand.isTracked) return;

        if (!rightHand.GetJoint(XRHandJointID.Wrist)
            .TryGetPose(out Pose wristPose)) return;

        Vector3 currentHandPos = wristPose.position;

        if (!rightNeutralSet)
        {
            rightNeutralPosition = currentHandPos;
            rightNeutralSet = true;
        }

        if (!canFly) return;

        Vector3 offset = currentHandPos - rightNeutralPosition;

        // Left/right = yaw
        if (Mathf.Abs(offset.x) > rotationDeadZone)
        {
            float yawSpeed = CalculateSpeed(offset.x, maxRotationDistance);
            yaw += yawSpeed * maxRotationSpeed * Time.deltaTime;
        }

        // Up/down = pitch
        if (Mathf.Abs(offset.y) > rotationDeadZone)
        {
            float pitchSpeed = CalculateSpeed(-offset.y, maxRotationDistance);
            pitch += pitchSpeed * maxRotationSpeed * Time.deltaTime;
        }

        // Clamp pitch
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Apply rotation to XR Origin
        if (xrOrigin != null)
            xrOrigin.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    float CalculateSpeed(float offset, float maxDistance)
    {
        float sign = Mathf.Sign(offset);
        float absOffset = Mathf.Abs(offset) - deadZoneRadius;
        float normalizedDistance = Mathf.Clamp01(
            absOffset / (maxDistance - deadZoneRadius)
        );
        return sign * normalizedDistance * maxSpeed;
    }

    public void EnableFlight()
    {
        canFly = true;
    }

    public void DisableFlight()
    {
        canFly = false;
    }
}