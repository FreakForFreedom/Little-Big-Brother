using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Device22.Abstract.Logic
{
	public class Transition
	{
		public const uint Null = 0;
		public uint Value = 0;
	}
	
	public class StateID
	{
		public const uint Null = 0;
		public uint Value = 0;
	}

	public abstract class BaseFSMState
	{
		protected Dictionary<Transition, StateID> transitions = new Dictionary<Transition, StateID>();
		protected StateID stateID;
		public StateID ID
		{ get { return stateID; } }

		public BaseFSMState () { }
	
		public void AddTransition(Transition transition, StateID id)
		{
			if (transition.Value == Transition.Null)
				Debug.LogError("(ERROR) Finite State Machine: Current transition is set to <null>");
			else if (id.Value == StateID.Null)
				Debug.LogError("(ERROR) Finite State Machine: Current transitions id is set to <null>");
			else if (transitions.ContainsKey(transition))
				Debug.LogError("(ERROR) Finite State Machine: State " + stateID.ToString() + " already has transition " + transition.ToString());
			else
				transitions.Add(transition, id);
		}

		public void DeleteTransition(Transition transition)
		{
			if (transition.Value == Transition.Null)
				Debug.LogError("(ERROR) Finite State Machine: Current transition is set to <null>");
			else if (!transitions.ContainsKey(transition))
				Debug.LogError("(ERROR) Finite State Machine: Transition " + transition.ToString() + " in State " + stateID.ToString() + " does not exist.");
			else
				transitions.Remove(transition);

		}

		public StateID GetCurrentState(Transition transition)
		{
			if (transitions.ContainsKey(transition))
				return transitions[transition];
			else
				return new StateID();
		}

		public virtual void DoBeforeEnteringState() { }

		public virtual void DoBeforeLeavingState() { }

		public abstract void Reason();

		public abstract void Act();
	}
}

