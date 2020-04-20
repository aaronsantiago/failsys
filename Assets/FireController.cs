using System;
using System.IO;
using UnityEngine;
using Nima.Math2D;

public class FireController : MonoBehaviour
{

	public bool clawsExtended = false;
	public bool clawsSparking = false;
	public bool armsExtended = false;
	public bool armsFiring = false;
	public float targetFade = 1f;
	public float fadeSpeed = .3f;
	public float targetX = 0f;
	public float xSpeed = 2.3f;
	public float targetZ = 0f;
	public float zSpeed = 2.3f;

	public bool armsActive {
		get {
			return armsFiring && arms_contract_time == arms_contract.Duration;
		}
	}

	public bool clawsActive {
		get {
			return clawsSparking && claws_contract_time == claws_contract.Duration;
		}
	}


	public float flameOpacity = 0;
	public float flameSize = 0;
	public float smokeOpacity = 0;
	public float smokeStability = 0;

	public float flameSpeed = 1.0f;
	public float smokeSpeed = 1.0f;

	private float fade_time = 1.0f;
	private float claws_contract_time = 0;
	private float claws_idle_time = 0f;
	private float claws_spark_time = 0;
	private float arms_contract_time = 0;
	private float arms_idle_time = 0;
	private float arms_fire_time = 0;
	private float smoke_flicker_time = 0;
	private float flame_size_time = 0;
	private float flame_flicker2_time = 0;
	private float flame_opacity_time = 0;
	private float smoke_opacity_time = 0;
	private float smoke_stability_time = 0;
	private float show_z_time = 0;
	private float show_x_time = 0;
	private float x_idle_time = 0;
	private float z_idle_time = 0;

	private Nima.Animation.ActorAnimation fade;
	private Nima.Animation.ActorAnimation claws_contract;
	private Nima.Animation.ActorAnimation claws_idle;
	private Nima.Animation.ActorAnimation claws_spark;
	private Nima.Animation.ActorAnimation arms_contract;
	private Nima.Animation.ActorAnimation arms_idle;
	private Nima.Animation.ActorAnimation arms_fire;
	private Nima.Animation.ActorAnimation smoke_flicker;
	private Nima.Animation.ActorAnimation flame_size;
	private Nima.Animation.ActorAnimation flame_flicker2;
	private Nima.Animation.ActorAnimation flame_opacity;
	private Nima.Animation.ActorAnimation smoke_opacity;
	private Nima.Animation.ActorAnimation smoke_stability;
	private Nima.Animation.ActorAnimation show_z;
	private Nima.Animation.ActorAnimation show_x;
	private Nima.Animation.ActorAnimation x_idle;
	private Nima.Animation.ActorAnimation z_idle;

	private Nima.Unity.ActorBaseComponent m_Actor;

	public void Start()
	{
		m_Actor = gameObject.GetComponent<Nima.Unity.ActorBaseComponent>();
		if(m_Actor != null)
		{
			if(m_Actor.ActorInstance != null)
			{
				fade = m_Actor.ActorInstance.GetAnimation("fade");
				claws_contract = m_Actor.ActorInstance.GetAnimation("claws_contract");
				show_z = m_Actor.ActorInstance.GetAnimation("show_z");
				show_x = m_Actor.ActorInstance.GetAnimation("show_x");
				z_idle = m_Actor.ActorInstance.GetAnimation("z_idle");
				x_idle = m_Actor.ActorInstance.GetAnimation("x_idle");
				claws_idle = m_Actor.ActorInstance.GetAnimation("claws_idle");
				claws_spark = m_Actor.ActorInstance.GetAnimation("claws_spark");
				arms_contract = m_Actor.ActorInstance.GetAnimation("arms_contract");
				arms_idle = m_Actor.ActorInstance.GetAnimation("arms_idle");
				arms_fire = m_Actor.ActorInstance.GetAnimation("arms_fire");
				smoke_flicker = m_Actor.ActorInstance.GetAnimation("smoke_flicker");
				flame_flicker2 = m_Actor.ActorInstance.GetAnimation("flame_flicker2");
				flame_size = m_Actor.ActorInstance.GetAnimation("flame_size");
				flame_opacity = m_Actor.ActorInstance.GetAnimation("flame_opacity");
				smoke_opacity = m_Actor.ActorInstance.GetAnimation("smoke_opacity");
				smoke_stability = m_Actor.ActorInstance.GetAnimation("smoke_stability");
			}
		}
	}

	public void Update()
	{
		if(m_Actor == null)
		{
			return;
		}
		if (fade_time > targetFade) {
			fade_time -= Time.deltaTime * fadeSpeed;
		}
		if (fade_time < targetFade) {
			fade_time += Time.deltaTime * fadeSpeed;
		}
		fade.Apply(fade_time, m_Actor.ActorInstance, 1.0f);
		if (show_z_time > targetZ) {
			show_z_time -= Time.deltaTime * zSpeed;
		}
		if (show_z_time < targetZ) {
			show_z_time += Time.deltaTime * zSpeed;
		}
		show_z.Apply(show_z_time, m_Actor.ActorInstance, 1.0f);
		if (show_x_time > targetX) {
			show_x_time -= Time.deltaTime * xSpeed;
		}
		if (show_x_time < targetX) {
			show_x_time += Time.deltaTime * xSpeed;
		}
		show_x.Apply(show_x_time, m_Actor.ActorInstance, 1.0f);

		// Advance idle animations first.
		loopPlayAnim(flame_flicker2, ref flame_flicker2_time, flameSpeed);
		loopPlayAnim(smoke_flicker, ref smoke_flicker_time, smokeSpeed);

		if (clawsExtended) {
			if (playUntilEndAnim(claws_contract, ref claws_contract_time)) {
				// if claws have finished extending, play the claw idle animation
				loopPlayAnim(claws_idle, ref claws_idle_time);
				// only play sparking if claws have finished extending
				if (clawsSparking) {
					loopPlayAnim(claws_spark, ref claws_spark_time);
				}
				else {
					claws_spark.Apply(0, m_Actor.ActorInstance, 1.0f);
				}
			}
			else {
				claws_idle_time = 0;
			}
		}
		else {
			playUntilEndAnim(claws_contract, ref claws_contract_time, -1f);
		}

		if (armsExtended) {
			if (playUntilEndAnim(arms_contract, ref arms_contract_time)) {
				// if arms have finished extending, play the arms idle animation
				loopPlayAnim(arms_idle, ref arms_idle_time);
				// only play firing if arms have finished extending
				if (armsFiring) {
					loopPlayAnim(arms_fire, ref arms_fire_time);
				}
				else {
					arms_fire.Apply(0, m_Actor.ActorInstance, 1.0f);
				}
			}
			else {
				arms_idle_time = 0;
			}
		}
		else {
			playUntilEndAnim(arms_contract, ref arms_contract_time, -1f);
		}

		smoke_stability.Apply(smokeStability, m_Actor.ActorInstance, 1.0f);
		flame_size.Apply(flameSize, m_Actor.ActorInstance, 1.0f);
		smoke_opacity.Apply(smokeOpacity, m_Actor.ActorInstance, 1.0f);
		flame_opacity.Apply(flameOpacity, m_Actor.ActorInstance, 1.0f);
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