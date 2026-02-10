using System;
using System.Collections.Generic;
using System.Linq;
using TutzEngine.StateMachine;
using UnityEngine;

namespace BoardDomain
{
    public class BoardGameplayDomain
    {
        public int TurnsLeft { get; private set; }
        private List<BoardPlayer> _players;

        public BoardGameplayDomain(List<BoardPlayer> players)
        {
            InitPlayers(players);
        }

        private void InitPlayers(List<BoardPlayer> players)
        {
            if (players == null || players.Count != 4)
                throw new ArgumentException("Players list must contain exactly 4 players.");
            _players = players;
        }

        public void SortPlayOrder(Dictionary<BoardPlayer, int> playerRolls)
        {
            _players = _players.OrderByDescending(p => playerRolls[p]).ToList();
        }

        public int GetPlayerPosition(BoardPlayer player)
        {
            // Order players by stars and coins
            var orderedPlayers = _players
                .OrderByDescending(p => p.Stars)
                .ThenByDescending(p => p.Coins)
                .ToList();

            int position = 1;

            for (int i = 0; i < orderedPlayers.Count;)
            {
                var current = orderedPlayers[i];

                // Get all players tied with the current player
                var tiedGroup = orderedPlayers
                    .Where(p => p.Stars == current.Stars && p.Coins == current.Coins)
                    .ToList();

                // If the target player is in the tied group, return the current position
                if (tiedGroup.Any(p => p.Id == player.Id))
                    return position;

                // Advance position and index by the size of the tied group
                position += tiedGroup.Count;
                i += tiedGroup.Count;
            }

            throw new ArgumentException("Player not found in the list.");
        }
    }

    public class BoardPlayer
    {
        public int Id { get; private set; }
        public int Coins { get; private set; }
        public int Stars { get; private set; }

        public void AddCoins(int amount) => SetCoinAmount(Coins + amount);
        public void LoseCoins(int amount) => SetCoinAmount(Coins - amount);
        private void SetCoinAmount(int value) => Coins = Mathf.Max(0, value);
        public void AddStars(int amount) => SetStarAmount(Stars + amount);
        public void LoseStars(int amount) => SetStarAmount(Stars - amount);
        private void SetStarAmount(int value) => Stars = Mathf.Max(0, value);
    }

    public enum PlayerControlType
    {
        Human,
        AI,
    }

    public class BoardGameplayStateMachineManager : BaseStateMachineManager<BoardGameplayBaseState>
    {
        public BoardGameplayDomain GameplayDomain { get; }
        public BoardGameplayShowBoardState ShowBoardState { get; }

        public BoardGameplayStateMachineManager(BoardGameplayDomain gameplay)
        {
            GameplayDomain = gameplay;
            ShowBoardState = new(this);
        }
    }

    public abstract class BoardGameplayBaseState : BaseState
    {
        protected BoardGameplayStateMachineManager Manager { get; }
        protected BoardGameplayDomain GameplayDomain => Manager.GameplayDomain;
        public BoardGameplayBaseState(BoardGameplayStateMachineManager manager)
        {
            Manager = manager;
        }

    }

    public class BoardGameplayShowBoardState : BoardGameplayBaseState
    {
        public BoardGameplayShowBoardState(BoardGameplayStateMachineManager manager) : base(manager)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entering Show Board State");
        }
    }
}

