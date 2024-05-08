using System;
using System.Collections.Generic;
using System.Linq;
using PolicingRedefined.Interaction.Assets;
using Rage;

namespace PyroCommon.API.Wrappers;

public static class SearchItems
{
    internal static void AddDrugItem(string item, Enums.DrugType drugType, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new DrugItem(item, ped, (EDrugType)drugType));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new DrugItem(item, (EItemLocation)itemLocation, vehicle, (EDrugType)drugType));
    }

    internal static void AddWeaponItem(string item, string weaponId, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new WeaponItem(item, ped, weaponId));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new WeaponItem(item, (EItemLocation)itemLocation, vehicle, weaponId));
    }

    internal static void AddFirearmItem(string item, string weaponId, bool visible, bool stolen, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new FirearmItem(item, ped, stolen, weaponId, visible, EFirearmState.Normal));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new FirearmItem(item, (EItemLocation)itemLocation, vehicle, stolen, weaponId, visible, EFirearmState.Normal));
    }

    internal static void AddSearchItem(string item, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere, Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.AddCustomPedSearchItem(new SearchItem(item, ped));
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.AddCustomVehicleSearchItem(new SearchItem(item, (EItemLocation)itemLocation, vehicle));
    }

    internal static void ClearAllItems(Ped ped = null, Vehicle vehicle = null)
    {
        if (ped == null && vehicle == null) return;
        if (ped != null && vehicle != null) return;
        if (ped != null) PolicingRedefined.API.SearchItemsAPI.ClearPedSearchItems(ped);
        if (vehicle != null) PolicingRedefined.API.SearchItemsAPI.ClearVehicleSearchItems(vehicle);
    }

    //STP WORKAROUND
    internal static void AddStpVehicleDriverSearchItems(Vehicle vehicle, params string[] items)
    {
        string existingItems = vehicle.Metadata.searchDriver; // Gets existing metadata
        var splitItems = existingItems.Split(',').ToList(); //splits metdata by comma
        var lastItems = splitItems[splitItems.Count - 1].Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        // the above line gets the last item of the metadata and splits it by and which removes the add and allows for it to see the item/items
        splitItems.RemoveAt(splitItems.Count - 1); //removes last item from splitItems in order to prevent duplicates
        splitItems = MergeTwoLists(splitItems, lastItems); // merges the existing metadata and the lastitem without the and
        splitItems = MergeTwoLists(splitItems, items.ToList()); //merges the new items to existing metadata
        splitItems[splitItems.Count - 1] = "and " + splitItems[splitItems.Count - 1]; //adds back the and
        var newItems = string.Join(", ", splitItems); //joins into one string
        vehicle.Metadata.searchDriver = newItems; //overwrites the metadata
    }

    internal static void AddStpPedSearchItems(Ped ped, params string[] items)
    {
        string existingItems = ped.Metadata.searchPed; // Gets existing metadata
        var splitItems = existingItems.Split(',').ToList(); //splits metdata by comma
        var lastItems = splitItems[splitItems.Count - 1].Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        // the above line gets the last item of the metadata and splits it by and which removes the add and allows for it to see the item/items
        splitItems.RemoveAt(splitItems.Count - 1); //removes last item from splitItems in order to prevent duplicates
        splitItems = MergeTwoLists(splitItems, lastItems); // merges the existing metadata and the lastitem without the and
        splitItems = MergeTwoLists(splitItems, items.ToList()); //merges the new items to existing metadata
        splitItems[splitItems.Count - 1] = "and " + splitItems[splitItems.Count - 1]; //adds back the and
        var newItems = string.Join(", ", splitItems); //joins into one string
        ped.Metadata.searchPed = newItems; //overwrites the metadata
    }

    private static List<T> MergeTwoLists<T>(List<T> list1, List<T> list2)
    {
        var mergedList = new List<T>();

        mergedList.AddRange(list1);
        mergedList.AddRange(list2);

        return mergedList;
    }
}