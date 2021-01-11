using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringTest : MonoBehaviour
{
    public TankShell shell;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shell.FireShell(transform.position, -transform.right);
            Debug.DrawLine(transform.position, transform.position - transform.right * 2f, Color.yellow);
        }    
    }
}
