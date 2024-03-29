using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using GenericModConfigMenu;

namespace Run_Fast
{
    public class ModEntry : Mod
    {
        private ModConfig config;
        
        // When you press a key, probably X, you run faster but it costs you stamina.
        
        public bool isSprinting = false;
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicking += GameLoopOnUpdateTicking;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoopOnOneSecondUpdateTicked;
            helper.Events.GameLoop.GameLaunched += GameLoopOnGameLaunched;
        }

        private void GameLoopOnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );
            // Add the mod options to the mod menu.
            // First, KeyBind
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Sprint Key",
                tooltip: () => "The key you press to sprint.",
                getValue: () => config.Button,  
                setValue: value => config.Button = value
            );
            // Second, Energy per Second
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Energy Cost",
                tooltip: () => "How much stamina you lose per second while sprinting.",
                getValue: () => config.EnergyCost,
                setValue: value => config.EnergyCost = value
            );
            // Third, How Fast
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Sprint Speed",
                tooltip: () => "How fast you run while sprinting., IE the Multiplier. Higher Values can cause bugs.",
                getValue: () => config.SprintSpeed,
                setValue: value => config.SprintSpeed = value
            );
            
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