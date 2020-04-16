﻿using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition <b>Time.timeScale</b> to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanTimeScale")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "TimeScale" + LeanTransition.MethodsMenuSuffix)]
	public class LeanTimeScale : LeanMethodWithState
	{
		public override void Register()
		{
			PreviousState = Register(Data.TimeScale, Data.Duration, Data.Ease);
		}

		public static LeanState Register(float fillAmount, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var data = LeanTransition.Register(State.Pool, duration);

			data.TimeScale = fillAmount;
			data.Ease       = ease;

			return data;
		}

		[System.Serializable]
		public class State : LeanState
		{
			[Tooltip("The timeScale we will transition to.")]
			public float TimeScale = 1.0f;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldTimeScale;

			public override bool CanAutoFill
			{
				get
				{
					return Time.timeScale != TimeScale;
				}
			}

			public override void AutoFill()
			{
				TimeScale = Time.timeScale;
			}

			public override void Begin()
			{
				oldTimeScale = Time.timeScale;
			}

			public override void Update(float progress)
			{
				Time.timeScale = Mathf.LerpUnclamped(oldTimeScale, TimeScale, Smooth(Ease, progress));
			}

			public static Stack<State> Pool = new Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static T timeScaleTransition<T>(this T target, float timeScale, float duration, LeanEase ease = LeanEase.Smooth)
			where T : DiagramComponent
		{
			Method.LeanTimeScale.Register(timeScale, duration, ease); return target;
		}

		public static GameObject timeScaleTransition(this GameObject target, float timeScale, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTimeScale.Register(timeScale, duration, ease); return target;
		}
	}
}