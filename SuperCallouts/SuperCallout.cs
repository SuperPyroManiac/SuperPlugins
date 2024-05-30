using System;
using System.Collections.Generic;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.RemasteredCallouts;

namespace SuperCallouts;

internal abstract class SuperCallout : ISuperCallout
{
    public abstract Location SpawnPoint { get; set; }
    public abstract float OnSceneDistance { get; set; }
    public abstract string CalloutName { get; set; }
    public abstract string CalloutMessage { get; set; }
    public abstract string CalloutAdvisory { get; set; }
    public Vector3 CalloutPosition { get; set; }
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

    public void OnBeforeCalloutDisplayed()
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
    }

    public void OnCalloutAccepted()
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
    }

    public void Process()
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
    }

    //Overrides
    public virtual void CalloutPrep() {}
    public virtual void CalloutAccepted() {}
    public virtual void CalloutRunning() {}
    public virtual void CalloutOnScene() {}
    public virtual void CalloutEnd(bool forceCleanup = false)
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
    }

    protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == EndCall) CalloutEnd();
    }

    protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index){}
}

internal interface ISuperCallout
{
    Location SpawnPoint { get; set; }
    float OnSceneDistance { get; set; }
    string CalloutName { get; set; }
    string CalloutMessage { get; set; }
    string CalloutAdvisory { get; set; }
    Vector3 CalloutPosition { get; set; }
    void CalloutPrep();
    void CalloutAccepted();
    void CalloutRunning();
    void CalloutOnScene();
    void CalloutEnd(bool forceCleanup = false);
    void Process();
}

internal class SuperCalloutWrapper : Callout
{
    private readonly ISuperCallout _superCallout;

    public SuperCalloutWrapper(ISuperCallout superCallout)
    {
        _superCallout = superCallout;
        CalloutPosition = _superCallout.SpawnPoint.Position;
        CalloutMessage = _superCallout.CalloutMessage;
        CalloutAdvisory = _superCallout.CalloutAdvisory;
        ShowCalloutAreaBlipBeforeAccepting(_superCallout.SpawnPoint.Position, 15f);
    }

    public override bool OnBeforeCalloutDisplayed()
    {
        _superCallout.CalloutPrep();
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        _superCallout.CalloutAccepted();
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        _superCallout.Process();
        base.Process();
    }

    public override void End()
    {
        _superCallout.CalloutEnd(true);
        base.End();
    }
}

public static class SuperCalloutFactory
{
    public static Callout CreateCallout(string type)
    {
        ISuperCallout superCallout = type.ToLower() switch
        {
            "FakeCall" => new FakeCall(),
            _ => throw new ArgumentException("Invalid callout type"),
        };

        return new SuperCalloutWrapper(superCallout);
    }
}