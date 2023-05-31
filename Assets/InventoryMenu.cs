using System.Collections;
using System.Collections.Generic;
using Pool;
using TMPro;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject itemCellPrefab;

    public GameObject descPanel;
    public TMP_Text descLabel;
    public Transform contentMenu;

    private ItemInfo draggingInfo;
    private ItemInfo hoveringInfo;
    // Start is called before the first frame update
    void Start()
    {
        EventPool.OptIn("updateItem",updateItem);
        updateItem();
    }

    void updateItem()
    {
        foreach (Transform child in contentMenu)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in Inventory.Instance.itemPair)
        {
            var cell = Instantiate(itemCellPrefab, contentMenu);
            cell.GetComponent<ItemCell>().init(item,this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void showDesc(ItemInfo info)
    {
        hoveringInfo = info;
        descPanel.SetActive(true);
        if (draggingInfo != null && draggingInfo != info)
        {
            descLabel.text = draggingInfo.name+" + "+info.name+" combine result: "+CSVDataManager.Instance.fusionResultName(draggingInfo.name,info.name);
        }
        else
        {
            descLabel.text = info.desc;
        }
    }

    public void hideDesc()
    {
        hoveringInfo = null;
        descPanel.SetActive(false);
    }

    public void startDrag(ItemInfo info)
    {
        draggingInfo = info;
    }

    public void finishDrag(ItemInfo info)
    {
        if (hoveringInfo != null)
        {
            CSVDataManager.Instance.fusion(hoveringInfo.name,info.name);
        }
        
        draggingInfo = null;
    }
}
