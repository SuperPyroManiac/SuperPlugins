//using CalloutInterfaceAPI;
//using LSPD_First_Response.Mod.Callouts;
//using Rage;
//
//namespace SuperCallouts.Callouts;
//
//[CalloutInterface("Template", CalloutProbability.Medium, "Example Description")]
//internal class Template : SuperCallout
//{
//    internal override Vector3 SpawnPoint { get; set; }
//    internal override float OnSceneDistance { get; set; }
//    internal override string CalloutName { get; set; }
//    
//    internal override void CalloutPrep()
//    {
//    }
//
//    internal override void CalloutAccepted()
//    {
//    }
//
//    internal override void CalloutRunning()
//    {
//    }
//
//    internal override void CalloutOnScene()
//    {
//    }
//
//    internal override void CalloutEnd(bool forceCleanup = false)
//    {
//        base.CalloutEnd(forceCleanup);
//    }
//}