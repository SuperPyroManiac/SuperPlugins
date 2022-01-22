using System;
using Rage;
using UltimateBackup.API;

namespace SuperCallouts.SimpleFunctions
{
    internal static class Wrapper
    {
        //ULTIMATE BACKUP
        internal static void CallCode3()
        {
            Functions.callCode3Backup(false);
        }

        internal static void CallCode2()
        {
            Functions.callCode2Backup(false);
        }

        internal static void CallSwat(bool noose)
        {
            Functions.callCode3SwatBackup(false, noose);
        }

        internal static void CallPursuit()
        {
            Functions.callPursuitBackup(false);
        }

        internal static void CallFd()
        {
            Functions.callFireDepartment();
        }

        internal static void CallEms()
        {
            Functions.callAmbulance();
        }
        
        //Callout Interface
        internal static void StartCi(LSPD_First_Response.Mod.Callouts.Callout sender, string priority, string agency = "")
        {
            try
            {
                CalloutInterface.API.Functions.SendCalloutDetails(sender, priority, agency);
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperCallouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperCallouts Error Report End");
            }
            
        }
        
        internal static void CiSendMessage(LSPD_First_Response.Mod.Callouts.Callout sender, string message)
        {
            try
            {
                CalloutInterface.API.Functions.SendMessage(sender, message);
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperCallouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperCallouts Error Report End");
            }
        }
    }
}