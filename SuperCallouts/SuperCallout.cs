using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.UIManager;
using Rage;
using Rage.Exceptions;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts;

internal abstract class SuperCallout : Callout
{
    // Required properties that derived classes must implement
    internal abstract Location SpawnPoint { get; set; }
    internal abstract float OnSceneDistance { get; set; }
    internal abstract string CalloutName { get; set; }

    // Entity tracking
    internal List<Entity> EntitiesToClear = [];
    internal List<Blip> BlipsToClear = [];

    // State tracking
    internal static Ped Player => Game.LocalPlayer.Character;
    internal bool OnScene;
    private bool _calloutEnded;

    // UI elements
    private readonly MenuPool _interaction = new();
    protected readonly UIMenu MainMenu = new("SuperCallouts", "Choose an option.");
    protected readonly UIMenu ConvoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    protected readonly UIMenuItem Questioning = new("Speak With Subjects");
    protected readonly UIMenuItem EndCall = new("~y~End Callout", "Ends the callout.");

    public override bool OnBeforeCalloutDisplayed()
    {
        try
        {
            CalloutPrep();
            CalloutPosition = SpawnPoint.Position;
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint.Position, 15f);
            return base.OnBeforeCalloutDisplayed();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
            return false;
        }
    }

    public override bool OnCalloutAccepted()
    {
        Log.Info($"{CalloutName} callout accepted!");

        SetupMenus();

        try
        {
            CalloutAccepted();
            return base.OnCalloutAccepted();
        }
        catch (Exception e)
        {
            HandleCalloutException(e);
            return false;
        }
    }

    private void SetupMenus()
    {
        // Add menus to pool
        _interaction.Add(MainMenu);
        _interaction.Add(ConvoMenu);

        // Configure menu settings
        ConfigureMenu(MainMenu);
        ConfigureMenu(ConvoMenu);

        // Set up menu structure
        MainMenu.AddItem(Questioning);
        MainMenu.AddItem(EndCall);
        MainMenu.BindMenuToItem(ConvoMenu, Questioning);
        ConvoMenu.ParentMenu = MainMenu;
        Questioning.Enabled = false;

        // Apply styling and register event handlers
        Style.ApplyStyle(_interaction, false);
        MainMenu.OnItemSelect += Interactions;
        ConvoMenu.OnItemSelect += Conversations;
    }

    private void ConfigureMenu(UIMenu menu)
    {
        menu.MouseControlsEnabled = false;
        menu.AllowCameraMovement = true;
    }

    private void HandleCalloutException(Exception e)
    {
        if (e.ToString().Contains("Could not spawn new vehicle") || e.ToString().Contains("Cannot load invalid model with hash"))
        {
            Log.Error("Vehicle spawn failed! This is likely a mods folder issue and not the plugins fault!\r\n" + e.Message, false);
        }
        else if (e is InvalidHandleableException)
        {
            Log.Error("Failed to start callout! Welcome to modded GTA. Not much I can do here.\r\n" + e.Message, false);
        }
        else
        {
            Log.Error(e.ToString());
        }

        CalloutEnd(true);
    }

    public override void Process()
    {
        try
        {
            if (_calloutEnded)
                return;

            CalloutRunning();
            CheckIfOnScene();
            HandleKeyPresses();
            CheckPlayerStatus();
            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }

        base.Process();
    }

    private void CheckIfOnScene()
    {
        if (!OnScene && Player.DistanceTo(SpawnPoint.Position) < OnSceneDistance)
        {
            OnScene = true;
            Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");

            try
            {
                GameFiber.StartNew(CalloutOnScene, "[SC] OnSceneFiber");
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                CalloutEnd(true);
            }
        }
    }

    private void HandleKeyPresses()
    {
        if (Game.IsKeyDown(Settings.EndCall))
            CalloutEnd();

        if (Game.IsKeyDown(Settings.Interact))
            MainMenu.Visible = !MainMenu.Visible;
    }

    private void CheckPlayerStatus()
    {
        if (Player.IsDead)
            CalloutEnd();
    }

    public override void End()
    {
        if (!_calloutEnded)
            CalloutEnd();

        base.End();
    }

    // Virtual methods for derived classes to override
    internal virtual void CalloutPrep() { }

    internal virtual void CalloutAccepted() { }

    internal virtual void CalloutRunning() { }

    internal virtual void CalloutOnScene() { }

    internal virtual void CalloutEnd(bool forceCleanup = false)
    {
        _calloutEnded = true;

        CleanupEntities();

        if (forceCleanup)
        {
            Log.Info($"{CalloutName} callout has been forcefully cleaned up.");
            Game.DisplayHelp("~r~Error Detected: ~y~Callout forcefully cleared!");
        }
        else
        {
            Game.DisplayHelp("~y~Callout Ended.");
        }

        _interaction.CloseAllMenus();
        Log.Info($"Ending {CalloutName} Callout.");
        End();
    }

    private void CleanupEntities()
    {
        foreach (var entity in EntitiesToClear.Where(entity => entity.Exists()))
            entity.Dismiss();

        foreach (var blip in BlipsToClear.Where(blip => blip.Exists()))
            blip.Delete();
    }

    protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == EndCall)
            CalloutEnd();
    }

    protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index) { }
}
