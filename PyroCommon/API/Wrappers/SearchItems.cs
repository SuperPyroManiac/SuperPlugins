using PolicingRedefined.Interaction.Assets;
using Rage;

namespace PyroCommon.API.Wrappers;

public static class SearchItems
{
    internal static void AddDrugItem(string item, Enums.DrugType drugType, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new DrugItem(item, EItemChance.Normal, ped, (EDrugType)drugType));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new DrugItem(item, EItemChance.Normal, (EItemLocation)itemLocation, vehicle, (EDrugType)drugType));
    }

    internal static void AddWeaponItem(string item, string weaponId, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new WeaponItem(item, EItemChance.Normal, ped, weaponId));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new WeaponItem(item, EItemChance.Normal, (EItemLocation)itemLocation, vehicle, weaponId));
    }

    internal static void AddFirearmItem(string item, string weaponId, bool visible, bool stolen, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new FirearmItem(item, EItemChance.Normal, ped, stolen, weaponId, visible));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new FirearmItem(item, EItemChance.Normal, (EItemLocation)itemLocation, vehicle, stolen, weaponId, visible));
    }

    internal static void AddSearchItem(string item, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new SearchItem(item, EItemChance.Normal, ped));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new SearchItem(item, EItemChance.Normal, (EItemLocation)itemLocation, vehicle));
    }
}