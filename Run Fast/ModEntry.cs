using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;

namespace Run_Fast
{
    public class ModEntry : Mod
    {
        private ModConfig config;
        
        // When you press a key, probably X, you run faster but it costs you stamina.
        
        public bool isSprinting = false;
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicking += GameLoopOnUpdateTicking;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoopOnOneSecondUpdateTicked; 
            config = helper.ReadConfig<ModConfig>();
        }

        private void GameLoopOnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            if (isSprinting && HasMoved() && Game1.player.running)
            {
                if (Game1.player.Stamina > 5)
                {
                    Game1.player.Stamina -= config.EnergyCost;
                }
                else
                {
                    isSprinting = false;
                }
            }
        }

        private void GameLoopOnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            if (isSprinting && Game1.player.running)
            {
                Game1.player.temporarySpeedBuff = config.SprintSpeed;
            }
        }

        private Vector2 playerPosition;
        private bool HasMoved()
        {
            if (playerPosition != Game1.player.Position)
            {
                playerPosition = Game1.player.Position;
                return true;
            }
            return false;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == config.Button)
            {
                isSprinting = !isSprinting;
            }

        }

        class ModConfig
        {
            public float SprintSpeed = 3f;      // How fast you run when you press the button.
            public float EnergyCost = 2f;       // How much stamina you lose when you press the button.
            public SButton Button = SButton.V;  // The button you press to sprint.
        }
    }
}