// -== UniRPG ==-
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================

using UnityEngine;
using System.Collections;

namespace UniRPG
{
	[AddComponentMenu("UniRPG/Character 2/Movement/Pro")]
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(Rigidbody))]
	public class Chara2_ProNavi : Chara2_NaviBase
	{
		private NavMeshAgent agent;
	
		private bool turning = false;
		private Transform _tr;
		private Vector3 targetDirection;
		private Vector3 moveDirection;

		// ============================================================================================================

		void Awake()
		{
			_tr = transform;
			agent = gameObject.GetComponent<NavMeshAgent>();
			
			Rigidbody rb = gameObject.GetComponent<Rigidbody>();
			rb.freezeRotation = true;
			rb.useGravity = false;
			rb.isKinematic = true;
		}

		public void OnEnable()
		{
			turning = false;
			agent.enabled = true;
		}

		public void OnDisable()
		{
			turning = false;
			if (UniRPGGlobal.InstanceExist)
			{
				if (UniRPGGlobal.Instance.state != UniRPGGlobal.State.InMainMenu)
				{
					Stop();
				}
			}

			GetComponent<Rigidbody>().Sleep();
			agent.enabled = false;
		}

		public void Update()
		{
			if (turning)
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 360f * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
				_tr.rotation = Quaternion.LookRotation(moveDirection);
				if (Vector3.Dot(_tr.forward, targetDirection) == 0f) turning = false;
			}
		}

		// ============================================================================================================

		public override void LookAt(Vector3 targetPosition)
		{
			this.turning = true;
			targetPosition.y = _tr.position.y;
			this.targetDirection = targetPosition - _tr.position;
		}

		public override void MoveTo(Vector3 targetPosition, float moveSpeed, float turnSpeed)
		{
			turning = false;
			agent.speed = moveSpeed;
			agent.angularSpeed = turnSpeed;
			agent.destination = targetPosition;
			agent.SetDestination(targetPosition);			
		}

		public override void Stop()
		{
			agent.Stop();
			agent.ResetPath();			
		}

		public override Vector3 CurrentVelocity()
		{
			if ((!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
			{
				return Vector3.zero;
			}
			return agent.velocity;
		}

		public override float CurrentSpeed()
		{
			if ((!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
			{
				return 0f;
			}
			return agent.velocity.magnitude;
		}

		public override bool IsMovingOrPathing()
		{
			return CurrentSpeed() > 0.0f;
		}

		// ============================================================================================================
	}
}