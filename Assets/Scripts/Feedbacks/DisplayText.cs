using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI displayText;
    // Start is called before the first frame update
    void Start()
    {
        displayText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DisplayThisText(string newText)
    {
        displayText.text = newText;
        yield return new WaitForSecondsRealtime(3);
        displayText.text = "";
    }
}
