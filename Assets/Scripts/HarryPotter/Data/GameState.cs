using System;
using System.Collections.Generic;
using UnityEngine;

namespace HarryPotter.Data
{
    [CreateAssetMenu(menuName = "HarryPotter/Game State")]
    public class GameState : ScriptableObject
    {
        public List<Player> Players;
        
        public Player LocalPlayer;
        public Player EnemyPlayer;

        public int CurrentPlayerIndex;

        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        public Player OppositePlayer => Players[1 - CurrentPlayerIndex];

        public void Initialize()
        {
            CurrentPlayerIndex = 0;
            Players = new List<Player>
            {
                LocalPlayer, 
                EnemyPlayer
            };

            foreach (var player in Players)
            {
                player.Initialize();
            }
        }
    }
}