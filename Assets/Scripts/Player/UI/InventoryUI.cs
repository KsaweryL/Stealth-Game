using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI diamondText;

    Diamond[] allDiamonds;
    public int numberOfAllDiamonds;
    
    // Start is called before the first frame update
    void Start()
    {
        diamondText = GetComponent<TextMeshProUGUI>();
        Diamond[] allDiamonds = FindObjectsOfType<Diamond>();
        numberOfAllDiamonds = allDiamonds.Length;

        diamondText.text = "0/" + numberOfAllDiamonds;
    }

    public void UpdateDiamondText(PlayerInventory playerInventory)
    {
        diamondText.text = playerInventory.NumberOfDiamonds.ToString() + "/" + numberOfAllDiamonds;
        

    }

    public void ResetDiamondText()
    {
        diamondText = GetComponent<TextMeshProUGUI>();
        diamondText.text = "0/" + numberOfAllDiamonds;
    }
}
