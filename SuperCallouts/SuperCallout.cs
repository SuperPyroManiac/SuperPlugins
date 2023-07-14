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
    internal float OnSceneDistance;
    internal List<Entity> EntitiesToClear = new();
    internal List<Blip> BlipsToClear = new();
    internal Ped Player => Game.LocalPlayer.Character;
    private bool onScene;
    //UI
    protected readonly MenuPool Interaction = new();
    protected readonly UIMenu MainMenu = new("SuperCallouts", "Choose an option.");
    protected readonly UIMenu ConvoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    protected readonly UIMenuItem Questioning = new("Speak With Subjects");
    protected readonly UIMenuItem EndCall = new("~y~End Callout", "Ends the callout.");

    public override bool OnBeforeCalloutDisplayed()
    {
        CalloutPrep();
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
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
        MainMenu.RefreshIndex();
        ConvoMenu.RefreshIndex();
        CalloutAccepted();
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        CalloutRunning();
        base.Process();
    }

    //Overrides
    internal abstract void CalloutPrep();
    internal abstract void CalloutAccepted();
    internal abstract void Callout();
    internal abstract void CalloutRunning();

    internal virtual void CalloutEnd(bool forceCleanup = false)
    {
        if (forceCleanup)
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity))
                if (entity.Exists()) entity.Delete();
            Log.Info("Callout has been forcefully cleaned up.");
        }
        else
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity))
                if (entity.Exists()) entity.Dismiss();
            Game.DisplayHelp("~y~Callout Ended.");
        }

        foreach (var blip in BlipsToClear.Where(blip => blip))
            if (blip.Exists()) blip.Delete();

        Interaction.CloseAllMenus();
        Log.Info("Ending Callout.");
        End();
    }
}