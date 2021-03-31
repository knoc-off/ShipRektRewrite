using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    Rigidbody2D playerRB;
    public float WaterSpeed;
    public float LandSpeed;
    public Camera cam;
    public Transform PlayerSprite;

    private int MoveMode = 0;

    public bool OnShip = false;
    public bool rotate = false;


    private float CameraLeaveSize = 8f;
    private float CameraEnterSize = 7f;

    private float EnterAngle;
    private float ShipDegreeOffset;
    private float EnterTime;
    private float LeaveTime;
    public float RotateCamSpeed = 1.0f;


    private float TimeSinceStay;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!OnShip)
        {
            cam.orthographicSize = Mathf.Lerp(CameraLeaveSize, 7f, (Time.realtimeSinceStartup - LeaveTime) * RotateCamSpeed) ; // delta of current player rotation and ship rotation.up down left & right?

            float rot = Input.GetAxis("RotateCamera");
            if (Input.GetButton("RotateCamera") && (Time.realtimeSinceStartup - EnterTime) > .50f)
            {
                float temprot = cam.transform.rotation.eulerAngles.z;
                temprot += rot < 0 ? -.5f : .5f;
                cam.transform.rotation = Quaternion.Euler(0,0,temprot);
            }
            
        }
        else
        {
            if(cam.orthographicSize != 8f)
                cam.orthographicSize = Mathf.Lerp(CameraEnterSize, 8f, (Time.realtimeSinceStartup - EnterTime) * RotateCamSpeed);
        }

        if (Time.realtimeSinceStartup - TimeSinceStay > 1)
        {
            MoveMode = 0;
            playerRB.mass = 0.75f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CameraEnterSize = cam.orthographicSize;
        OnShip = true;
        // Set timer Start
        MoveMode = 1;
        playerRB.mass = 0;

        EnterTime = Time.realtimeSinceStartup;
        EnterAngle = cam.transform.rotation.eulerAngles.z;
        ShipDegreeOffset = collision.transform.eulerAngles.z;



        //print("cam: " + EnterAngle + ", ship: " + ShipDegreeOffset);
        float temp = 45;
        int lowestIter = 0;
        var positive = 1;
        if (EnterAngle > 0)
            positive = -1;


        for (var i = 0; i < 4; i++) // Rotate to the nearest 90 degrees then save as offset -- Works! 
        {
            if (temp > Mathf.Abs(Mathf.DeltaAngle(EnterAngle, ShipDegreeOffset + (90 * i * positive))))
            {
                //print("player is in between: " + (90 * i * positive) + " and " + ((90 * i * positive) + (90 * positive)));
                temp = Mathf.Abs(Mathf.DeltaAngle(EnterAngle, ShipDegreeOffset + (90 * i * positive)));
                lowestIter = i;
            }
        }
        ShipDegreeOffset = lowestIter * 90 * positive;
        //print("cam: " + EnterAngle + ", ship: " + ShipDegreeOffset);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnShip = true;
        if (Mathf.Abs(Mathf.DeltaAngle(EnterAngle, collision.transform.eulerAngles.z + ShipDegreeOffset)) > 0.5)
            rotate = true;
        else
            rotate = false;

        LeaveTime = Time.realtimeSinceStartup;
        float rot = Input.GetAxis("RotateCamera");
        if (Input.GetButton("RotateCamera") && (Time.realtimeSinceStartup - EnterTime) > .50f)
        {
            EnterAngle = cam.transform.rotation.eulerAngles.z;
            EnterTime = Time.realtimeSinceStartup;
            ShipDegreeOffset += rot < 0 ? -90 : 90;
        }
        TimeSinceStay = Time.realtimeSinceStartup;
        MoveMode = 1;
        playerRB.mass = 0;
        // For time.fixedtime - setTime Mathf.Learp angle from/ to
        //print(Time.realtimeSinceStartup - EnterTime);
        //Quaternion setRotation = new Quaternion();
        //setRotation.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(EnterAngle, collision.transform.rotation.z, Time.realtimeSinceStartup - EnterTime));
        cam.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.LerpAngle(EnterAngle, collision.transform.eulerAngles.z + ShipDegreeOffset, (Time.realtimeSinceStartup - EnterTime) * RotateCamSpeed))); // delta of current player rotation and ship rotation.up down left & right?
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CameraLeaveSize = cam.orthographicSize;
        LeaveTime = Time.realtimeSinceStartup;
        OnShip = false;
        MoveMode = 0;
        playerRB.mass = 0.75f;

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        move = Vector2.ClampMagnitude(move, 1);
        Vector3 camF = cam.transform.up;
        Vector3 camR = cam.transform.right;

        camF = camF.normalized;
        camR = camR.normalized;

        playerRB.AddForce((camF * move.y + camR * move.x) * WaterSpeed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = Vector2.ClampMagnitude(move, 1);

        if (MoveMode == 0)
        {
            Vector3 camF = cam.transform.up;
            Vector3 camR = cam.transform.right;

            camF = camF.normalized;
            camR = camR.normalized;

            playerRB.AddForce((camF * move.y + camR * move.x) * WaterSpeed, ForceMode2D.Force);


        }
        else if (MoveMode == 1)
        {

            Vector3 camF = cam.transform.up;
            Vector3 camR = cam.transform.right;


            camF = camF.normalized;
            camR = camR.normalized;

            transform.position += (camF * move.y + camR * move.x) * Time.fixedDeltaTime * LandSpeed;




        }




        Vector3 dir = -move;
        float angle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg + 90;
        angle += cam.transform.localRotation.eulerAngles.z; // relitive to camera





        if (move.sqrMagnitude > 0)
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}