using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class XRHandsStarter : MonoBehaviour
{
    void Awake()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0 && !subsystems[0].running)
        {
            subsystems[0].Start();
            Debug.Log("XRHandSubsystem started in Awake");
        }
    }
}