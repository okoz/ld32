using UnityEngine;
using System.Collections;

/// <summary>
/// Controller that can pan the camera about the Y axis of a target.
/// </summary>
public class TargetPanningCamera : MonoBehaviour
{
    public Transform Target;
    public float PanSpeed;

    private float panAmount;

	void Start()
    {
	
	}
	
	void Update()
    {
        panAmount = Input.GetAxis("Horizontal");
	}

    void LateUpdate()
    {
        transform.LookAt(Target);

        if(Target)
        {
            transform.RotateAround(Target.transform.position, Vector3.up, PanSpeed * panAmount);
        }
    }
}
