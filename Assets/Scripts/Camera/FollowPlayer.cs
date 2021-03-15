using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform player;

    public Camera Cam;

    public float SmoothSpeed = 0.125f;

    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10) + offset;
    }

    void FixedUpdate()
    {
        var temp = Vector3.SmoothDamp(transform.position, player.position + offset, ref velocity, SmoothSpeed) + offset; //player.position + offset;
        temp.z = -10;
        transform.position = temp;
    }
}
