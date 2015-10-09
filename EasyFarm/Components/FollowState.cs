﻿/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using System.Linq;
using EasyFarm.Classes;
using FFACETools;

namespace EasyFarm.Components
{
    /// <summary>
    ///     Moves to target enemies.
    /// </summary>
    public class FollowState : CombatBaseState
    {
        private readonly UnitService UnitService;

        public FollowState(FFACE fface) : base(fface)
        {
            this.UnitService = new UnitService(fface);
        }

        public override void EnterComponent()
        {
            // Stand up from resting. 
            if (FFACE.Player.Status == Status.Healing)
            {
                Player.Stand(FFACE);
            }

            // Disengage an invalid target. 
            if (FFACE.Player.Status == Status.Fighting)
            {
                Player.Disengage(FFACE);
            }
        }

        public override bool CheckComponent()
        {
            // Do not follow during fighting. 
            if (IsFighting) return false;

            // Do not follow when resting. 
            if (new RestComponent(FFACE).CheckComponent()) return false;

            // Avoid following empty units. 
            if (string.IsNullOrWhiteSpace(Config.Instance.FollowedPlayer)) return false;

            // Get the player specified in user settings. 
            var player = GetPlayerByName(Config.Instance.FollowedPlayer);

            // If no player is nearby, return. 
            if (player == null) return false;

            // If we're already close, no action needed. 
            return player.Distance > Config.Instance.FollowDistance;
        }

        /// <summary>
        /// Retrieves the player from the unit array by name. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Unit GetPlayerByName(string name)
        {
            return UnitService.Units.FirstOrDefault(x => x.Name == name);
        }

        public override void RunComponent()
        {
            // Get the player specified in user settings. 
            var player = GetPlayerByName(Config.Instance.FollowedPlayer);

            // Follow the player. 
            FFACE.Navigator.GotoNPC(player.Id);
        }
    }
}