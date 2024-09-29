using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LSPD_First_Response.Mod.API;

namespace PyroCommon.Wrappers
{
    internal class SuperEvents
    {
        private static Assembly _sEvents = Functions.GetAllUserPlugins().First(assembly => assembly.GetName().Name.Equals("SuperEvents"));
        
        internal static void PauseEvents()
        {
            _sEvents.GetType("SuperEvents.EventFunctions.EventManager")
                ?.GetMethod("PauseEvents", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(null, null);
        }
        
        internal static void EndEvent()
        {
            _sEvents.GetType("SuperEvents.EventFunctions.EventManager")
                ?.GetMethod("EndEvent", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(null, null);
        }
        
        internal static void ForceEvent(string eventName)
        {
            _sEvents.GetType("SuperEvents.EventFunctions.EventManager")
                ?.GetMethod("ForceEvent", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(eventName, null);
        }

        internal static List<Type> GetAllEvents()
        {
            var eventManagerType = _sEvents.GetType("SuperEvents.EventFunctions.EventManager");
            if (eventManagerType == null) return null;
            var allEventsField = eventManagerType.GetField("AllEvents", BindingFlags.Static | BindingFlags.NonPublic);

            if (allEventsField != null)
            {
                var allEventsValue = allEventsField.GetValue(null);
                return allEventsValue as List<Type>;
            }

            return null;
        }

        internal static List<Type> GetRegisteredEvents()
        {
            var eventManagerType = _sEvents.GetType("SuperEvents.EventFunctions.EventManager");
            if (eventManagerType == null) return null;
            var registeredEventsField = eventManagerType.GetField("RegisteredEvents", BindingFlags.Static | BindingFlags.NonPublic);

            if (registeredEventsField != null)
            {
                var registeredEventsValue = registeredEventsField.GetValue(null);
                return registeredEventsValue as List<Type>;
            }

            return null;
        }
    }
}
