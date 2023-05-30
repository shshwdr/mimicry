using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    public GameObject selectOb;

    public Image render;

    public string itemName;
    public int amount;
    private Button button;
    public void init((string, int) pair)
    {
        itemName = pair.Item1;
        amount = pair.Item2;
        gameObject.SetActive(true);
        render.sprite = Resources.Load<Sprite>("items/"+itemName);
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() =>
            {
Inventory.Instance.selectItem(itemName);
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
}
