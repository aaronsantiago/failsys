using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireInputManager : MonoBehaviour
{

	public float armsFiringDuration = .5f;
	public float armsFiringRemaining = 0f;
	public float clawsSparkingDuration = .5f;
	public float clawsSparkingRemaining = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FireController fc = GetComponent<FireController>();
        if (Input.GetKeyDown("z"))
        {
        	armsFiringRemaining = armsFiringDuration; 
        }
        if (Input.GetKeyDown("x"))
        {
        	clawsSparkingRemaining = clawsSparkingDuration; 
        }
        armsFiringRemaining = armsFiringRemaining - Time.deltaTime;
        if (armsFiringRemaining > 0) {
        	fc.armsFiring = true;
        }
        else {
        	fc.armsFiring = false;
        }
        clawsSparkingRemaining = clawsSparkingRemaining - Time.deltaTime;
        if (clawsSparkingRemaining > 0) {
        	fc.clawsSparking = true;
        }
        else {
        	fc.clawsSparking = false;
        }
    }
}
