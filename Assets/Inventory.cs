using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    public List<string> itemList;

    
    public Dictionary<string, int> itemDict = new Dictionary<string, int>();

    public  string selectedItem;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in itemList)
        {
            itemDict[item] = 1;
        }   
        DontDestroyOnLoad(gameObject);
        CanvasMenu.Instance.tutorial.SetActive(true);
    }

    public List<(string, int)> itemPair
    {
        get
        {
            var res = new List<(string, int)>();
            foreach (var item in itemList)
            {
                if (itemDict[item] > 0)
                {
                    res.Add((item,itemDict[item] ));
                }
            }
            return res;
        }
    }
    public void addItem(string item)
    {
        if (itemDict.ContainsKey(item))
        {
            itemDict[item]++;
        }
        else
        {
            itemList.Add(item);
            itemDict[item] = 1;
        }
        
        updateItem();
    }

    public void removeItem(string item)
    {
        itemDict[item]--;
        updateItem();
    }

    public void selectItem(string item)
    {
        selectedItem = item;
        updateItem();
    }

    void updateItem()
    {
        
        EventPool.Trigger("updateItem");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
