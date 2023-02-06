using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child
{
    string name;
    bool male;
    bool gay;
    Kingdom kingdom;

    public GameObject myGameObject;

    public Child(Kingdom kingdom)
    {
        int m = Random.Range(0, 2);

        if (m == 1)
        {
            this.male = true;
            this.name = "Bojan";
        }
        else
        {
            this.male = false;
            this.name = "Ana";
        }

        int g = Random.Range(0, 11);
        if (g == 1)
        {
            this.gay = true;
        }
        else
        {
            this.gay = false;
        }

        this.kingdom = kingdom;
    }

    public string getName()
    {
        return name;
    }

    public bool isMale()
    {
        return this.male;
    }
    public bool isGay()
    {
        return gay;
    }
    public Kingdom getKingdom()
    {
        return kingdom;
    }

    internal bool isCompatible(Child childToMarry)
    {
        if (childToMarry.isMale())
        {
            if(childToMarry.isGay())
            {
                if(this.isMale() && this.isGay())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if(!this.isMale() && !this.isGay())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            if (childToMarry.isGay())
            {
                if (!this.isMale() && this.isGay())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (this.isMale() && !this.isGay())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
