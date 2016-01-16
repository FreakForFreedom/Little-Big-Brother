using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Device22.Game.Logic;
using Device22.Messaging;

namespace Device22.Logic.AI
{
	public class Consciousness : MonoBehaviour
	{
		private Memory BBMemory;

		void Awake()
		{
			DontDestroyOnLoad(gameObject);

			BBMemory = BigBrother.Instance.HisMemory;
			
			// Listeners for players
			Messenger.AddListener<Player>("Hey 'bro, remember this player!", BBMemory.AddNewActor<Player>);
			Messenger.AddListener<IEnumerable<Player>>("Hey 'bro, remember all this stuff 'n players!", BBMemory.AddNewActors<Player>);
			Messenger.AddListener<Player>("Hey 'bro, forget this player!", BBMemory.RemoveActor<Player>);
			// Listeners for npcs
			Messenger.AddListener<NPC>("Hey 'bro, remember this npc!", BBMemory.AddNewActor<NPC>);
			Messenger.AddListener<IEnumerable<NPC>>("Hey 'bro, remember all this stuff 'n npcs!", BBMemory.AddNewActors<NPC>);
			Messenger.AddListener<NPC>("Hey 'bro, forget this npc!", BBMemory.RemoveActor<NPC>);
			// Listeners for stations
			// Listeners for asteriods
			Messenger.AddListener<Asteroid>("Hey 'bro, remember this asteroid!", BBMemory.AddNewObject<Asteroid>);
			Messenger.AddListener<Asteroid>("Hey 'bro, boom goes the asteroid!", BBMemory.RemoveObject<Asteroid>);
			// Listeners for planets
			// Listeners for suns

			// Listeners for "talking" to players
			Messenger.AddListener("I have to check with the laws what happend there.", SendChatMsgCheckingLaws);
			Messenger.AddListener("O.K. You're clean. For now...", SendChatMsgPlayersClean);
			Messenger.AddListener("O.K. You broke the law.", SendChatMsgPlayerCought);
			Messenger.AddListener("A slight change in the story is coming right up!", SendChatMsgNextStoryElement);
			Messenger.AddListener("Stop hitting each other!", SendChatMsgStopAttackingPlayer);
			Messenger.AddListener("You really like mining, do you?", SendChatMsgPlayerMining);
			Messenger.AddListener("Strange folk retaliate!", SendChatMsgStrangeFolkAttack);
			Messenger.AddListener("Strange folk attacking!", SendChatMsgStrangeFolkAttacking);
			Messenger.AddListener("Strange folk are fleeing!", SendChatMsgStrangeFolkFleeing);
			Messenger.AddListener("The rebel fleet is here!", SendChatMsgRebelFleetIsHere);
		}

        void OnDisable()
        {
            //Messenger<float>.RemoveListener("speed changed", OnSpeedChanged);
        }

		public void SendChatMsgRebelFleetIsHere()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Stop this or the strange folk will get angry!");
		}

		public void SendChatMsgStrangeFolkAttacking()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Stop this or the strange folk will get angry!");
		}

		public void SendChatMsgStrangeFolkAttack()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: The strange folks are attacking and you are outnumbered.");
		}

		public void SendChatMsgStrangeFolkFleeing()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Well, the strange folks are outnumbered, so they flew away.");
		}

		public void SendChatMsgCheckingLaws()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Jo! I have to check with the laws, what one of you guys just did there");
		}

		public void SendChatMsgPlayersClean()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: You're all clean... for now");
		}

		public void SendChatMsgPlayerCought()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: One of y'all broke the law! Be prepared...");
		}

		public void SendChatMsgNextStoryElement()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: A slight change in the story is coming right up!");
		}

		public void SendChatMsgStopAttackingPlayer()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Stop attacking each other!");
		}

		public void SendChatMsgPlayerMining()
		{
			BigBrother.Instance.Chat.Chat("Big Brother: Stop attacking each other!");
		}
	}
}