using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private Npc otherNpc;
    public Inventory Inventory { get { return Character.Inventory; } }
    public void DoTrade(Npc _npc)
    {
        otherNpc = _npc;
        otherNpc.DoTrade();
        controller.gameObject.SetActive(true);
        controller.PressInventoryKey();
    }
    public void HideInventoryView()
    {
        Inventory.gameObject.SetActive(false);
        if (otherNpc != null) 
        { 
            if (otherNpc.Inventory.gameObject.activeSelf) 
                otherNpc.Inventory.gameObject.SetActive(false);
        }
        IntoNomalUI();
    }
    public void ShowInventoryView() { Character.ShowInventory(); }
}
