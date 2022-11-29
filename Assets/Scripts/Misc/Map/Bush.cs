using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    //Script that controls the behaviour for bushes in the simulation
    //Bushes have a limited amount of fruit that can be eaten by agents
    //These fruit are removed when eaten by an agent
    //Fruit regenaretes over time
    #region  fields
    private static float fruitGrowthTime = 5;
    private static int maxFruitNumber = 3;
    private int fruitNumber = 0;
    #endregion
    #region References
    public GameObject fruitA,fruitB,fruitC;
    #endregion
    public static void SetStaticVariables(float growthTime, int maxFruit)
    {
        fruitGrowthTime = growthTime;
        maxFruitNumber = maxFruit;
    }
    void Start()
    {
        Initialise();
    }
    public void Initialise()
    {
        //Randomise starting values
        fruitNumber = Random.Range(0, maxFruitNumber + 1);
        float angle = Random.Range(0,5) * 90;
        transform.Rotate(Vector3.up * angle);

        RefreshFruit();
        StartCoroutine(i_growFruit());
    }
    private void RefreshFruit()
    {
        switch(fruitNumber)
        {
            case 0:
                fruitA.SetActive(false);
                fruitB.SetActive(false);
                fruitC.SetActive(false);
                break;
            case 1:
                fruitA.SetActive(true);
                fruitB.SetActive(false);
                fruitC.SetActive(false);
                break;
            case 2:
                fruitA.SetActive(true);
                fruitB.SetActive(true);
                fruitC.SetActive(false);
                break;
            case 3:
                fruitA.SetActive(true);
                fruitB.SetActive(true);
                fruitC.SetActive(true);
                break;
            default:
                fruitA.SetActive(false);
                fruitB.SetActive(false);
                fruitC.SetActive(false);
                break;     
        }
    }
    
    private IEnumerator i_growFruit()
    {
        while(true)
        {
            yield return new WaitForSeconds(fruitGrowthTime);
            fruitNumber += fruitNumber >= maxFruitNumber ? 0 : 1;
            RefreshFruit();
        }
    }
    private bool TryEatFruit()
    {
        if(fruitNumber > 0)
        {
            fruitNumber--;
            return true;
        }  
        return false;
    }
    #region getters
    public bool HasFruit()
    {
        return fruitNumber > 0 ? true : false;
    }
    #endregion

}
