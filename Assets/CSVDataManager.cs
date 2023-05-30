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
        var attractInfos = CsvUtil.LoadObjects<ItemAttractEnemyInfo>("itemAttractEnemy");
        foreach (var enemy in enemyInfos)
        {
            enemy.attracts = new Dictionary<string, ItemAttractEnemyInfo>();
            enemyDict[enemy.name] = enemy;
        }
        foreach (var enemy in itemInfos)
        {
            itemDict[enemy.name] = enemy;
        }

        foreach (var pair in attractInfos)
        {
            enemyDict[pair.enemy].attracts[pair.item] = pair;
        }
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
