﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Skins
{
	public class NormalSkin : SkinCreationBase<NormalSkinInfo>
	{
		private BoxCollider lineCollider;
		private MeshRenderer meshRenderer;
		private Transform body;
		private Transform bodiesParent;
		private Transform particlesParent;
		private Vector3 bodyStartPosition;

		public NormalSkin(Line line)
		{
			base.line = line;
			GameObject prefab = Resources.Load<GameObject>("LineSkins/Normal/Object");
			gameObject = GameObject.Instantiate(prefab, line.transform.position, line.transform.rotation, line.transform);  // 实例化皮肤go
			meshRenderer = gameObject.GetComponent<MeshRenderer>();
			skinInfo = gameObject.GetComponent<NormalSkinInfo>();
			lineCollider = line.GetComponent<BoxCollider>();
			particlesParent = new GameObject("ParticlesParent").transform;
			bodiesParent = new GameObject("BodiesParent").transform;
		}

		public override void Update()
		{
			if (line.Moving)
			{
				if (body != null)
				{
					body.position = Vector3.Lerp(bodyStartPosition, line.transform.position, 0.5f) + line.transform.rotation * Vector3.back * line.transform.localScale.z / 2f;
					body.localScale = new Vector3(body.localScale.x, body.localScale.y, Vector3.Distance(bodyStartPosition, line.transform.position));
					body.LookAt(line.transform);
				}
			}
		}

		public override void Die(DeathCause deathCause)
		{
			switch (deathCause)
			{
				case DeathCause.Obstacle:
					if (skinInfo.hitObstacleParticles.Length != 0)
					{
						foreach (ParticleAttributes particle in skinInfo.hitObstacleParticles[UnityEngine.Random.Range(0, skinInfo.hitObstacleParticles.Length)].particles)
						{
							GameObject particleObj = GameObject.Instantiate(particle.obj, line.transform.position, Quaternion.Euler(Vector3.zero), particlesParent);
							GameObject.Destroy(particleObj, particle.alive);
						}
					}
					GameController.PlaySound(skinInfo.dieAudio);
					break;
			}
		}

		public override void EndFly()
		{
			CreateBody();
			// 生成跳跃粒子
			if (skinInfo.jumpEffectParticles.Length != 0)
			{
				foreach (ParticleAttributes particle in skinInfo.jumpEffectParticles[UnityEngine.Random.Range(0, skinInfo.jumpEffectParticles.Length)].particles)
				{
					GameObject particleObj = GameObject.Instantiate(particle.obj, line.transform.position, line.transform.rotation, particlesParent);
					GameObject.Destroy(particleObj, particle.alive);
				}
			}
		}

		public override void PickCrown(Crown crown, Line line) { }

		public override void PickDiamond(Diamond diamond, Line line) { }

		public override void Respawn()
		{
			GameObject.Destroy(particlesParent.gameObject);
			GameObject.Destroy(bodiesParent.gameObject);
			particlesParent = new GameObject("ParticlesParent").transform;
			bodiesParent = new GameObject("BodiesParent").transform;
		}

		public override void StartFly()
		{
			/*if (body != null)  //下落线身与地板对齐
			{
				body.localScale -= Vector3.forward * lineCollider.size.z;
				body.Translate(Vector3.back * lineCollider.size.z / 2, Space.Self);
				body = null;
			}*/
			body = null;
		}

		public override void Turn(bool foucs)
		{
			CreateBody();
		}

		public override void Win() { }

		public override void Enable()
		{
			base.Enable();
			meshRenderer.enabled = true;
		}

		public override void Disable()
		{
			base.Disable();
			meshRenderer.enabled = false;
		}

		private void CreateBody()
		{
			if (bodiesParent == null) { bodiesParent = new GameObject("Bodies").transform; }
			if (body != null)
			{
				body.position = Vector3.Lerp(bodyStartPosition, line.transform.position, 0.5f) + Quaternion.Euler(line.nextWay) * Vector3.back * line.transform.localScale.z / 2f;
				body.localScale = new Vector3(body.localScale.x, body.localScale.y, Vector3.Distance(bodyStartPosition, line.transform.position));
				body.LookAt(line.transform);
			}
			bodyStartPosition = line.transform.position;
			body = GameObject.Instantiate(skinInfo.bodyPrefab, bodyStartPosition, line.transform.rotation, bodiesParent).transform;
		}
	}
}
