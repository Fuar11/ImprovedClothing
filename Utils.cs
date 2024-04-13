using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using MelonLoader;

namespace ImprovedClothing
{
    internal class Utils
    {

        public static void takeOffOutermostClothingItem(ClothingRegion area)
        {

            GearItem gi = getOutermostClothingItemByArea(area);
            if(gi != null)
            {
                GameObject itemToPlace = gi.gameObject;
                GameManager.GetPlayerManagerComponent().StartPlaceMesh(itemToPlace, PlaceMeshFlags.UpdateInventoryOnSuccess);
            }
        }

        public static GearItem getOutermostClothingItemByArea(ClothingRegion clothingRegion)
        {
            PlayerManager pm = GameManager.GetPlayerManagerComponent();

            if (pm.GetClothingInSlot(clothingRegion, ClothingLayer.Top2)) return pm.GetClothingInSlot(clothingRegion, ClothingLayer.Top2);
            else if (pm.GetClothingInSlot(clothingRegion, ClothingLayer.Top)) return pm.GetClothingInSlot(clothingRegion, ClothingLayer.Top);
            else if (pm.GetClothingInSlot(clothingRegion, ClothingLayer.Mid)) return pm.GetClothingInSlot(clothingRegion, ClothingLayer.Mid);
            else return pm.GetClothingInSlot(clothingRegion, ClothingLayer.Base);
        }


    }
}
