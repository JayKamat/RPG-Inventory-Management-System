using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Event Callback
    public delegate void UpdateSelected();
    public static UpdateSelected OnItemSelected;
    public static UpdateSelected OnItemDropped;

    //Global Selected Objects
    public static GameObject goSelectedItem;
    public static GameObject goParentPanel;
    public static GameObject goSelectedSlot;

    //public variables
    public GameObject goItemPrefab;
    public GameObject goInventoryContent;
    public ItemConfig icItemDataSet;

    //Selected Item Stats
    public TMP_InputField CurrentName;
    public TMP_Text CurrentDesc;
    public Image CurrentIcon;
    public TMP_Text CurrentClass;
    public TMP_InputField CurrentDmg;
    public TMP_InputField CurrentDfs;
    public TMP_InputField CurrentStr;
    public TMP_InputField CurrentInt;
    public TMP_InputField CurrentAgi;
    public GameObject BonusObject;
    public TMP_InputField CurrentBonus;

    //Overall Stats
    public TMP_InputField OverallDmg;
    public TMP_InputField OverallDfs;
    public TMP_InputField OverallStr;
    public TMP_InputField OverallInt;
    public TMP_InputField OverallAgi;
    public TMP_InputField OverallHP;
    public TMP_InputField OverallMana;
    public TMP_InputField OverallDodgeChance;
    public TMP_InputField OverallCritChance;


    //Equpment Slots
    public GameObject HeadSlot;
    public GameObject BodySlot;
    public GameObject FeetSlot;
    public GameObject LWeaponSlot;
    public GameObject RWeaponSlot;

    public TMP_Dropdown SortMenu;
    
    public AudioSource SFXMouseClick;
    public static AudioSource GlobalClickSFX;

    ///private variables
    GameObject goInstantiatedItem;
    //Equipped Array
    int[] aEquipedItems = { -1, -1, -1, -1, -1 };

    bool isSlotEmpty=true;

    bool bButtonClicked = false;

    int SelectedIndex;

    int currentPerk;
    string currentPerkType;

    float TotalDamage;
    float TotalDefense;
    float TotalStrength;
    float TotalIntelligence;
    float TotalAgility;

    //For Sorting
    List<Item> SortedItems;

    //Compare Functions
    static int SortByType(Item i1, Item i2)
    {
        return i1.slot.CompareTo(i2.slot);
    }

    static int SortByClass(Item i1, Item i2)
    {
        return i1.item_class.CompareTo(i2.item_class);
    }

    static int SortByAlphabet(Item i1, Item i2)
    {
        return i1.item_name.CompareTo(i2.item_name);
    }

    // Use this for initialization
    void Start () {

        //Subscribe to update on selecting new item
        OnItemSelected += UpdateSelectedStats;
        OnItemDropped += PlaceItemSlot;

        GlobalClickSFX = SFXMouseClick;

        //Retrieve PlayerPrefs
        for (int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.GetInt(i.ToString(), (-1)) != -1)
                icItemDataSet.items[PlayerPrefs.GetInt(i.ToString())].equipped = true;
        }
        Debug.Log(aEquipedItems);

        //Initial Sorting
        SortedItems = icItemDataSet.items;
        Sort(SortMenu.value);

        //Populating Inventory
        foreach (var item in SortedItems)
        {
            goInstantiatedItem = Instantiate(goItemPrefab, goInventoryContent.transform,false);
            goInstantiatedItem.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            goInstantiatedItem.GetComponent<ItemHandler>().SetItemNumber(icItemDataSet.items.IndexOf(item));

            if(item.equipped)
            {
                switch (item.slot.ToString())
                {
                    case "Head":
                        goSelectedItem = goInstantiatedItem;
                        onButtonClick();
                        break;

                    case "Body":
                        goSelectedItem = goInstantiatedItem;
                        onButtonClick();
                        break;

                    case "Feet":
                        goSelectedItem = goInstantiatedItem;
                        onButtonClick();
                        break;

                    case "Weapon":
                        goSelectedItem = goInstantiatedItem;
                        onButtonClick();
                        break;

                    default:
                        break;
                }
            }
        }

    }

    void Sort(int sorttype)
    {
        switch(sorttype)
        {
            case 0:
                SortedItems.Sort(SortByType);
                break;

            case 1:
                SortedItems.Sort(SortByClass);
                break;

            case 2:
                SortedItems.Sort(SortByAlphabet);
                break;

            default:
                SortedItems.Sort(SortByType);
                break;
        }
    }

    public void ReorderItems()
    {
        Sort(SortMenu.value);

        List<GameObject> ChildGameObjects = goInventoryContent.transform.Cast<Transform>().ToList().ConvertAll<GameObject>(t => t.gameObject);

        aEquipedItems = new int[5] { -1,-1,-1,-1,-1 };
        foreach (var item in SortedItems)
        {
            ChildGameObjects[SortedItems.IndexOf(item)].transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            ChildGameObjects[SortedItems.IndexOf(item)].transform.GetComponent<ItemHandler>().SetItemNumber(icItemDataSet.items.IndexOf(item));

            if (item.equipped)
            {
                switch (item.slot.ToString())
                {
                    case "Head":
                        aEquipedItems[0] = SortedItems.IndexOf(item);
                        break;

                    case "Body":
                        aEquipedItems[1] = SortedItems.IndexOf(item);
                        break;

                    case "Feet":
                        aEquipedItems[2] = SortedItems.IndexOf(item);
                        break;

                    case "Weapon":
                        if (aEquipedItems[3] != -1)
                            aEquipedItems[4] = SortedItems.IndexOf(item);
                        else
                            aEquipedItems[3] = SortedItems.IndexOf(item);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    //Run on delegate call via ItemHandler
    //Function to get the selected difference
    public void UpdateSelectedStats()
    {
        SelectedIndex = goSelectedItem.GetComponent<ItemHandler>().GetItemNumber();

        //Set Current Stats
        CurrentName.text = icItemDataSet.items[SelectedIndex].item_name;
        CurrentDesc.text = icItemDataSet.items[SelectedIndex].description;
        CurrentIcon.sprite = icItemDataSet.items[SelectedIndex].icon;
        CurrentClass.text = icItemDataSet.items[SelectedIndex].item_class.ToString();
        CurrentDmg.text = icItemDataSet.items[SelectedIndex].damage.ToString();
        CurrentDfs.text = icItemDataSet.items[SelectedIndex].defence.ToString();
        CurrentStr.text = icItemDataSet.items[SelectedIndex].strength.ToString();
        CurrentInt.text = icItemDataSet.items[SelectedIndex].intel.ToString();
        CurrentAgi.text = icItemDataSet.items[SelectedIndex].agility.ToString();

        if (icItemDataSet.items[SelectedIndex].item_class.ToString() == "Mythical")
        {
            currentPerk = icItemDataSet.items[SelectedIndex].perkPercentage;
            currentPerkType = icItemDataSet.items[SelectedIndex].perk_type.ToString();
            BonusObject.SetActive(true);
            CurrentBonus.text = icItemDataSet.items[SelectedIndex].perkPercentage.ToString() + "% " + icItemDataSet.items[SelectedIndex].perk_type.ToString();
        }
        else
            BonusObject.SetActive(false);

        switch (icItemDataSet.items[SelectedIndex].slot.ToString())
        {
            case "Head":
                isSlotEmpty = HeadSlot.GetComponent<ItemSlotHandler>().isEmpty;
                break;

            case "Body":
                isSlotEmpty = BodySlot.GetComponent<ItemSlotHandler>().isEmpty;
                break;

            case "Feet":
                isSlotEmpty = FeetSlot.GetComponent<ItemSlotHandler>().isEmpty;
                break;

            case "Weapon":
                if (LWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty || RWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty)
                    isSlotEmpty = true;
                else
                    isSlotEmpty = false;

                Debug.Log(LWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty + " " + RWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty);
                break;

            default:
                break;
        }
        UpdateOverallStats();
    }

    //Function to update overall stats
    public void UpdateOverallStats()
    {
        SelectedIndex = goSelectedItem.GetComponent<ItemHandler>().GetItemNumber();

        if(!icItemDataSet.items[SelectedIndex].equipped)
        {
            OverallStatsDifference();
        }
        else
        {
            //Set Overall Stats
            OverallDmg.text = TotalDamage.ToString();
            OverallDfs.text = TotalDefense.ToString();
            OverallStr.text = TotalStrength.ToString();
            OverallInt.text = TotalIntelligence.ToString();
            OverallAgi.text = TotalAgility.ToString();
            OverallHP.text = (TotalStrength * 12).ToString();
            OverallMana.text = (TotalIntelligence * 14).ToString();
            OverallDodgeChance.text = (TotalAgility * 0.2).ToString();
            OverallCritChance.text = (TotalAgility * 0.15).ToString();
        }
        
    }

    //Function to get the additional difference
    void OverallStatsDifference()
    {
        SelectedIndex = goSelectedItem.GetComponent<ItemHandler>().GetItemNumber();

        int slotindex=0;

        float AdditionalDamage=0;
        float AdditionalDefense=0;
        float AdditionalStrength=0;
        float AdditionalIntel=0;
        float AdditionalAgility=0;

        switch (icItemDataSet.items[SelectedIndex].slot.ToString())
        {
            case "Head":
                slotindex = 0;
                break;

            case "Body":
                slotindex = 1;
                break;

            case "Feet":
                slotindex = 2;
                break;

            case "Weapon":
                if (RWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty)
                    slotindex = 3;
                else
                    slotindex = 4;
                break;

            default:
                break;
        }

        //Calculate Stat difference if some slot not empty
        if(!isSlotEmpty)
        {
            AdditionalDamage = ((TotalDamage - icItemDataSet.items[aEquipedItems[slotindex]].damage + icItemDataSet.items[SelectedIndex].damage) - TotalDamage);
            AdditionalDefense = ((TotalDefense - icItemDataSet.items[aEquipedItems[slotindex]].defence + icItemDataSet.items[SelectedIndex].defence) - TotalDefense);
            AdditionalStrength = ((TotalStrength - icItemDataSet.items[aEquipedItems[slotindex]].strength + icItemDataSet.items[SelectedIndex].strength) - TotalStrength);
            AdditionalIntel = ((TotalIntelligence - icItemDataSet.items[aEquipedItems[slotindex]].intel + icItemDataSet.items[SelectedIndex].intel) - TotalIntelligence);
            AdditionalAgility = ((TotalAgility - icItemDataSet.items[aEquipedItems[slotindex]].agility + icItemDataSet.items[SelectedIndex].agility) - TotalAgility);
        }
        

        //Set Overall Stats
        OverallDmg.text = TotalDamage.ToString() + "<b>" + (isSlotEmpty ? (icItemDataSet.items[SelectedIndex].damage.ToString(" + #; - #")) : AdditionalDamage.ToString(" + #; - #; + 0")) + "</b>";
        OverallDfs.text = TotalDefense.ToString() + "<b>" + (isSlotEmpty ? (icItemDataSet.items[SelectedIndex].defence.ToString(" + #; - #")) : AdditionalDefense.ToString(" + #; - #; + 0")) + "</b>";
        OverallStr.text = TotalStrength.ToString() + "<b>" + (isSlotEmpty ? (icItemDataSet.items[SelectedIndex].strength.ToString(" + #; - #")) : AdditionalStrength.ToString(" + #; - #; + 0")) + "</b>";
        OverallInt.text = TotalIntelligence.ToString() + "<b>" + (isSlotEmpty ? (icItemDataSet.items[SelectedIndex].intel.ToString(" + #; - #")) : AdditionalIntel.ToString(" + #; - #; + 0")) + "</b>";
        OverallAgi.text = TotalAgility.ToString() + "<b>" + (isSlotEmpty ? (icItemDataSet.items[SelectedIndex].agility.ToString(" + #; - #")) : AdditionalAgility.ToString(" + #; - #; + 0")) + "</b>";
        OverallHP.text = (TotalStrength * 12).ToString() + "<b>" + (isSlotEmpty ? ((icItemDataSet.items[SelectedIndex].strength * 12).ToString(" + #; - #")) : (AdditionalStrength * 12).ToString(" + #; - #; + 0")) + "</b>";
        OverallMana.text = (TotalIntelligence * 14).ToString() + "<b>" + (isSlotEmpty ? ((icItemDataSet.items[SelectedIndex].intel * 14).ToString(" + #; - #")) : (AdditionalIntel * 14).ToString(" + #; - #; + 0")) + "</b>";
        OverallDodgeChance.text = (TotalAgility * 0.2).ToString() + "<b>" + (isSlotEmpty ? ((icItemDataSet.items[SelectedIndex].agility * 0.2).ToString(" + #; - #")) : (AdditionalAgility * 0.2).ToString(" + #; - #; + 0")) + "</b>";
        OverallCritChance.text = (TotalAgility * 0.15).ToString() + "<b>" + (isSlotEmpty ? ((icItemDataSet.items[SelectedIndex].agility * 0.15).ToString(" + #; - #")) : (AdditionalAgility * 0.15).ToString(" + #; - #; + 0")) + "</b>";
    }

    //Code Snippet to equip item in PlaceItemSlot
    void EquipItem(int slotnum)
    {
        //Deselect previous item
        if (aEquipedItems[slotnum] != -1)
        {
            UnequipItem(aEquipedItems[slotnum]);
        }

        goSelectedSlot.transform.GetChild(0).GetComponent<Image>().sprite = icItemDataSet.items[SelectedIndex].icon;
        goSelectedSlot.transform.GetChild(0).gameObject.SetActive(true);
        goSelectedSlot.transform.GetChild(1).gameObject.SetActive(false);
        goSelectedSlot.GetComponent<ItemSlotHandler>().isEmpty = false;

        //Set new item
        aEquipedItems[slotnum] = SelectedIndex;
        goSelectedItem.GetComponent<ItemHandler>().equipped = true;
        icItemDataSet.items[SelectedIndex].equipped = true;

        //Update Overall Stats
        TotalDamage += icItemDataSet.items[SelectedIndex].damage;
        TotalDefense += icItemDataSet.items[SelectedIndex].defence;
        TotalStrength += icItemDataSet.items[SelectedIndex].strength;
        TotalIntelligence += icItemDataSet.items[SelectedIndex].intel;
        TotalAgility += icItemDataSet.items[SelectedIndex].agility;

        //Add Bonus 
        if (icItemDataSet.items[SelectedIndex].item_class.ToString() == "Mythical")
        {
            currentPerk = icItemDataSet.items[SelectedIndex].perkPercentage;
            currentPerkType = icItemDataSet.items[SelectedIndex].perk_type.ToString();
            switch (currentPerkType)
            {
                case "Damage":
                    TotalDamage += (icItemDataSet.items[SelectedIndex].damage * currentPerk / 100);
                    break;

                case "Defence":
                    TotalDefense += (icItemDataSet.items[SelectedIndex].defence * currentPerk / 100);
                    break;

                case "Strength":
                    TotalStrength += (icItemDataSet.items[SelectedIndex].strength * currentPerk / 100);
                    break;

                case "Intel":
                    TotalIntelligence += (icItemDataSet.items[SelectedIndex].intel * currentPerk / 100);
                    break;

                case "Agility":
                    TotalAgility += (icItemDataSet.items[SelectedIndex].agility * currentPerk / 100);
                    break;

                default:
                    break;
            }
        }

        UpdateOverallStats();
    }

    //Called via delegate from ItemSlotHandler when user drops an item in a slot
    public void PlaceItemSlot()
    {
        SelectedIndex = goSelectedItem.GetComponent<ItemHandler>().GetItemNumber();

        //Remove previous instance of Selected item from other slot
        if (Array.IndexOf(aEquipedItems, SelectedIndex) > -1)
        {
            UnequipItem(SelectedIndex);
        }

        //Equip in new slot with check for item type if fail just gets unequiped
        switch (goSelectedSlot.name)
        {
            case "HeadSlotPanel":
                if(icItemDataSet.items[SelectedIndex].slot.ToString() == "Head")
                {
                    //Equip Snippet Used
                    EquipItem(0);
                }
                break;

            case "BodySlotPanel":
                if (icItemDataSet.items[SelectedIndex].slot.ToString() == "Body")
                {
                    //Equip Snippet Used
                    EquipItem(1);
                }
                break;

            case "FeetSlotPanel":
                if (icItemDataSet.items[SelectedIndex].slot.ToString() == "Feet")
                {
                    //Equip Snippet Used
                    EquipItem(2);
                }
                break;

            case "LWeaponSlotPanel":
                if (icItemDataSet.items[SelectedIndex].slot.ToString() == "Weapon")
                {
                    //Equip Snippet Used
                    EquipItem(3);
                }
                break;

            case "RWeaponSlotPanel":
                if (icItemDataSet.items[SelectedIndex].slot.ToString() == "Weapon")
                {
                    //Equip Snippet Used
                    EquipItem(4);
                }
                break;

            default:
                break;
        }
        bButtonClicked = false;

        Debug.Log(aEquipedItems[0]);
        Debug.Log(aEquipedItems[1]);
        Debug.Log(aEquipedItems[2]);
        Debug.Log(aEquipedItems[3]);
        Debug.Log(aEquipedItems[4]);
    }

    //Code Snippet run to Unequip item of index arg
    void UnequipItem(int index)
    {
        switch (Array.IndexOf(aEquipedItems, index))
        {
            case 0:
                HeadSlot.transform.GetChild(0).gameObject.SetActive(false);
                HeadSlot.transform.GetChild(1).gameObject.SetActive(true);
                HeadSlot.GetComponent<ItemSlotHandler>().isEmpty = true;
                isSlotEmpty = true;
                break;

            case 1:
                BodySlot.transform.GetChild(0).gameObject.SetActive(false);
                BodySlot.transform.GetChild(1).gameObject.SetActive(true);
                BodySlot.GetComponent<ItemSlotHandler>().isEmpty = true;
                isSlotEmpty = true;
                break;

            case 2:
                FeetSlot.transform.GetChild(0).gameObject.SetActive(false);
                FeetSlot.transform.GetChild(1).gameObject.SetActive(true);
                FeetSlot.GetComponent<ItemSlotHandler>().isEmpty = true;
                isSlotEmpty = true;
                break;

            case 3:
                LWeaponSlot.transform.GetChild(0).gameObject.SetActive(false);
                LWeaponSlot.transform.GetChild(1).gameObject.SetActive(true);
                LWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty = true;
                isSlotEmpty = true;
                break;

            case 4:
                RWeaponSlot.transform.GetChild(0).gameObject.SetActive(false);
                RWeaponSlot.transform.GetChild(1).gameObject.SetActive(true);
                RWeaponSlot.GetComponent<ItemSlotHandler>().isEmpty = true;
                isSlotEmpty = true;
                break;

            default:
                break;
        }
        //set '-1' default value in Equipped array
        aEquipedItems[Array.IndexOf(aEquipedItems, index)] = -1;
        icItemDataSet.items[index].equipped = false;

        //Update Overall Stats
        TotalDamage -= icItemDataSet.items[index].damage;
        TotalDefense -= icItemDataSet.items[index].defence;
        TotalStrength -= icItemDataSet.items[index].strength;
        TotalIntelligence -= icItemDataSet.items[index].intel;
        TotalAgility -= icItemDataSet.items[index].agility;

        //Remove Bonus 
        if (icItemDataSet.items[index].item_class.ToString() == "Mythical")
        {
            currentPerk = icItemDataSet.items[index].perkPercentage;
            currentPerkType = icItemDataSet.items[index].perk_type.ToString();
            switch (currentPerkType)
            {
                case "Damage":
                    TotalDamage -= (icItemDataSet.items[index].damage * currentPerk / 100);
                    break;

                case "Defence":
                    TotalDefense -= (icItemDataSet.items[index].defence * currentPerk / 100);
                    break;

                case "Strength":
                    TotalStrength -= (icItemDataSet.items[index].strength * currentPerk / 100);
                    break;

                case "Intel":
                    TotalIntelligence -= (icItemDataSet.items[index].intel * currentPerk / 100);
                    break;

                case "Agility":
                    TotalAgility -= (icItemDataSet.items[index].agility * currentPerk / 100);
                    break;

                default:
                    break;
            }
        }
        

        //Dont run if called via Equip/Unequip Button
        if (!bButtonClicked)
            UpdateOverallStats();
    }

    public void onButtonClick()
    {
        //check flag
        bButtonClicked = true;

        if (goSelectedItem!=null)
        {
            int index = goSelectedItem.GetComponent<ItemHandler>().GetItemNumber();
            if (Array.IndexOf(aEquipedItems, index) > -1)
            {
                //Unequip Item
                bButtonClicked = false;
                UnequipItem(index);
            }
            else
            {
                //Equip Item
                switch(icItemDataSet.items[index].slot.ToString())
                {
                    case "Head":
                        goSelectedSlot = HeadSlot;
                        break;

                    case "Body":
                        goSelectedSlot = BodySlot;
                        break;

                    case "Feet":
                        goSelectedSlot = FeetSlot;
                        break;

                    case "Weapon":
                        if(!LWeaponSlot.transform.GetChild(0).gameObject.activeSelf)
                        {
                            goSelectedSlot = LWeaponSlot;
                        }
                        else
                        {
                            goSelectedSlot = RWeaponSlot;
                        }
                        break;

                    default:
                        break;
                }
                PlaceItemSlot();
            }
        }
        bButtonClicked = false;
    }

    private void OnDestroy()
    {
        //Unsubscribe to update on selecting new item
        OnItemSelected -= UpdateSelectedStats;
        OnItemDropped -= PlaceItemSlot;
    }

    public void SaveInfo()
    {
        for(int i=0; i < 5; i++)
        {
            PlayerPrefs.SetInt(i.ToString(), aEquipedItems[i]);
        }
        PlayerPrefs.Save();
    }

}
