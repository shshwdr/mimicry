using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class EnemyInfo
{
    public string name;
    public string drop;
    public string words;
    public string disappoint;
    public Dictionary<string, ItemAttractEnemyInfo> attracts;
}
public class ItemAttractEnemyInfo
{
    public string item;
    public string enemy;
    public string words;
    public int isKill;
}

public class ItemInfo
{
    public string name;
    public string desc;
    
    public Dictionary<string, ItemFusionInfo> fusion;
    public HashSet<string> visitedFusion;
}

public class ItemFusionInfo
{
    public string item1;
    public string item2;
    public string result;

}
public class CSVDataManager : Singleton<CSVDataManager>
{
    public Dictionary<string, EnemyInfo> enemyDict = new Dictionary<string, EnemyInfo>();
    public Dictionary<string, ItemInfo> itemDict = new Dictionary<string, ItemInfo>();

    // Start is called before the first frame update
    void Start()
    {
        
        var enemyInfos = CsvUtil.LoadObjects<EnemyInfo>("enemy");
        var itemInfos = CsvUtil.LoadObjects<ItemInfo>("item");
        var itemFusionInfos = CsvUtil.LoadObjects<ItemFusionInfo>("itemFusion");
        var attractInfos = CsvUtil.LoadObjects<ItemAttractEnemyInfo>("itemAttractEnemy");
        foreach (var enemy in enemyInfos)
        {
            enemy.attracts = new Dictionary<string, ItemAttractEnemyInfo>();
            enemyDict[enemy.name] = enemy;
        }
        foreach (var enemy in itemInfos)
        {
            enemy.fusion = new Dictionary<string, ItemFusionInfo>();
            enemy.visitedFusion = new HashSet<string>();
            itemDict[enemy.name] = enemy;
        }
        
        foreach (var enemy in itemFusionInfos)
        {
            itemDict[enemy.item1].fusion[enemy.item2] = enemy;
            itemDict[enemy.item2].fusion[enemy.item1] = enemy;
        }

        foreach (var pair in attractInfos)
        {
            enemyDict[pair.enemy].attracts[pair.item] = pair;
        }
    }

    public string fusionResultName(string item1, string item2)
    {
        if (itemDict[item1].visitedFusion.Contains(item2))
        {
            var info = fusionInfo(item1, item2);
            if (info == null)
            {
                return "Failed";
            }

            var itemInfo = this.itemInfo(info.result);
            return itemInfo.name;
        }
        else
        {
            return "???";
        }
    }
    public void fusion(string item1, string item2)
    {
        itemDict[item1].visitedFusion.Add(item2);
        itemDict[item2].visitedFusion.Add(item1);
        var info = fusionInfo(item1, item2);
        if (info != null)
        {
            Inventory.Instance.removeItem(item1);
            Inventory.Instance.removeItem(item2);
            Inventory.Instance.addItem(info.result);
        }
    }
    public ItemFusionInfo fusionInfo(string item1, string item2)
    {
        if (itemDict.ContainsKey(item1))
        {
            if (itemDict[item1].fusion.ContainsKey(item2))
            {
                return itemDict[item1].fusion[item2];
            }
        }
        return null;
    }

    public EnemyInfo enemyInfo(string name)
    {
        if (enemyDict.ContainsKey(name))
        {
            return enemyDict[name];
        }
    Debug.LogError("no enemy "+name);
        return null;
    }
    public ItemInfo itemInfo(string name)
    {
        if (itemDict.ContainsKey(name))
        {
            return itemDict[name];
        }
        Debug.LogError("no item "+name);
        return null;
    }
    public ItemAttractEnemyInfo enemyReactToItem(string enemy, string item)
    {
        if (enemyDict.ContainsKey(enemy))
        {
            if (enemyDict[enemy].attracts.ContainsKey(item))
            {
                return enemyDict[enemy].attracts[item];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
