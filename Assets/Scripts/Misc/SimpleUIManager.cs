using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUIManager : SimpleUI
{
    public static SimpleUIManager Instance;
    public List<string> stringList = new List<string>();

    [Header("Output")]
    public List<Text> outputTexts; 

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        ClearTextOutputs();
        foreach(string str in stringList)
        {
            Write(str);
            Write("\n");
        }
    }

    public void Write(string txt)
    {
        foreach(Text t in outputTexts)
        {
            t.text += txt;
        }
    }

    public void ClearTextOutputs()
    {
        foreach (Text t in outputTexts)
        {
            t.text = "";
        }
    }
}

/// Text fromation:
/// \n    New line
/// \t Tab
/// \v    Vertical Tab
/// \b    Backspace
/// \r    Carriage return
/// \f    Formfeed
/// \\    Backslash
/// \'    Single quotation mark
/// \"    Double quotation mark
/// \d    Octal
/// \xd    Hexadecimal
/// \ud    Unicode character


