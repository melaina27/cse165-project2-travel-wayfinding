using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class HandTrackingDiagnostics : MonoBehaviour
{
    XRHandSubsystem subsystem;
    float logInterval = 2f;
    float timer = 0f;

    void Start()
    {
        Debug.Log("=== HAND TRACKING DIAGNOSTICS START ===");

        // Check subsystem
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        Debug.Log($"[Subsystem] Count: {subsystems.Count}");

        if (subsystems.Count == 0)
        {
            Debug.LogError("[Subsystem] NO XRHandSubsystem found. Check XR Plug-in Management -> OpenXR -> Hand Tracking Subsystem is enabled on PC tab.");
            return;
        }

        subsystem = subsystems[0];
        Debug.Log($"[Subsystem] Found: {subsystem != null}");
        Debug.Log($"[Subsystem] Running: {subsystem.running}");

        // Try manual start
        if (!subsystem.running)
        {
            Debug.LogWarning("[Subsystem] Not running, attempting manual Start()...");
            subsystem.Start();
            Debug.Log($"[Subsystem] Running after manual Start(): {subsystem.running}");
        }

        // Check XR Settings
        Debug.Log($"[XR] Active loader: {UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader?.name ?? "NULL"}");

        // Check HandVisualizer objects in scene
        var leftHand = GameObject.Find("Left Hand Tracking");
        var rightHand = GameObject.Find("Right Hand Tracking");
        Debug.Log($"[Scene] Left Hand Tracking GameObject exists: {leftHand != null}");
        Debug.Log($"[Scene] Right Hand Tracking GameObject exists: {rightHand != null}");

        if (leftHand != null)
        {
            Debug.Log($"[Scene] Left Hand active: {leftHand.activeInHierarchy}");
            var meshController = leftHand.GetComponentInChildren<SkinnedMeshRenderer>();
            Debug.Log($"[Scene] Left SkinnedMeshRenderer found: {meshController != null}");
            if (meshController != null)
            {
                Debug.Log($"[Scene] Left SMR enabled: {meshController.enabled}");
                Debug.Log($"[Scene] Left SMR material: {meshController.material?.name ?? "NULL"}");
                Debug.Log($"[Scene] Left SMR bounds center: {meshController.bounds.center}");
            }
        }

        if (rightHand != null)
        {
            Debug.Log($"[Scene] Right Hand active: {rightHand.activeInHierarchy}");
            var meshController = rightHand.GetComponentInChildren<SkinnedMeshRenderer>();
            Debug.Log($"[Scene] Right SkinnedMeshRenderer found: {meshController != null}");
            if (meshController != null)
            {
                Debug.Log($"[Scene] Right SMR enabled: {meshController.enabled}");
                Debug.Log($"[Scene] Right SMR material: {meshController.material?.name ?? "NULL"}");
                Debug.Log($"[Scene] Right SMR bounds center: {meshController.bounds.center}");
            }
        }

        // Check camera
        var cam = Camera.main;
        Debug.Log($"[Camera] Main camera found: {cam != null}");
        if (cam != null)
        {
            Debug.Log($"[Camera] Position: {cam.transform.position}");
            Debug.Log($"[Camera] Culling mask: {cam.cullingMask}");
            Debug.Log($"[Camera] Near clip: {cam.nearClipPlane}");
            Debug.Log($"[Camera] Far clip: {cam.farClipPlane}");
        }

        Debug.Log("=== DIAGNOSTICS INIT COMPLETE ===");
    }

    void Update()
    {
        if (subsystem == null) return;

        timer += Time.deltaTime;
        if (timer < logInterval) return;
        timer = 0f;

        Debug.Log("=== HAND TRACKING UPDATE ===");
        Debug.Log($"[Subsystem] Still running: {subsystem.running}");

        // Left hand
        var left = subsystem.leftHand;
        Debug.Log($"[Left] isTracked: {left.isTracked}");
        if (left.isTracked)
        {
            LogHandJoints("Left", left);
        }

        // Right hand
        var right = subsystem.rightHand;
        Debug.Log($"[Right] isTracked: {right.isTracked}");
        if (right.isTracked)
        {
            LogHandJoints("Right", right);
        }

        // Check if SMR bounds moved (are hands rendering somewhere?)
        var leftObj = GameObject.Find("Left Hand Tracking");
        var rightObj = GameObject.Find("Right Hand Tracking");
        if (leftObj != null)
        {
            var smr = leftObj.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
                Debug.Log($"[Left SMR] enabled: {smr.enabled} | bounds center: {smr.bounds.center} | isVisible: {smr.isVisible}");
        }
        if (rightObj != null)
        {
            var smr = rightObj.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
                Debug.Log($"[Right SMR] enabled: {smr.enabled} | bounds center: {smr.bounds.center} | isVisible: {smr.isVisible}");
        }
    }

    void LogHandJoints(string handName, XRHand hand)
    {
        // Log key joints only
        XRHandJointID[] keyJoints = {
            XRHandJointID.Wrist,
            XRHandJointID.Palm,
            XRHandJointID.IndexTip,
            XRHandJointID.ThumbTip
        };

        foreach (var jointID in keyJoints)
        {
            if (hand.GetJoint(jointID).TryGetPose(out Pose pose))
                Debug.Log($"[{handName}] {jointID}: pos={pose.position} rot={pose.rotation.eulerAngles}");
            else
                Debug.LogWarning($"[{handName}] {jointID}: FAILED to get pose");
        }
    }
}
