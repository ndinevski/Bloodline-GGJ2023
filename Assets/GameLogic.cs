using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    static GameLogic instance;
    public int NUMBER_OF_KINGDOMS = 8;
    public int PROBABILITY_OF_BIRTH = 30;

    public List<GameObject> kingdomObjects;

    public GameObject BakerSpinner;
    public GameObject PlumberSpinner;
    public GameObject MinerSpinner;
    public Button MarryButton;
    public GameObject KingDied;
    public TextMeshProUGUI totalPopulation;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI wealthText;
    public TextMeshProUGUI hapText;

    public GameObject firstSelectedChild = null;
    public GameObject secondSelectedChild = null;

    bool gameIsOver  = false;   

    public Dictionary<string, Kingdom> kingdomsByName = new Dictionary<string, Kingdom>();
    Kingdom myKingdom;
    Kingdom enemyKingdom;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        generateKingdoms();

        InstallHandlers(BakerSpinner, "Up", OnUpFoodClicked);
        InstallHandlers(BakerSpinner, "Down", OnDownFoodClicked);
        InstallHandlers(PlumberSpinner, "Up", OnUpWaterClicked);
        InstallHandlers(PlumberSpinner, "Down", OnDownWaterClicked);
        InstallHandlers(MinerSpinner, "Up", OnUpMinerClicked);
        InstallHandlers(MinerSpinner, "Down", OnDownMinerClicked);

        MarryButton.onClick.AddListener(TryToMarry);
        KingDied.SetActive(false);
        gameIsOver= false;   
    }

    public void TryToMarry()
    {
        VisualChild firstVisualChild = firstSelectedChild.GetComponent<VisualChild>();
        Child firstChild = firstVisualChild.myChild;

        VisualChild secondVisualChild = secondSelectedChild.GetComponent<VisualChild>();
        Child secondChild = secondVisualChild.myChild;

        firstChild.getKingdom().tryToMarry(firstChild, secondChild);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
            return;

        if (firstSelectedChild != null || secondSelectedChild!= null)
        {
            if (firstSelectedChild!=null && secondSelectedChild!=null)
            {
                VisualChild firstVisualChild = firstSelectedChild.GetComponent<VisualChild>();
                Child firstChild = firstVisualChild.myChild;

                VisualChild secondVisualChild = secondSelectedChild.GetComponent<VisualChild>();
                Child secondChild = secondVisualChild.myChild;

                if (firstChild.isCompatible(secondChild))
                    MarryButton.gameObject.SetActive(true);
                else
                    MarryButton.gameObject.SetActive(false);
                
            }
            return;
        }

        MarryButton.gameObject.SetActive(false);

        foreach (Kingdom kingdom in kingdomsByName.Values)
        {
            kingdom.update();
        }
        
        myKingdom.viewResourceUpdate(foodText, waterText, wealthText, hapText);
        
    }

    void InstallHandlers(GameObject parent, string Name, UnityAction func)
    {
        Transform ButtonTransform = parent.transform.Find(Name);
        if (ButtonTransform != null)
        {
            Button btn = ButtonTransform.gameObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(func);
            }
        }
    }
   

    private void OnUpWaterClicked()
    {
        
         myKingdom.IncreasePlumberShare(PlumberSpinner, MinerSpinner, BakerSpinner);
         myKingdom.updateTotal(totalPopulation);
        
    }

    private void OnDownWaterClicked()
    {
        
            myKingdom.DecreasePlumberShare(PlumberSpinner, MinerSpinner, BakerSpinner);
            myKingdom.updateTotal(totalPopulation);
       
    }


    private void OnUpFoodClicked()
    {
       
            myKingdom.IncreaseBakerShare(BakerSpinner, MinerSpinner, PlumberSpinner);
            myKingdom.updateTotal(totalPopulation);
        
    }

    private void OnDownFoodClicked()
    {
        
            myKingdom.DecreaseBakerShare(BakerSpinner, MinerSpinner, PlumberSpinner);
            myKingdom.updateTotal(totalPopulation);
        
    }
    private void OnUpMinerClicked()
    {
         myKingdom.IncreaseMinerShare(MinerSpinner, PlumberSpinner, BakerSpinner);
         myKingdom.updateTotal(totalPopulation);
        
    }

    private void OnDownMinerClicked()
    {
        
            myKingdom.DecreaseMinerShare(MinerSpinner, PlumberSpinner, BakerSpinner);
            myKingdom.updateTotal(totalPopulation);
       
    }

   

    static public GameLogic getInstance()
    {
        return instance;
    }

    private void generateKingdoms()
    {
        string name;
        for (int i = 0; i < NUMBER_OF_KINGDOMS; i++)
        {

            if (i == 0)
            {
                name = "Bitola";
                myKingdom = new Kingdom(name);
                myKingdom.setStatusOfKingdom(Kingdom.StatusOfKingdom.self);
                kingdomsByName.Add(name, myKingdom);
                myKingdom.myGameObject= kingdomObjects[0];
            }
            else if (i == NUMBER_OF_KINGDOMS - 1)
            {
                name = "Skopje";
                enemyKingdom = new Kingdom(name);
                enemyKingdom.setStatusOfKingdom(Kingdom.StatusOfKingdom.enemy);
                kingdomsByName.Add(name, enemyKingdom);
                enemyKingdom.myGameObject= kingdomObjects[kingdomObjects.Count - 1];
            }
            else
            {
                name = "Kingdom " + i.ToString();
                Kingdom kingdom = new Kingdom(name);
                kingdom.setStatusOfKingdom(Kingdom.StatusOfKingdom.neutral);
                kingdomsByName.Add(name, kingdom);
                kingdom.myGameObject= kingdomObjects[i];
            }
        }
    }

    public void SelectChild(GameObject cld)
    {
        if (firstSelectedChild==null)
        {
            firstSelectedChild = cld;
        }
        else if (secondSelectedChild == null)
        {
            secondSelectedChild = cld;
        }
    }

    public void DeSelectChild(GameObject cld)
    {
        if (firstSelectedChild == cld)
        {
            firstSelectedChild = null;
        }
        else if (secondSelectedChild == cld)
        {
            secondSelectedChild = null;
        }
    }

    public Vector3 GetSelectedOffset(GameObject cld)
    {
        if (firstSelectedChild==cld)
        {
            return Camera.main.transform.position + Camera.main.transform.forward * 4.0f +
                    Camera.main.transform.right * -2.0f;
        }

        if (secondSelectedChild==cld)
        {
            return Camera.main.transform.position + Camera.main.transform.forward * 4.0f +
                    Camera.main.transform.right * 2.0f;
        }

        return Camera.main.transform.position + Camera.main.transform.forward * 4.0f;
    }

    public bool HasEmptySelectionSlot()
    {
        return firstSelectedChild== null || secondSelectedChild == null;        
    }

    public bool firstChildSelected()
    {
        return firstSelectedChild != null;
    }
    public bool secondChildSelected()
    {
        return secondSelectedChild != null;
    }

    public void gameOver(int reason)
    {
        gameIsOver= true;   
        switch (reason)
        {
            case 0:
                KingDied.SetActive(true);
                break;

        }
        
    }
}
