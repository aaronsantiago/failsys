using System;
using System.IO;
using UnityEngine;
using Nima.Math2D;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{

	private Nima.Animation.ActorAnimation outro;

	private Nima.Unity.ActorBaseComponent m_Actor;
	private float outro_time = 0;

	public void Start()
	{
		m_Actor = gameObject.GetComponent<Nima.Unity.ActorBaseComponent>();
		if(m_Actor != null)
		{
			if(m_Actor.ActorInstance != null)
			{
				outro = m_Actor.ActorInstance.GetAnimation("outro");
			}
		}
	}

	public void Update()
	{
		if(m_Actor == null)
		{
			return;
		}

    	if (playUntilEndAnim(outro, ref outro_time)) {
    		SceneManager.LoadScene("Title");
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
		if (time >= anim.Duration) {
			time = anim.Duration;
			return true;
		}
		if (time <= 0) {
			time = 0;
		}
		anim.Apply(time, m_Actor.ActorInstance, 1.0f);
		return false;
	}
}