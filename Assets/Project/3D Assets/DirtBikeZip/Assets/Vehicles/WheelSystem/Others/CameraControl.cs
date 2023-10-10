using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{
    public Transform target;
    public float distance = 10.0f;
    public float minDistance = 1.5f;
    public float maxDistance = 15f;

    public float maxSpeed = 0.3f;

    public float yMin = 0.0f;
    public float yMax = 80.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    private bool control = true;
    private float touchStartDistance;
    private Vector2 touchStartPosition;
    private Vector2 touchSpeed;
    [System.NonSerialized]
    public bool isFollow = false;
    [SerializeField] float zDistance = 5.0f;
    [SerializeField] float yDistance = 2.0f;

    private Vector3 zDisplacement;
    private Vector3 yDisplacement;
    private Vector3 targetPosition;

    [SerializeField] private float rotationDamping = 2.0f;
    [SerializeField] private float yDamping = 1.0f;
    private Quaternion lookRotation;

    void Start()
    {
        isFollow = InputOutput.isMobilePlatform;
        InputOutput.usedCamera = GetComponent<Camera>();
    }

    void LateUpdate ()
    {
        if (!target)
        {
            return;
        }
        if (isFollow)
        {
            zDisplacement = Vector3.Lerp(zDisplacement, Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized * zDistance, rotationDamping * Time.deltaTime);
            targetPosition = Vector3.Lerp(targetPosition, target.position, 5.0f * Time.deltaTime);
            Vector3 position = targetPosition - zDisplacement + Vector3.up * yDistance;
            Vector3 positionXZ = new Vector3(position.x, 0.0f, position.z);
            Vector3 deltaY = position - positionXZ;
            yDisplacement = Vector3.Lerp(yDisplacement, deltaY, yDamping * Time.deltaTime);
            lookRotation = Quaternion.LookRotation((target.position - transform.position).normalized);

            transform.position = positionXZ + yDisplacement;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationDamping * Time.deltaTime);
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
                control = false;
            else
                control = true;
            if (control)
            {
                if (InputOutput.inUsedCameraScreen)
                {
                    Vector3 mouseScreenSpeed = InputOutput.mouseScreenSpeed * Time.deltaTime;
                    x += mouseScreenSpeed.x * maxSpeed;
                    y -= mouseScreenSpeed.y * maxSpeed;

                    y = Mathf.Clamp(y, yMin, yMax);

                    Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
                    Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -distance)) + target.position;

                    transform.rotation = rotation;
                    transform.position = position;
                }
                distance += -300.0f * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
                if (distance < minDistance)
                    distance = minDistance;
                else if (distance > maxDistance)
                    distance = maxDistance;
            }
        }
    }
}
