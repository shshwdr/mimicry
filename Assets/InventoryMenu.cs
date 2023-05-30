using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject itemCellPrefab;

    public Transform contentMenu;
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
            cell.GetComponent<ItemCell>().init(item);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
