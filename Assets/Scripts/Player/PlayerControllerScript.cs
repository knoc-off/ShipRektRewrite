using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    Rigidbody2D playerRB;
    public float Maxspeed = 1;
    public float speed;
    public float LandSpeed;
    public float MinVelocity = 1;
    public float stopDelay;
    public Transform cam;
    public Transform PlayerSprite;
    private int MoveMode = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        MoveMode = 1;
        playerRB.mass = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        cam.rotation = collision.transform.rotation; // delta of current player rotation and ship rotation.up down left & right?
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MoveMode = 0;
        playerRB.mass = 0.75f;

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        move = Vector2.ClampMagnitude(move, 1);
        Vector3 camF = cam.up;
        Vector3 camR = cam.right;

        camF = camF.normalized;
        camR = camR.normalized;

        playerRB.AddForce((camF * move.y + camR * move.x) * speed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = Vector2.ClampMagnitude(move, 1);

        if (MoveMode == 0)
        {
            Vector3 camF = cam.up;
            Vector3 camR = cam.right;

            camF = camF.normalized;
            camR = camR.normalized;

            playerRB.AddForce((camF * move.y + camR * move.x) * speed, ForceMode2D.Force);


        }
        else if(MoveMode == 1)
        {

            Vector3 camF = cam.up;
            Vector3 camR = cam.right;


            camF = camF.normalized;
            camR = camR.normalized;

            transform.position += (camF * move.y + camR * move.x) * Time.fixedDeltaTime * LandSpeed;




        }




        Vector3 dir = -move;
        float angle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg +90;
        angle += cam.transform.localRotation.eulerAngles.z; // relitive to camera





        if (move.sqrMagnitude > 0)
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
} 