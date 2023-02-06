using System;

public class King
{
    string name;
    int age;
    float kingDeathProb = 10;


    public King()
    {
        //generate name TODO
        this.name = "Arthur";

        var random = new Random();

        this.age = random.Next(30, 35);



    }

    public King(string v)
    {
        this.name = v;
        var random = new Random();

        this.age = random.Next(30, 35);

    }

    

    public bool isAlive()
    {
        if (age < 50)
        {
          
            return true;
        }

        var r = new System.Random();

        int random = r.Next(0, 101);

        if (random > kingDeathProb)
        {
            kingDeathProb += 2;
            return true;
        }

        kingDeathProb = 10;
        return false;
    }

    public void increaseAge()
    {
        age++;
    }

    public int getAge()
    {
        return age;
    }
}