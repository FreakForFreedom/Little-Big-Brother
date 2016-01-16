using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Device22.Logic.AI
{
	// This class is only a placeholder for later added fuzzy logic.
	public class Personality : MonoBehaviour
	{
		private IEnumerator WorkDamnIt()
		{
			// Wait for n seconds
			//yield return new WaitForSeconds(UpdateFrequency);
			
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