using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGameSystem : MonoBehaviour
{

	public float fireLife = 0;
	public float fireLifeDelta = -1f;
	public float fireLifeShotDelta = 3f;
	public float fireOverLife = 5f;
	public float fireAliveSmokeThreshold = 5f;

	public float fireSpeedMult = .4f;

	public float fireAliveTime = 0;
	public float smokeLife = 0;
	public float smokeLifeNoFireDelta = -3f;
	public float smokeLifeShotDelta = -2f;
	public float smokeFireAddMultiplier = .1f;
	public float smokeConstantAdd = .1f;
	public float smokeOverLife = 3f;
	public float smokeSpeedMult = .3f;

	public float antiMatterAmount = 0f;
	public float smokeAntiMatterAddMultiplier = .1f;

	public bool autoReset = false;
	public bool lockValues = false;

    void Start()
    {
        
    }

    void Update()
    {
    	// modify renderer
        FireController fc = GetComponent<FireController>();
    	fc.flameOpacity = fireLife;
    	fc.flameSize = fireLife;
    	fc.flameSpeed = fireLife * fireSpeedMult;
    	fc.smokeOpacity = smokeLife * 4;
    	fc.smokeSpeed = smokeLife * smokeSpeedMult;
    	fc.smokeStability = smokeLife;

        // simulations
        if (!lockValues) {
	        fireLife += fireLifeDelta * Time.deltaTime;
	        if (fireLife > 0) {
	        	fireAliveTime += Time.deltaTime;
	        }
	        else {
	        	fireAliveTime = 0;
	        }
	        if (fireAliveTime > fireAliveSmokeThreshold) {
	        	smokeLife += smokeFireAddMultiplier * Time.deltaTime * fireLife;
	        	smokeLife += smokeConstantAdd * Time.deltaTime;
	        }
	        else {
	        	smokeLife += smokeLifeNoFireDelta * Time.deltaTime;
	        }
	        if (fc.clawsActive) {
	        	smokeLife += smokeLifeShotDelta *  Time.deltaTime;
	        }
	        if (fc.armsActive) {
	        	fireLife += fireLifeShotDelta * Time.deltaTime;
	        }

	        antiMatterAmount += smokeLife * smokeAntiMatterAddMultiplier * Time.deltaTime;
	    }

        // bounds
        fireLife = Mathf.Max(0, fireLife);
        smokeLife = Mathf.Max(0, smokeLife);

        if (autoReset) {
	        if (fireLife > fireOverLife) {
	        	fireLife = 0;
	        }

	        if (smokeLife > smokeOverLife) {
	        	fireLife = 0;
	        }
	    }
    }
}
