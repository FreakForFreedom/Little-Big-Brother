using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Device22;
using Device22.Networking;
using Device22.Logic;
using Device22.Messaging;
using LuaInterface;

namespace Device22.Game.Logic
{
	public class StoryManager : MonoBehaviour
	{
		public static StoryElement currStoryElement { get; private set; }

		void Awake() { }
		
		void Start()
		{
			StoryManager.LoadAllFromDatabase();
			
			//Lua.RegisterLuaMethod("SpawnEnemy", objectGen, objectGen.GetType().GetMethod("SpawnEnemy"));
			
			string luaScriptsFolder = Application.streamingAssetsPath;
			Lua.CompileScript(luaScriptsFolder + "/LawResponses.lua");

			//Coroutiner.StartCoroutine(CheckForPossibleTransitionAndDoIt(60.0f));	// Is called from BB
		}

		public IEnumerator CheckForPossibleTransitionAndDoIt(Action<bool> setIsRunningBool, float waitForSeconds, HPlayerModel model, params FactQuery[] queries)
		{
			if ((currStoryElement == null) || (currStoryElement.neighborStateID.Count == 0)) yield break;
			setIsRunningBool(true);
			foreach (uint nextStateID in currStoryElement.neighborStateID)
			{
				// take the next possible story state
				StoryElement se = StoryElement.GetByID(nextStateID);

				// and each criteria in it
				int criteriaCount = se.transitionToCriteria.Count;
				foreach(KeyValuePair<uint, StoryElement.CriteriaValue> c in se.transitionToCriteria)
				{
					// check HPlayerModel
					HPlayerModel.Trait t = HPlayerModel.Trait.TryGetTrait(c.Key);
					if (t != null)
					{
						if (c.Value.compareTo(c.Value.value, (int)model.GetNodeValue(t)))
							criteriaCount--;
					}
					else
					{
						// if criteria is not in a HPlayerModel
						// check against all queries
						foreach (FactQuery q in queries)						
						{
							int value = 0;
							// try get the concept value from player/world/... facts
							FactQuery.Concept concept = (FactQuery.Concept)c.Key;
							//Debug.Log (concept);
							if (q.TryGetFactValue(concept, out value))
							{
								// if it matches with the laws criteria, continue
								if (c.Value.compareTo(value, c.Value.value))
									criteriaCount--;
								// query doesn't contain the corresponding fact value, move to next query
								else
									continue;
							}
						}
					}
					// return in 3 seconds and resume work
					yield return new WaitForSeconds(3);
					
					// if matching law with the most criteria was found
					if (criteriaCount == 0)
					{
						se.ApplyToGameplay();
						Messenger.Broadcast("A slight change in the story is coming right up!");
						StoryManager.currStoryElement = se;
                        goto BreakForTimeX;                     // I'm going to hell for his. I just know it. Please forgive me!
					}
				}
			}
        BreakForTimeX:
			yield return new WaitForSeconds(waitForSeconds);
            setIsRunningBool(false);
            yield break;
		}

		public static void LoadAllFromDatabase()
		{
			// This is needed to load all laws from the database at start. (Coroutines can't be static unfortunately)
			StoryElement dummy = new StoryElement(true);
			Coroutiner.StartCoroutine(dummy.ConvertDataToStoryElements(0, result => currStoryElement = result));
		}
	}
}