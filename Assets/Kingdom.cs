using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Kingdom
{
    public GameObject myGameObject = null;
    private float timeToBirth = 5;
    float timeToCalculation = 1;
    float timeKingAge = 3;
    float defaultTimeKing = 3;

    private string name;
    private King king;
    private List<Child> children;

    public float food;
    public float water;
    public float wealth;
    public float population;

    float unitOfPopulation = 100;
    float foodLossPerUnit = 0.6f;
    float waterLossPerUnit = 0.3f;
    float wealthLossPerUnit = 0.2f;
    float UnitOfHapinessIncrease = 2;

    float shareOfPopulationBaker = 0.4f;
    float shareOfPopulationPlumber = 0.4f;
    float shareOfPopulationMiner = 0.2f;

    float foodProducedPerUnitOfPopulation = 1.0f;
    float waterProducedPerUnitOfPopulation = 1.0f;
    float wealthProducedPerUnitOfPopulation = 1.0f;

    float percentageOfChange = 0.01f;
    float kingDeathProb = 10f;

    float progressTimer;
    float maxProgressTime = 15;

    int childrenCounter = 0;

    private float hapiness;



    public enum StatusOfKingdom
    {
        enemy,
        neutral,
        progress,
        self,
        ally
    };


    private StatusOfKingdom statusOfKingdom = StatusOfKingdom.neutral;

 
    public StatusOfKingdom getType()
    {
        return statusOfKingdom;
    }


    public Kingdom(string name)
    {
        this.name = name;
        children = new List<Child>();

        food = 50;
        water = 50;
        wealth = 50;
        population = 500;
        hapiness = 50;

        king = new King();

    }

    public void ageKing()
    {

        king.increaseAge();

        if (!king.isAlive())
        {
            checkIfSelf();
        }
 
    }

    public void checkIfSelf()
    {
        if (getType() == StatusOfKingdom.self)
        {
            if (children.Count <= 0)
            {
                GameLogic.getInstance().gameOver(0);
            }
            else
            {
                Child firstBorn = children[0];
                king = new King(firstBorn.getName());
                children.Remove(firstBorn);
                GameObject.Destroy(firstBorn.myGameObject);
            }
        }
    }


    public void IncreaseBakerShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationBaker += percentageOfChange;
        shareOfPopulationMiner -= percentageOfChange / 2;
        shareOfPopulationPlumber -= percentageOfChange/2;

        UpdateValue(spinner1, shareOfPopulationBaker);
        UpdateValue(spinner2, shareOfPopulationMiner);
        UpdateValue(spinner3, shareOfPopulationPlumber);
    }

    public void DecreaseBakerShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationBaker -= percentageOfChange;
        shareOfPopulationMiner += percentageOfChange / 2;
        shareOfPopulationPlumber += percentageOfChange / 2;
        UpdateValue(spinner1, shareOfPopulationBaker);
        UpdateValue(spinner2, shareOfPopulationMiner);
        UpdateValue(spinner3, shareOfPopulationPlumber);
    }

    public void IncreasePlumberShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationPlumber += percentageOfChange;
        shareOfPopulationMiner -= percentageOfChange / 2;
        shareOfPopulationBaker -= percentageOfChange / 2;
        UpdateValue(spinner1, shareOfPopulationPlumber);
        UpdateValue(spinner2, shareOfPopulationMiner);
        UpdateValue(spinner3, shareOfPopulationBaker);
    }

    public void DecreasePlumberShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationPlumber -= percentageOfChange;
        shareOfPopulationMiner += percentageOfChange / 2;
        shareOfPopulationBaker += percentageOfChange / 2;
        UpdateValue(spinner1, shareOfPopulationPlumber);
        UpdateValue(spinner2, shareOfPopulationMiner);
        UpdateValue(spinner3, shareOfPopulationBaker);
    }
    public void IncreaseMinerShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationMiner += percentageOfChange;
        shareOfPopulationPlumber -= percentageOfChange / 2;
        shareOfPopulationBaker -= percentageOfChange / 2;
        UpdateValue(spinner1, shareOfPopulationMiner);
        UpdateValue(spinner2, shareOfPopulationPlumber);
        UpdateValue(spinner3, shareOfPopulationBaker);
    }

    public void DecreaseMinerShare(GameObject spinner1, GameObject spinner2, GameObject spinner3)
    {
        shareOfPopulationMiner -= percentageOfChange;
        shareOfPopulationPlumber += percentageOfChange / 2;
        shareOfPopulationBaker += percentageOfChange / 2;
        UpdateValue(spinner1, shareOfPopulationMiner);
        UpdateValue(spinner2, shareOfPopulationPlumber);
        UpdateValue(spinner3, shareOfPopulationBaker);
    }

  public void checkKingsInProgress()
    {

        foreach(Kingdom kingdom in GameLogic.getInstance().kingdomsByName.Values)
        {
            if (kingdom.getType() == StatusOfKingdom.progress)
            {
                if (!kingdom.king.isAlive())
                {
                    kingdom.setStatusOfKingdom(StatusOfKingdom.neutral);
                }
            }
        }

    }

    public void update()
    {
        if ((timeKingAge -= Time.deltaTime) < 0)
        {
            ageKing();
            timeKingAge = defaultTimeKing;
            checkKingsInProgress();
            
        }
        if (( timeToBirth -= Time.deltaTime)<0)
        {
            if (!tryBirthOfChild())
            {
                var rand = new System.Random();
                int num = rand.Next(1, 4);           

                if (num == 1)
                {
                    food += 5;
             
                }else if (num == 2)
                {
                    water += 5;
                }
                else
                {
                    wealth += 5;
                }
            }
            
            timeToBirth = 5;
        }
       
       if (timeToCalculation < 0 )
        {
            timeToCalculation = 1;
            calculateResources();
            
        }
       else
        {
            timeToCalculation -= Time.deltaTime;
        }


       if(statusOfKingdom == StatusOfKingdom.progress)
        {
            if((progressTimer -= Time.deltaTime) < 0)
            {
                setStatusOfKingdom(StatusOfKingdom.ally);
            }

            GameObject flag = myGameObject.transform.Find("Kingdom").gameObject.transform.GetChild(0).gameObject;

            var cubeRenderer = flag.GetComponentInChildren<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.red);
            Vector3 flagPos = flag.transform.position;  
            flagPos.y += Time.deltaTime*0.5f;
            flag.transform.position = flagPos;

        }
    }
    
    void calculateResources()
    {
        

        float deltaFood = 0;
        float deltaWater = 0;
        float deltaWealth = 0;

        float popUnit = population / unitOfPopulation;
        deltaFood -= foodLossPerUnit * popUnit;
        //Debug.Log("Subtracting " + deltaFood + " units of hapiness because of food");
        deltaWater -= waterLossPerUnit * popUnit;
        //Debug.Log("Subtracting " + deltaWater + " units of hapiness because of water");
        deltaWealth -= wealthLossPerUnit * popUnit;
        //Debug.Log("Subtracting " + deltaWealth + " units of hapiness because of wealth");

        food += deltaFood;
        water += deltaWater;
        wealth += deltaWealth;

        float deltaHapiness = 0;

        if (food > 0 && water > 0 && wealth > 0)
        {
            deltaHapiness += UnitOfHapinessIncrease*2;
        }
        else if ((food > 0 && water > 0 && wealth < 0) || (food > 0 && water < 0 && wealth > 0) || (food < 0 && water > 0 && wealth > 0))
        {
            deltaHapiness -= UnitOfHapinessIncrease;
        }
        else
        {
            deltaHapiness -= UnitOfHapinessIncrease*2;
        }


        food = Mathf.Max( food, 0);
        water = Mathf.Max(water, 0);
        wealth = Mathf.Max(wealth, 0);
        hapiness += deltaHapiness;

        hapiness = Mathf.Clamp(hapiness, 0, 100);


        float BakerPopulation = population * shareOfPopulationBaker;
        deltaFood = (BakerPopulation / unitOfPopulation) * foodProducedPerUnitOfPopulation;
        food += deltaFood;

        float PlumberPopulation = population * shareOfPopulationPlumber;
        deltaWater = (PlumberPopulation / unitOfPopulation) * waterProducedPerUnitOfPopulation;
        water += deltaWater;

        float MinerPopulation = population * shareOfPopulationMiner;
        deltaWealth = (MinerPopulation / unitOfPopulation) * wealthProducedPerUnitOfPopulation;
        wealth += deltaWealth;


     
    }

    void UpdateValue(GameObject uiObj, float share)
    {

        Transform textTransform = uiObj.transform.Find("Value");
        
        if(textTransform != null)
        {
            TextMeshProUGUI tmpro = textTransform.GetComponent<TextMeshProUGUI>();
            if (tmpro != null)
            {
                tmpro.text = (population * share).ToString("#.##");
            }
        }

    }

    private bool tryBirthOfChild()
    {
        if (hapiness > 75)
        {
            float r = UnityEngine.Random.RandomRange(0, 100);
            if (r < GameLogic.getInstance().PROBABILITY_OF_BIRTH)
            {
                Child child = new Child(this);

                //insert name for child TODO

                children.Add(child);
                childrenCounter++;

                Transform ChildParentFolder = myGameObject.transform.Find("Children");  
                if (ChildParentFolder != null)
                {
                    VisualKingdom vk = myGameObject.GetComponentInChildren<VisualKingdom>();
                    if (vk != null) 
                    {
                        GameObject newVisualChild = GameObject.Instantiate(vk.childPrefab);
                        newVisualChild.transform.SetParent(ChildParentFolder.transform);
                        newVisualChild.transform.localPosition = new Vector3(childrenCounter - 3, 0, 0);
                        child.myGameObject= newVisualChild;

                        var cubeRenderer = newVisualChild.GetComponentInChildren<Renderer>();
                        Color final_color;
                        if (child.isMale())
                        {
                            final_color = (child.isGay())?Color.magenta : Color.blue;
                        }
                        else
                        {
                            final_color = (child.isGay()) ? Color.yellow : Color.red;
                        }
                        cubeRenderer.material.SetColor("_Color", final_color);

                        VisualChild vc = newVisualChild.GetComponent<VisualChild>();
                        vc.myChild = child;
                    }
                    
                }
                


               

                return true;
            }
        }
        return false;
    }
    
    public void tryToMarry(Child mine, Child other)
    {
        if (mine.isCompatible(other))
        {
            marryChild(mine, other);
        }
    }


    public void marryChild(Child myChild, Child otherChild)
    {
        children.Remove(myChild);
        GameObject.Destroy(myChild.myGameObject);
        
        otherChild.getKingdom().getChildren().Remove(otherChild);
        GameObject.Destroy(otherChild.myGameObject);

        otherChild.getKingdom().setStatusOfKingdom(StatusOfKingdom.progress);
       

    }


    public List<Child> getChildren()
    {
        return children;
    }


    public void updateTotal(TextMeshProUGUI totalPopulation)
    {

        float total = Mathf.Floor(population * (shareOfPopulationBaker + shareOfPopulationMiner + shareOfPopulationPlumber));
        totalPopulation.text = total.ToString();
    }

    public void viewResourceUpdate(TextMeshProUGUI foodText, TextMeshProUGUI waterText, TextMeshProUGUI wealthText, TextMeshProUGUI hapText)
    {
  

        foodText.text =  food.ToString("#.##");
        waterText.text =  water.ToString("#.##");
        wealthText.text = wealth.ToString("#.##");
        hapText.text = hapiness.ToString("#.##");
    }
    public void setStatusOfKingdom(StatusOfKingdom statusOfKingdom)
    {

        if(statusOfKingdom == StatusOfKingdom.progress)
        {
            progressTimer = maxProgressTime;
        }

        this.statusOfKingdom = statusOfKingdom;
    }


   

    public string getName()
    {
        return name;
    }
}