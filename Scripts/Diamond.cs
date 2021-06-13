﻿using Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
	public class Diamond : MonoBehaviour, ICollection
	{
		public GameObject child;
		public float speed;
		public Line limit;
		public ParticlesGroup[] particles;
		private static Transform particlesParent;
		private bool picked = false;  // 被线吃
		private bool destroyed = false;  // 被摧毁

		public bool IsPicked
		{
			get { return picked; }
		}

		public bool IdDestroyed
		{
			get { return destroyed; }
		}

		public void Pick()
		{
			if (picked || destroyed) { return; }  //被吃了
			GameController.collections.Add(this);
			child.SetActive(false);
			foreach (Line line in GameController.lines)
			{
				line.skin.PickDiamond(this, line);
			}
			//粒子效果
			if (particles.Length == 0) { return; }
			particlesParent = particlesParent != null ? particlesParent : new GameObject("ParticlesGroup").transform;
			ParticlesGroup group = particles[Random.Range(0, particles.Length)];
			foreach (ParticleAttributes particle in group.particles)
			{
				Destroy(Instantiate(particle.obj, transform.position, particle.obj.transform.rotation, particlesParent), particle.alive);
			}
		}

		public void Recover()
		{
			if (picked) { GameController.collections.Remove(this); }
			picked = destroyed = false;
			child.SetActive(true);
			Destroy(particlesParent.gameObject);
		}

		private void Awake()
		{
			picked = false;
		}

		private void Start()
		{
			child.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self) ;
		}

		private void Update()
		{
			child.transform.Rotate(Vector3.up, speed, Space.Self);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (picked) { return; }
			if (other.CompareTag("Player"))
			{
				Line line = other.GetComponent<Line>();
				if (limit != null && line != limit) { return; }
				EventManager.onDiamondPicked.Invoke(new DiamondPickedEventArgs(line, this, true), (DiamondPickedEventArgs e1) =>
				{
					line.events.onDiamondPicked.Invoke(e1, (DiamondPickedEventArgs e2) =>
					{
						if (!e1.canceled && !e2.canceled) { Pick(); }
					});
				});
			}
		}
	}
}
