using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    //private variables
    int iItemNumber;
    public bool equipped=false;

    GameObject ParentPanel;
    GameObject OriginalParent;

    void Start()
    {
        ParentPanel = GameObject.Find("Panel");
        OriginalParent = this.transform.parent.gameObject;
    }

    public void SetItemNumber(int i)
    {
        iItemNumber = i;
    }

    public int GetItemNumber()
    {
        return iItemNumber;
    }

    //Handlers  
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.GlobalClickSFX.Play();
        this.transform.SetParent(ParentPanel.transform);
        this.transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;      
    }

    public void OnDrag(PointerEventData eventData)
    {
            this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(OriginalParent.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.GlobalClickSFX.Play();
        GameManager.goSelectedItem = this.gameObject;
        if (GameManager.OnItemSelected != null)
        {
            GameManager.OnItemSelected();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
