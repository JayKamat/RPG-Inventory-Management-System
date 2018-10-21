using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHandler : MonoBehaviour, IDropHandler {

    public bool isEmpty = true;

    // Use this for initialization
    void Start () {
	}

    public void OnDrop(PointerEventData eventData)
    {
        GameManager.GlobalClickSFX.Play();
        GameManager.goSelectedItem = eventData.pointerDrag.gameObject;
        GameManager.goSelectedSlot = this.gameObject;

        if(GameManager.OnItemDropped!=null)
        {
            GameManager.OnItemDropped();
        }
    }
}
