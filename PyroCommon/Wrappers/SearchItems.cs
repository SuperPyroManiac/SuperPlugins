using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

namespace PyroCommon.Wrappers;

public static class SearchItems
{
    //PR API
    // internal static void AddDrugItem(string item, Enums.DrugType drugType, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    // {
    //     if (ped == null && vehicle == null) return;
    //     if (ped != null && vehicle != null) return;
    //     if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new DrugItem(item, ped, (EDrugType)drugType));
    //     if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new DrugItem(item, (EItemLocation)itemLocation, vehicle, (EDrugType)drugType));
    // }
    //
    // internal static void AddWeaponItem(string item, string weaponId, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    // {
    //     if (ped == null && vehicle == null) return;
    //     if (ped != null && vehicle != null) return;
    //     if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new WeaponItem(item, ped, weaponId));
    //     if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new WeaponItem(item, (EItemLocation)itemLocation, vehicle, weaponId));
    // }
    //
    // internal static void AddFirearmItem(string item, string weaponId, bool visible, bool stolen, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    // {
    //     if (ped == null && vehicle == null) return;
    //     if (ped != null && vehicle != null) return;
    //     if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new FirearmItem(item, ped, stolen, weaponId, visible, EFirearmState.Normal));
    //     if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new FirearmItem(item, (EItemLocation)itemLocation, vehicle, stolen, weaponId, visible, EFirearmState.Normal));
    // }
    //
    // internal static void AddSearchItem(string item, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    // {
    //     if (ped == null && vehicle == null) return;
    //     if (ped != null && vehicle != null) return;
    //     if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new SearchItem(item, ped));
    //     if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new SearchItem(item, (EItemLocation)itemLocation, vehicle));
    // }
    //
    // internal static void ClearAllItems(Ped ped = null, Vehicle vehicle = null)
    // {
    //     if (ped == null && vehicle == null) return;
    //     if (ped != null && vehicle != null) return;
    //     if (ped != null) PolicingRedefined.API.SearchItemsAPI.ClearPedSearchItems(ped);
    //     if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.ClearVehicleSearchItems(vehicle);
    // }

    //STP WORKAROUND
    internal static void AddStpVehicleDriverSearchItems(Vehicle vehicle, params string[] items)
    {
        var existingItems = (string)vehicle.Metadata.searchDriver ?? string.Empty;
        var splitItems = existingItems.Split(',').ToList();
        var lastItems = splitItems[splitItems.Count - 1].Split([" and "], StringSplitOptions.RemoveEmptyEntries).ToList();
        splitItems.RemoveAt(splitItems.Count - 1);
        splitItems = MergeTwoLists(splitItems, lastItems);
        splitItems = MergeTwoLists(splitItems, items.ToList());
        splitItems[splitItems.Count - 1] = "and " + splitItems[splitItems.Count - 1];
        var newItems = string.Join(", ", splitItems);
        vehicle.Metadata.searchDriver = newItems;
    }

    internal static void AddStpPedSearchItems(Ped ped, params string[] items)
    {
        var existingItems = (string)ped.Metadata.searchPed ?? string.Empty;
        var splitItems = existingItems.Split(',').ToList();
        var lastItems = splitItems[splitItems.Count - 1].Split([" and "], StringSplitOptions.RemoveEmptyEntries).ToList();
        splitItems.RemoveAt(splitItems.Count - 1);
        splitItems = MergeTwoLists(splitItems, lastItems);
        splitItems = MergeTwoLists(splitItems, items.ToList());
        splitItems[splitItems.Count - 1] = "and " + splitItems[splitItems.Count - 1];
        var newItems = string.Join(", ", splitItems);
        ped.Metadata.searchPed = newItems;
    }

    private static List<T> MergeTwoLists<T>(List<T> list1, List<T> list2)
    {
        var mergedList = new List<T>();

        mergedList.AddRange(list1);
        mergedList.AddRange(list2);

        return mergedList;
    }
}
