using System;
using System.IO;
using UnityEngine;
using Nima.Math2D;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{

	public AudioSource titleSong;
	private Nima.Animation.ActorAnimation start;
	private Nima.Animation.ActorAnimation title;

	private Nima.Unity.ActorBaseComponent m_Actor;
	private float start_time = 0;
	private float title_time = 0;

	bool starting = false;
	public void Start()
	{
		m_Actor = gameObject.GetComponent<Nima.Unity.ActorBaseComponent>();
		if(m_Actor != null)
		{
			if(m_Actor.ActorInstance != null)
			{
				start = m_Actor.ActorInstance.GetAnimation("start");
				title = m_Actor.ActorInstance.GetAnimation("title");
			}
		}
    	FireStorySystem.storyReset = false;
    	FireStorySystem.steamReset = false;
	}

	public void Update()
	{
		if(m_Actor == null)
		{
			return;
		}

		// Advance idle animations first.
		loopPlayAnim(title, ref title_time, 1);
		start.Apply(0, m_Actor.ActorInstance, 1.0f);
        if (Input.GetKeyDown("z"))
        {
        	starting = true;
        }
        if (starting) {
        	if (playUntilEndAnim(start, ref start_time)) {
        		SceneManager.LoadScene("SampleScene");
        	}
        	titleSong.volume = 1 - start_time;
        }
	}

	private void loopPlayAnim(Nima.Animation.ActorAnimation anim, ref float time, float speed = 1f) {
		if (anim == null) return;
		time = (time+Time.deltaTime * speed)%anim.Duration;
		anim.Apply(time, m_Actor.ActorInstance, 1.0f);
	}

	private bool playUntilEndAnim(Nima.Animation.ActorAnimation anim, ref float time, float speed = 1f) {
		if (anim == null) return false;
		time = (time+Time.deltaTime * speed);
		if (time >= anim.Duration - .05f) {
			time = anim.Duration - .05f;
			anim.Apply(time, m_Actor.ActorInstance, 1.0f);
			return true;
		}
		if (time <= 0) {
			time = 0;
		}
		anim.Apply(time, m_Actor.ActorInstance, 1.0f);
		return false;
	}
}