using UnityEngine;

public class DroneHandFlightController : MonoBehaviour
{
    public DroneRaceManager raceManager;

    public Transform leftHand;
    public Transform rightHand;

    public float moveSpeed = 12f;
    public float turnSpeed = 80f;
    public float verticalSpeed = 8f;

    void Update()
    {
        if (raceManager != null && !raceManager.CanMove) return;

        Vector3 move = Vector3.zero;
        float turn = 0f;

        // For computer testing only.
        // Final demo should use hand tracking.
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) turn -= 1f;
        if (Input.GetKey(KeyCode.D)) turn += 1f;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) move -= Vector3.up;

        if (rightHand != null)
        {
            move += rightHand.forward;
        }

        if (leftHand != null && rightHand != null)
        {
            float heightDiff = rightHand.position.y - leftHand.position.y;
            move += Vector3.up * Mathf.Clamp(heightDiff * 2f, -1f, 1f);
        }

        if (move.sqrMagnitude > 0.001f)
            transform.position += move.normalized * moveSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
    }
}
