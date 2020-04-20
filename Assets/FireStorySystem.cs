using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class FireStorySystem : MonoBehaviour
{
	public float postIntroFireAliveTimeThreshold = 5f;
	public float postIntroSmokeConstantAdd = .15f;

    public AudioClip introClip;
    public AudioClip armsExtendedClip;
    public AudioClip clawsExtendedClip;
    public AudioClip cargoIgnitedClip; // Cargo minerals successfully ignited.
    public AudioClip cargoIgnitedIntroClip; // Cargo minerals successfully ignited. System must keep mineral flame alive without overloading cargo walls until sufficient amounts of anti-matter are generated.
    public AudioClip streamBegunClip; // Anti-matter stream detected.
    public AudioClip streamBegunIntroClip; // Anti-matter stream detected. System must keep stream within viable operating bounds until sufficient amounts of anti-matter are generated.
    public AudioClip criticalStream; // Anti-matter stream width critical.
    public AudioClip failureStream; // Error. Anti-matter stream has overloaded shield protocols. Rebooting emergency failure systems.
    public AudioClip criticalFlame; // Cargo flame heat critical.
    public AudioClip failureFlame; // Error. Mineral flame has breached cargo walls. Rebooting emergency failure systems.
    public AudioClip failureFlameDied; // Error. Mineral flame has expired and starter materials are unavailable. Rebooting emergency failure systems.
    public AudioClip antiMatterThresholdReached; // Sufficient anti-matter has been generated. Shield protocol engaging. Shutting down emergency failure systems.
    public AudioClip antiMatterProgress1Reached; // Anti-matter generation 33% complete.
    public AudioClip antiMatterProgress2Reached; // Anti-matter generation 50% complete.
    public AudioClip antiMatterProgress3Reached; // Anti-matter generation 75% complete.
    public float antiMatterProgress1Threshold = .3f; // Anti-matter generation 33% complete.
    public float antiMatterProgress2Threshold = .5f; // Anti-matter generation 50% complete.
    public float antiMatterProgress3Threshold = .75f; // Anti-matter generation 75% complete.

    public float criticalFlameThreshold = 7f;
    public float criticalFlameFailureThreshold = 8.5f;
    public float criticalStreamThreshold = 7f;
    public float criticalStreamFailureThreshold = 8.5f;

    public float antiMatterFinishThreshold = 10f;
    public float antiMatterFinishTimeScale = .2f;

    bool flameWithinCritical = false;
    bool streamWithinCritical = false;

    public bool skipIntro = false;
    public bool skipExtendedPrompts = false;
    public static bool storyReset = false;
    public static bool steamReset = false;

    FireController fc;
    FireGameSystem fgs;
 
    // Play default sound
    void Start()
    {
        fc = GetComponent<FireController>();
        fgs = GetComponent<FireGameSystem>();
    	if (storyReset) {
    		skipIntro = true;
    		skipExtendedPrompts = true;
	    	fgs.fireAliveSmokeThreshold = postIntroFireAliveTimeThreshold;
    	}
    	if (steamReset) {
	    	fgs.smokeConstantAdd = postIntroSmokeConstantAdd;
    	}
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(ExampleCoroutine());
    }



    IEnumerator ExampleCoroutine()
    {
        if (!skipIntro) {
	        yield return AudioPlay(introClip);
	    }

        yield return new WaitForSeconds(1.4f);
        fc.armsExtended = true;
        fc.targetFade = 0;

        yield return AudioPlay(armsExtendedClip);

        if (!skipIntro) {
	        fc.targetZ = 1;
	    }

        while (fgs.fireLife == 0) {
        	yield return null;
        }
        fc.targetZ = 0;

        if (!skipExtendedPrompts) {
        	yield return AudioPlay(cargoIgnitedIntroClip);
        }
        else {
        	yield return AudioPlay(cargoIgnitedClip);
        }

        while (fgs.smokeLife == 0) {
        	if(fgs.fireLife > criticalFlameThreshold && !flameWithinCritical) {
        		yield return AudioPlay(criticalFlame);
        		yield return new WaitForSeconds(.2f);
        		flameWithinCritical = true;
        	}
        	if (fgs.fireLife < criticalFlameThreshold) {
        		flameWithinCritical = false;
        	}
        	if(fgs.fireLife <= 0) {
        		storyReset = true;
        		fc.fadeSpeed = 3f;
        		fc.targetFade = 1f;
        		yield return AudioPlay(failureFlameDied);
        		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        	}
        	if(fgs.fireLife > criticalFlameFailureThreshold) {
        		storyReset = true;
        		fc.fadeSpeed = 3f;
        		fc.targetFade = 1f;
        		yield return AudioPlay(failureFlame);
        		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        	}
        	yield return null;
        }

        fgs.fireAliveSmokeThreshold = postIntroFireAliveTimeThreshold;
        
        if (!steamReset) {
        	fc.targetX = 1;
        	yield return AudioPlay(streamBegunIntroClip);
        }
        else {
        	yield return AudioPlay(streamBegunClip);
        }
        fc.clawsExtended = true;
        steamReset = true;
        yield return new WaitForSeconds(.3f);
        yield return AudioPlay(clawsExtendedClip);
        fgs.smokeConstantAdd = postIntroSmokeConstantAdd;

        while (fgs.antiMatterAmount < antiMatterFinishThreshold) {
        	if (fgs.antiMatterAmount > antiMatterProgress1Threshold) {
        		yield return AudioPlay(antiMatterProgress1Reached);
        		yield return new WaitForSeconds(.2f);
        		antiMatterProgress1Threshold = 99999999;
        		fc.targetX = 0;
        	}
        	else if (fgs.antiMatterAmount > antiMatterProgress2Threshold) {
        		yield return AudioPlay(antiMatterProgress2Reached);
        		yield return new WaitForSeconds(.2f);
        		antiMatterProgress2Threshold = 99999999;
        	}
        	else if (fgs.antiMatterAmount > antiMatterProgress3Threshold) {
        		yield return AudioPlay(antiMatterProgress3Reached);
        		yield return new WaitForSeconds(.2f);
        		antiMatterProgress3Threshold = 99999999;
        	}

        	if(fgs.fireLife > criticalFlameThreshold && !flameWithinCritical) {
        		yield return AudioPlay(criticalFlame);
        		yield return new WaitForSeconds(.2f);
        		flameWithinCritical = true;
        	}
        	if (fgs.fireLife < criticalFlameThreshold) {
        		flameWithinCritical = false;
        	}
        	if(fgs.fireLife > criticalFlameFailureThreshold) {
        		storyReset = true;
        		fc.fadeSpeed = 3f;
        		fc.targetFade = 1f;
        		yield return AudioPlay(failureFlame);
        		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        	}
        	if(fgs.smokeLife > criticalStreamThreshold && !streamWithinCritical) {
        		yield return AudioPlay(criticalStream);
        		yield return new WaitForSeconds(.2f);
        		streamWithinCritical = true;
        	}
        	if (fgs.smokeLife < criticalStreamThreshold) {
        		streamWithinCritical = false;
        	}
        	if(fgs.fireLife <= 0) {
        		storyReset = true;
        		fc.fadeSpeed = 3f;
        		fc.targetFade = 1f;
        		yield return AudioPlay(failureFlameDied);
        		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        	}
        	if(fgs.smokeLife > criticalStreamFailureThreshold) {
        		storyReset = true;
        		fc.fadeSpeed = 3f;
        		fc.targetFade = 1f;
        		yield return AudioPlay(failureStream);
        		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        	}
        	yield return null;
        }

        Time.timeScale = antiMatterFinishTimeScale;
        fgs.lockValues = true;
        fc.targetFade = 0f;
        yield return AudioPlay(antiMatterThresholdReached);
        Time.timeScale = 1;
        SceneManager.LoadScene("Outro");
    }

    WaitForSeconds AudioPlay(AudioClip clip) {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        return new WaitForSeconds (GetComponent<AudioSource>().clip.length*Time.timeScale);
    }
}
