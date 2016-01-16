using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Device22.Messaging;
using Device22.Game.Logic;
using System.Runtime.InteropServices;


namespace Device22.Logic.AI
{
	// "Big Brother" A.I.
	public class BigBrother : MonoBehaviour
	{
		//[DllImport "Lua/lua51.dll"]

		private static BigBrother instance;
		public static BigBrother Instance
		{
			get { return instance; }
			private set { instance = value; }
		}

		public Memory HisMemory { get; private set; }
		public Consciousness HisConsciousness { get; private set; }
		public Personality HisPersonality { get; private set; }

		/// <summary>
		/// Reference to the Chat object, so Big Brother can "talk" with the others.
		/// </summary>
		public uLinkChatGUI Chat;

		/*public enum WatchableObjectType
		{
			STAR = 0,
			PLANET = 1,
			PLAYER = 2,
			NPC = 3,
			ITEM = 4
			// Simply add more types here.
		}*/
		
		// This is the update frequency - meaning the A.I. does it checks every n seconds.
		public float UpdateFrequency = 30.0f;
		
		void Awake()
		{ 
			if (Instance != null)
				Destroy (gameObject);
			else
				Instance = this;
			
			DontDestroyOnLoad(gameObject);
		}
		
		// Use this for initialization
		void Start ()
		{			
			//StartCoroutine("WorkDamnIt");

			//memory = new Memory();
			//consciousness = new Consciousness();
			//personality = new Personality();
			GameObject memoryGO = new GameObject("Memory");
			HisMemory = memoryGO.AddComponent<Memory>();
			GameObject consciousnessGO = new GameObject("Consciousness");
			HisConsciousness = consciousnessGO.AddComponent<Consciousness>();
			GameObject personalityGO = new GameObject("Personality");
			HisPersonality = personalityGO.AddComponent<Personality>();
			memoryGO.transform.parent = consciousnessGO.transform.parent = personalityGO.transform.parent = this.gameObject.transform;

			Debug.Log("Big 'Bro is here, get used to it!");
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		void OnDisable()
		{

		}

		void OnGUI()
		{

		}

		private IEnumerator WorkDamnIt()
		{
			// Wait for n seconds
			yield return new WaitForSeconds(UpdateFrequency);
			
			// Game loop stuff
			
			// Check if there is anything
			// Check if there are suns, planets and asteroids
			// Check if there are npcs
			// Check if there are stations
			// Check 
			
			// More game loop stuff
			yield return GenerateStatistics();
		}
		
		private IEnumerator GenerateStatistics()
		{
			// Statistics stuff
			yield return ChangeGameplay();
		}
		
		private IEnumerator ChangeGameplay()
		{
			yield return WorkDamnIt();
		}
	}
}

