using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderScript
{
    public int milk; // 0 = none, 1 = whole, 2 = almond, 3 = oat
    public int sugar; // 0 = no sugar, 1 = yes sugar

    public OrderScript(int milk, int sugar)
    {
        this.milk = milk;
        this.sugar = sugar;
    }
}
