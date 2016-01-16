using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Device22.Abstract.Logic;

namespace Device22.Logic
{
	public class FSM
	{
		private List<BaseFSMState> states;

		private StateID currentStateID;
		public StateID CurrentStateID
		{ get { return currentStateID; } }
		private BaseFSMState currentState;
		public BaseFSMState CurrentState
		{ get { return currentState; } }

		public FSM()
		{
			states = new List<BaseFSMState>();
		}

		public void AddState(BaseFSMState state)
		{
			if (state == null)
				Debug.LogError("(ERROR) Finite State Machine: State cannot be <null>");
			else
			{
				foreach(BaseFSMState s in states)
				{
					if (state.ID == s.ID)
					{
						Debug.LogError("(ERROR) Finite State Machine: State  + " + state.ID.ToString() + " was already added.");
						return;
					}
				}

				states.Add (state);
				if (states.Count == 1)
				{
					currentState = state;
					currentStateID = state.ID;
				}
			}
		}

		public void DeleteState(StateID id)
		{
			if (id.Value == StateID.Null)
				Debug.LogError("(ERROR) Finite State Machine: State id is set to <null>");
			else
			{
				foreach(BaseFSMState s in states)
				{
					if (id == s.ID)
					{
						states.Remove(s);
						return;
					}
				}
				Debug.LogError("(ERROR) Finite State Machine: State " + id.ToString() + " was not found and can't be deleted.");
			}
		}

		public void DoTransition(Transition transition)
		{
			if (transition.Value == Transition.Null)
				Debug.LogError("(ERROR) Finite State Machine: Transition is set to <null>");
			else
			{
				StateID id = currentState.GetCurrentState(transition);
				if (id.Value == StateID.Null)
					Debug.LogError("(ERROR) Finite State Machine: State " + id.ToString() + " does not have target state " + transition.ToString() + " for transition.");
				else
				{
					currentStateID = id;
					foreach(BaseFSMState s in states)
					{
						if (s.ID == currentStateID)
						{
							currentState.DoBeforeLeavingState();
							currentState = s;
							currentState.DoBeforeEnteringState();
							break;
						}
					}
				}
			}
		}
	}
}

