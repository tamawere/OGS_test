using UnityEngine;

/// <summary>
/// this class simulates the users data
/// </summary>
public class Variables : MonoBehaviour
{
    public int whiteOrbs = 2540;//white orbs quantity
    public int redOrbs = 120;//red orbs quantity
    public int yellowOrbs = 752;//yello worbs quantity
    public int greenOrbs = 3524;//green orbs quantity
    public int cyanOrbs = 1548;//cyan orbs quantity
    public int blueOrbs = 896;//blue orbs quantity
    public int magentaOrbs = 2578;// magenta orbs quantity

    //array of each power family´s information, runes and variables
    public spherefamilies[] redFamily, yellowFamily, greenFamily, cyanFamily, blueFamily, magentaFamily;
}

//struct with the basic information of each  rune
[System.Serializable]
public struct spherefamilies
{
    public int unlocked;//unlocked percentage of the rune
    public Color color;//rune´s color
    public string name;//rune´s name
    public int whiteCost, colorCost,mixCost,mixcolorIndex; //cost in orbs to create the rune
    public int quantity;//how many units of this rune do we have
    public spherefamilies(int unlocked, Color color, string name,int whiteCost,int colorCost, int mixCost,int mixcolorIndex,int quantity)
    {
        this.unlocked = unlocked;
        this.color = color;
        this.name = name;
        this.whiteCost = whiteCost;
        this.colorCost = colorCost;
        this.mixCost = mixCost;
        this.mixcolorIndex = mixcolorIndex;
        this.quantity = quantity;
    }
}