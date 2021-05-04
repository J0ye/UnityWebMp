using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;

public class SyncVar
{
    /// <summary>
    /// Unique name to identify the variable on run time. Can only be set by inheriting classes in their constructor
    /// </summary>
    private string callName;

    public virtual string CallName { get => callName; protected set => callName = value; }

    protected virtual void SetCallName(string value) { }
}

public class SyncString : SyncVar
{
    private string value;
    public string Value { get => value; set => SetValue(value); }

    /// <summary>
    /// Standard constructor for a SyncString
    /// </summary>
    /// <param name="name">Name of the SyncString</param>
    /// <param name="value">Value of the SyncString</param>
    public SyncString(string name, string val)
    {
        if (name.Contains("SyncString") || name.Contains(":") || name.Contains("|")) 
            throw new ArgumentException("Scritp is trying to create a illigal SyncString with the name " + name);

        CallName = name;
        value = val;
        AddToSyncedList();
    }
    /// <summary>
    /// Special constructor that will parse a string into a SyncString
    /// </summary>
    /// <param name="toParse">Target string. Should be written in special SyncString syntax, i.e. SyncString:name|value'</param>
    public SyncString(string toParse)
    {
        SyncString temp = Parse(toParse);
        CallName = temp.CallName;
        value = temp.Value;
        AddToSyncedList();
    }

    /// <summary>
    /// Creates a string, that is ready to be send or to be parsed back into a SyncString
    /// </summary>
    /// <returns>string in SyncStrin syntax, i.e. SyncString:m_callName| m_value</returns>
    public override string ToString()
    {
        return "SyncString: " + CallName + "|" + Value;
    }
    /// <summary>
    /// Parses a target string into SyncString, only if the traget
    /// is in the SyncString syntax, i.e. SyncString:m_callName| m_value
    /// </summary>
    /// <param name="target"></param>
    /// <returns>SyncString with target parsed values</returns>
    public static SyncString Parse(string target)
    {
        if(target.Contains("SyncString"))
        {
            string[] split = target.Split(":".ToCharArray());
            split = split[1].Split("|".ToCharArray());
            split[0] = split[0].TrimStart();
            if(SyncedStrings.Instance.GetEntry(split[0]) != null)
            {
                Debug.Log("Found existing string");
                SyncedStrings.Instance.GetEntry(split[0]).value = split[1];
                return SyncedStrings.Instance.GetEntry(split[0]);
            }
            SyncString temp = new SyncString(split[0], split[1]);
            return temp;
        }

        throw new InvalidOperationException("Script is trying to parse a string that is not a SyncString." + target + " misses 'SyncString:'  syntax.");
    }
    /// <summary>
    /// Sends the values of this SyncString to the server.
    /// </summary>
    public void SendChanges()
    {
        SyncedStrings.Instance.Behaviour.Send(ToString());
        Debug.Log("Send changes to server");
    }

    protected void SetValue(string val)
    {
        value = val;
        SendChanges();
    }

    /// <summary>
    /// Will add this SyncString to the SyncedString instance. Also handles exception, if the instance is null.
    /// </summary>
    private void AddToSyncedList()
    {
        if (SyncedStrings.Instance != null)
        {
            SyncedStrings.Instance.AddEntry(this);
        }
        else
        {
            throw new InvalidOperationException("Script is trying to create a SyncedString without creating  a list first. Insert new SyncedStrings(Behaviour); before " + CallName);
        }
    }
}

public class SyncFloat : SyncVar
{
    private float value;

    public float Value { get => value; set => SetValue(value); }

    /// <summary>
    /// Standard constructor for a SyncFloat
    /// </summary>
    /// <param name="name">Name of the SyncFloat</param>
    /// <param name="value">Value of the SyncFloat</param>
    public SyncFloat(string name, float val)
    {
        if (name.Contains("SyncFloat") || name.Contains(":") || name.Contains("|"))
            throw new ArgumentException("Scritp is trying to create a illigal SyncFloat with the name " + name);

        CallName = name;
        value = val;
        AddToSyncedList();
    }
    /// <summary>
    /// Special constructor that will parse a string into a SyncFloat
    /// </summary>
    /// <param name="toParse">Target string. Should be written in special SyncFloat syntax, i.e. SyncFloat:name|value'</param>
    public SyncFloat(string toParse)
    {
        SyncFloat temp = Parse(toParse);
        CallName = temp.CallName;
        value = temp.Value;
        AddToSyncedList();
    }

    /// <summary>
    /// Creates a string, that is ready to be send or to be parsed back into a SyncFloat
    /// </summary>
    /// <returns>string in SyncFloat syntax, i.e. SyncFloat:m_callName|m_value</returns>
    public override string ToString()
    {
        return "SyncFloat: " + CallName + "|" + Value.ToString();
    }
    /// <summary>
    /// Parses a target string into SyncFloat, only if the traget
    /// is in the SyncFloat syntax, i.e. SyncFloat:m_callName|m_value
    /// </summary>
    /// <param name="target"></param>
    /// <returns>SyncFloat with target parsed values</returns>
    public static SyncFloat Parse(string target)
    {
        if (target.Contains("SyncFloat"))
        {
            string[] split = target.Split(":".ToCharArray());
            split = split[1].Split("|".ToCharArray());
            split[0] = split[0].TrimStart();
            if (SyncedFloats.Instance.GetEntry(split[0]) != null)
            {
                Debug.Log("Found existing float");
                SyncedFloats.Instance.GetEntry(split[0]).value = float.Parse(split[1]);
                return SyncedFloats.Instance.GetEntry(split[0]);
            }
            SyncFloat temp = new SyncFloat(split[0], float.Parse(split[1]));
            return temp;
        }

        throw new InvalidOperationException("Script is trying to parse a float that is not a SyncFloat." + target + " misses 'SyncFloat:'  syntax.");
    }
    /// <summary>
    /// Sends the values of this SyncFloat to the server.
    /// </summary>
    public void SendChanges()
    {
        SyncedStrings.Instance.Behaviour.Send(ToString());
        Debug.Log("Send changes to server");
    }

    protected void SetValue(float val)
    {
        value = val;
        SendChanges();
    }

    /// <summary>
    /// Will add this SyncFloat to the SyncedFloats instance. Also handles exception, if the instance is null.
    /// </summary>
    private void AddToSyncedList()
    {
        if (SyncedFloats.Instance != null)
        {
            SyncedFloats.Instance.AddEntry(this);
            Debug.Log("Added " + CallName + " to list of floats");
        }
        else
        {
            throw new InvalidOperationException("Script is trying to create a SyncedFloat without creating a list first. Insert new SyncedFloats(Behaviour); before " + CallName);
        }
    }
}

public class SyncedStrings
{
    private BasicBehaviour bb;
    private List<SyncString> m_vars = new List<SyncString>();

    public BasicBehaviour Behaviour { get => bb; }
    public static SyncedStrings Instance;

    public SyncedStrings(BasicBehaviour basic)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        bb = basic;
    }

    public void AddEntry(SyncString newEntry)
    {
        if (!Contains(newEntry.CallName))
        {
            m_vars.Add(newEntry);
            Debug.Log("Added new SyncString " + newEntry.CallName);
        } else
        {
            throw new ArgumentException("Can not sync multiple strings with the name: " + newEntry.CallName);
        }
    }

    public void RemoveEntry(SyncString targetEntry)
    {
        if (Contains(targetEntry.CallName))
        {
            m_vars.Add(targetEntry);
        }
        else
        {
            Debug.LogError("Script is trying to remove an entry that dose not exit: " + targetEntry.CallName);
        }
    }

    public SyncString GetEntry(string target)
    {
        if(Contains(target))
        {
            foreach(SyncString str in m_vars)
            {
                if(str.CallName == target)
                {
                    return str;
                }
            }
        }
        return null;
    }

    public int GetCount()
    {
        return m_vars.Count;
    }

    private bool Contains(string name)
    {
        foreach(SyncString str in m_vars)
        {
            if (str.CallName == name) return true;
        }

        return false;
    }
}

public class SyncedFloats
{
    private BasicBehaviour bb;
    private List<SyncFloat> m_vars = new List<SyncFloat>();

    public static SyncedFloats Instance;
    public BasicBehaviour Behaviour { get => bb;}

    public SyncedFloats(BasicBehaviour basic)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        bb = basic;
    }

    public void AddEntry(SyncFloat newEntry)
    {
        if (!Contains(newEntry.CallName))
        {
            m_vars.Add(newEntry);
        }
        else
        {
            throw new ArgumentException("Can not sync multiple Floats with the name: " + newEntry.CallName);
        }
    }

    public void RemoveEntry(SyncFloat targetEntry)
    {
        if (Contains(targetEntry.CallName))
        {
            m_vars.Add(targetEntry);
        }
        else
        {
            Debug.LogError("Script is trying to remove an entry that dose not exit: " + targetEntry.CallName);
        }
    }

    public SyncFloat GetEntry(string target)
    {
        if (Contains(target))
        {
            foreach (SyncFloat str in m_vars)
            {
                if (str.CallName == target)
                {
                    return str;
                }
            }
        }
        return null;
    }

    public int GetCount()
    {
        return m_vars.Count;
    }

    private bool Contains(string name)
    {
        foreach (SyncFloat str in m_vars)
        {
            if (str.CallName == name) return true;
        }

        return false;
    }
}
