using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mimicry : AffectableItem
{
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        dialogueLabel.text = Inventory.Instance.selectedItem == ""?"An empty mimicry, get something into it!":"A mimicry with "+Inventory.Instance.selectedItem;
    }
}
