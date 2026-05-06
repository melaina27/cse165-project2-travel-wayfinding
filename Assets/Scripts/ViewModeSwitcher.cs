using UnityEngine;

public class ViewModeSwitcher : MonoBehaviour
{
    public Camera mainCamera;

    public Transform firstPersonView;
    public Transform cockpitView;
    public Transform thirdPersonView;

    public GameObject cockpitVisual;
    public GameObject droneVisual;

    private int mode = 0;

    void Update()
    {
        // For computer testing only.
        // Later we can connect this to a hand gesture.
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchMode();
        }
    }

    public void SwitchMode()
    {
        mode = (mode + 1) % 3;

        if (mode == 0) SetView(firstPersonView, false, false);
        if (mode == 1) SetView(cockpitView, true, false);
        if (mode == 2) SetView(thirdPersonView, false, true);
    }

    void SetView(Transform target, bool showCockpit, bool showDrone)
    {
        if (mainCamera == null || target == null) return;

        mainCamera.transform.SetParent(target);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;

        if (cockpitVisual) cockpitVisual.SetActive(showCockpit);
        if (droneVisual) droneVisual.SetActive(showDrone);
    }
}
