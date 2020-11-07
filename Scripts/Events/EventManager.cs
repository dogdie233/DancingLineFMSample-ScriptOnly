using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Event
{
	public static class EventManager
	{
		public delegate void StateChange(ref StateChangeEvent arg);
		public delegate void GameOver(ref GameOverEvent arg);
		public static UnityEvent onStart = new UnityEvent();
		public static UnityEvent onBGButtonClick = new UnityEvent();
		public static StateChange OnStateChange;
		public static GameOver onGameOver;

		public static void RunStateChangeEvent(GameState oldState, GameState newState, UnityAction<StateChangeEvent> callback)
		{
			StateChangeEvent arg = new StateChangeEvent(oldState, newState);
			try { OnStateChange.BeginInvoke(ref arg, ar => { OnStateChange.EndInvoke(ref arg, ar); callback.Invoke(arg); }, null); }
			catch (NullReferenceException) { callback.Invoke(arg); }
		}

		public static void RunGameOverEvent(UnityAction<GameOverEvent> callback)
		{
			GameOverEvent arg = new GameOverEvent();
			try { onGameOver.BeginInvoke(ref arg, ar => { onGameOver.EndInvoke(ref arg, ar); callback.Invoke(arg); }, null); }
			catch (NullReferenceException) { callback.Invoke(arg); }
		}
	}
}
