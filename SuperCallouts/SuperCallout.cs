using System;
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
    internal abstract Vector3 SpawnPoint { get; set; }
    internal abstract float OnSceneDistance { get; set; }
    internal abstract string CalloutName { get; set; }
    internal List<Entity> EntitiesToClear = new();
    internal List<Blip> BlipsToClear = new();
    internal static Ped Player => Game.LocalPlayer.Character;
    internal bool OnScene;
    //UI
    protected readonly MenuPool Interaction = new();
    protected readonly UIMenu MainMenu = new("SuperCallouts", "Choose an option.");
    protected readonly UIMenu ConvoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    protected readonly UIMenuItem Questioning = new("Speak With Subjects");
    protected readonly UIMenuItem EndCall = new("~y~End Callout", "Ends the callout.");

    public override bool OnBeforeCalloutDisplayed()
    {
        CalloutPrep();
        CalloutPosition = SpawnPoint;
        ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
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
        CalloutAccepted();
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
            CalloutRunning();
            if (!OnScene && Player.DistanceTo(SpawnPoint) < OnSceneDistance)
            {
                OnScene = true;
                CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                var onSceneFiber = GameFiber.StartNew(CalloutOnScene);
            }
            if (Game.IsKeyDown(Settings.EndCall)) CalloutEnd();
            if (Game.IsKeyDown(Settings.Interact)) MainMenu.Visible = !MainMenu.Visible;
            Interaction.ProcessMenus();
        }
        catch(Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
        base.Process();
    }

    //Overrides
    internal virtual void CalloutPrep() {}
    internal virtual void CalloutAccepted() {}
    internal virtual void CalloutRunning() {}
    internal virtual void CalloutOnScene() {}
    internal virtual void CalloutEnd(bool forceCleanup = false)
    {
        if (forceCleanup)
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity))
                if (entity.Exists()) entity.Delete();
            Log.Info($"{CalloutName} callout has been forcefully cleaned up.");
        }
        else
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity))
                if (entity.Exists()) entity.Dismiss();
        }
        Game.DisplayHelp("~y~Callout Ended.");
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code-4");
        foreach (var blip in BlipsToClear.Where(blip => blip))
            if (blip.Exists()) blip.Delete();

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