﻿using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace SuperCallouts;

internal abstract class SuperCallout : Callout
{
    internal abstract Location SpawnPoint { get; set; }
    internal abstract float OnSceneDistance { get; set; }
    internal abstract string CalloutName { get; set; }
    internal List<Entity> EntitiesToClear = new();
    internal List<Blip> BlipsToClear = new();
    internal static Ped Player => Game.LocalPlayer.Character;
    internal bool OnScene;
    internal bool CalloutEnded;
    //UI
    protected readonly MenuPool Interaction = new();
    protected readonly UIMenu MainMenu = new("SuperCallouts", "Choose an option.");
    protected readonly UIMenu ConvoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    protected readonly UIMenuItem Questioning = new("Speak With Subjects");
    protected readonly UIMenuItem EndCall = new("~y~End Callout", "Ends the callout.");

    public override bool OnBeforeCalloutDisplayed()
    {
        try
        {
            CalloutPrep();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
        CalloutPosition = SpawnPoint.Position;
        ShowCalloutAreaBlipBeforeAccepting(SpawnPoint.Position, 15f);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Log.Info($"{CalloutName} callout accepted!");
        Interaction.Add(MainMenu);
        Interaction.Add(ConvoMenu);
        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;
        ConvoMenu.MouseControlsEnabled = false;
        ConvoMenu.AllowCameraMovement = true;
        MainMenu.AddItem(Questioning);
        MainMenu.AddItem(EndCall);
        MainMenu.BindMenuToItem(ConvoMenu, Questioning);
        ConvoMenu.ParentMenu = MainMenu;
        Questioning.Enabled = false;
        try 
        {
            CalloutAccepted();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
        MainMenu.RefreshIndex();
        ConvoMenu.RefreshIndex();
        MainMenu.OnItemSelect += Interactions;
        ConvoMenu.OnItemSelect += Conversations;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            if (CalloutEnded) return;
            CalloutRunning();
            if (!OnScene && Player.DistanceTo(SpawnPoint.Position) < OnSceneDistance)
            {
                OnScene = true;
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                try {GameFiber.StartNew(CalloutOnScene);}
                catch(Exception e)
                {
                    Log.Error(e.ToString());
                    CalloutEnd(true);
                }
            }
            if (Game.IsKeyDown(Settings.EndCall)) CalloutEnd();
            if (Game.IsKeyDown(Settings.Interact)) MainMenu.Visible = !MainMenu.Visible;
            if (Player.IsDead) CalloutEnd();
            Interaction.ProcessMenus();
        }
        catch(Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
        base.Process();
    }

    public override void End()
    {
        if (!CalloutEnded) CalloutEnd();
        base.End();
    }

    //Overrides
    internal virtual void CalloutPrep() {}
    internal virtual void CalloutAccepted() {}
    internal virtual void CalloutRunning() {}
    internal virtual void CalloutOnScene() {}
    internal virtual void CalloutEnd(bool forceCleanup = false)
    {
        CalloutEnded = true;
        if (forceCleanup)
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity.Exists())) entity.Delete();
            Log.Info($"{CalloutName} callout has been forcefully cleaned up.");
        }
        else
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity.Exists())) entity.Dismiss();
        }
        Game.DisplayHelp("~y~Callout Ended.");
        foreach (var blip in BlipsToClear.Where(blip => blip.Exists())) blip.Delete();
        
        Interaction.CloseAllMenus();
        Log.Info($"Ending {CalloutName} Callout.");
        End();
    }

    protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == EndCall) CalloutEnd();
    }

    protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index){}
}