using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    private bool isOpened;
    private float angle;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                isOpened = !isOpened;
                angle = isOpened ? 40.0f : 0.0f;
            }
        }
        transform.localRotation = Quaternion.Lerp( transform.localRotation, Quaternion.Euler(0.0f, 0.0f, angle), 5.0f * Time.deltaTime);
    }
}
