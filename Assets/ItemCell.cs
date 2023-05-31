using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject selectOb;

    public Image render;

    public string itemName;
    public int amount;
    private Button button;
    private InventoryMenu menu;
    private ItemInfo info;
    public TMP_Text amountLabel;
    public void init((string, int) pair,InventoryMenu m)
    {
        itemName = pair.Item1;
        amount = pair.Item2;
        info = CSVDataManager.Instance.itemInfo(itemName);
        menu = m;
        gameObject.SetActive(true);
        render.sprite = Resources.Load<Sprite>("items/"+itemName);
        button = GetComponentInChildren<Button>();
        amountLabel.text = amount.ToString();
        button.onClick.AddListener(() =>
            {
                if (AffectableManager.Instance.canAction)
                {
                    
                    Inventory.Instance.selectItem(itemName);
                }
            }
        );
        if (Inventory.Instance.selectedItem == itemName)
        {
            selectOb .SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.showDesc(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       menu.hideDesc();
       ;
    }


    public void OnBeginDrag()
    {
        menu.startDrag(info);
    }

    public void OnEndDrag()
    {
        menu.finishDrag(info);
    }
}
