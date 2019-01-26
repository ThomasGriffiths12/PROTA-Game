﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public int ShopId;
    public List<ShopItem> ShopItems;
    public GameObject ShopItemPrefab;
    public Animation anim;
	// Use this for initialization
	void Start () {


        ShopItems = FileSystem.FromJson<ShopItem>("/EntityData/Shops/FoodShop1.json").ToList();
        int count = 0;
        ShopItems = ShopItems.OrderBy(i => i.Cost).ToList();
        foreach (var item in ShopItems)
        {
            item.ModifierList = new Dictionary<StatType, float>();
            string[] modifierssplit = item.Modifiers.Split(',');
            foreach (var splititem in modifierssplit)
            {
                var data = splititem.Split(':');
                item.ModifierList.Add((StatType)Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }

            var pref = GameObject.Instantiate(ShopItemPrefab, this.transform.GetChild(2));
            var buttonInner = pref.transform.GetChild(0);

            pref.GetComponent<Button>().onClick.AddListener(() => ShopItemBought(item.ItemName));

            var image = buttonInner.GetChild(0);
            image.GetComponent<Image>().sprite = Resources.Load<Sprite>("EntityIcons/" + item.SpriteName);

            var textArea = buttonInner.GetChild(1).transform;
            textArea.GetChild(0).GetComponent<Text>().text = item.ItemName;
            textArea.GetChild(1).GetComponent<Text>().text = item.ItemDesc;

            var effectsBar = textArea.GetChild(2);
            effectsBar.GetChild(1).GetComponent<Text>().text = item.ModifierList.Where(i => i.Key == StatType.Hunger).Sum(i => i.Value).ToString();
            effectsBar.GetChild(3).GetComponent<Text>().text = item.ModifierList.Where(i => i.Key == StatType.Hydration).Sum(i => i.Value).ToString();
            effectsBar.GetChild(5).GetComponent<Text>().text = item.ModifierList.Where(i => i.Key == StatType.Warmth).Sum(i => i.Value).ToString();
            effectsBar.GetChild(7).GetComponent<Text>().text = item.ModifierList.Where(i => i.Key == StatType.Cleanliness).Sum(i => i.Value).ToString();


            CultureInfo gb = CultureInfo.GetCultureInfo("en-GB");
            buttonInner.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = item.Cost.ToString("c2", gb);
            count++;
        }

        anim.Play("SlideIn");
        int mej = 0;
    }

    public void ShopItemBought(string name)
    {
        //var Player = GameObject.FindGameObjectWithTag("Player");
        //Player.GetComponent<PlayerManager>();
        var item = ShopItems.FirstOrDefault(i => i.ItemName == name);
        CultureInfo gb = CultureInfo.GetCultureInfo("en-GB");
        string msg = item.ItemName + " purchased for " + item.Cost.ToString("c2", gb);
        ChatLogger.SendChatMessage(msg, Color.white);
        string change = "";
        foreach (var modifier in item.ModifierList)
        {
            change += modifier.Key.ToString() + " " + (modifier.Value < 0 ? "decreased " : "increased ") + "by " + modifier.Value + " | ";
        }
        change = change.TrimEnd('|',' ');
        ChatLogger.SendChatMessage(change, Color.green);

        Debug.Log("Button");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
