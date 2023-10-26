using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROVQuickSlots : MonoBehaviour, IQuickSlots
{
    public event QuickSlots.OnBind onBind;
    public event QuickSlots.OnToggle onToggle;
    public event QuickSlots.OnSelect onSelect;

    public TechType[] GetSlotBinding()
    {
        return new TechType[] { TechType.None };
    }

    public TechType GetSlotBinding(int slotID)
    {
        return TechType.None;
    }

    public InventoryItem GetSlotItem(int slotID)
    {
        return null;
    }

    public int GetSlotByItem(InventoryItem item)
    {
        return -1;
    }

    public float GetSlotProgress(int slotID)
    {
        return 1f;
    }

    public float GetSlotCharge(int slotID)
    {
        return 1f;
    }

    public void SlotKeyDown(int slotID)
    {
        // TODO(xale): implement
    }

    public void SlotKeyHeld(int slotID)
    {
        // TODO(xale): implement
    }

    public void SlotKeyUp(int slotID)
    {
        // TODO(xale): implement
    }

    public void SlotNext()
    {
        // TODO(xale): implement
        DebugMessages.Show("ROVQuickSlots.SlotNext()");
    }

    public void SlotPrevious()
    {
        // TODO(xale): implement
        DebugMessages.Show("ROVQuickSlots.SlotPrevious()");
    }

    public void SlotLeftDown()
    {
        // TODO(xale): implement
    }

    public void SlotLeftHeld()
    {
        // TODO(xale): implement
    }

    public void SlotLeftUp()
    {
        // TODO(xale): implement
    }

    public void SlotRightDown()
    {
        // TODO(xale): implement
    }

    public void SlotRightHeld()
    {
        // TODO(xale): implement
    }

    public void SlotRightUp()
    {
        // TODO(xale): implement
    }

    public void DeselectSlots()
    {
        // TODO(xale): implement
    }

    public int GetActiveSlotID()
    {
        return -1;
    }

    public bool IsToggled(int slotID)
    {
        return false;
    }

    public int GetSlotCount()
    {
        return 1;
    }

    public void Bind(int slotID, InventoryItem item)
    {
        // TODO(xale): implement
    }

    public void Unbind(int slotID)
    {
        // TODO(xale): implement
    }
}
