using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipSync : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("Test Print");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.name);
        
    }
    void OnCollisionEnter(Collision collision)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
