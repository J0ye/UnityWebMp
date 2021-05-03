using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncVar
{
    public string m_callName;
}

public class SyncString : SyncVar
{
    public string m_value;

    /// <summary>
    /// Standard constructor for a SyncString
    /// </summary>
    /// <param name="name">Name of the SyncString</param>
    /// <param name="value">Value of the SyncString</param>
    public SyncString(string name, string value)
    {
        if (name.Contains("SyncString") || name.Contains(":") || name.Contains("|")) 
            throw new ArgumentException("Scritp is trying to create a illigal SyncString with the name " + name);

        m_callName = name;
        m_value = value;
        AddToSyncedList();
    }
    /// <summary>
    /// Special constructor that will parse a string into a SyncString
    /// </summary>
    /// <param name="toParse">Target string. Should be written in special SyncString syntax, i.e. SyncString:name|value'</param>
    public SyncString(string toParse)
    {
        SyncString temp = Parse(toParse);
        m_callName = temp.m_callName;
        m_value = temp.m_value;
        AddToSyncedList();
    }

    /// <summary>
    /// Creates a string, that is ready to be send or to be parsed back into a SyncString
    /// </summary>
    /// <returns>string in SyncStrin syntax, i.e. SyncString:m_callName| m_value</returns>
    public override string ToString()
    {
        return "SyncString: " + m_callName + "|" + m_value;
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
                return SyncedStrings.Instance.GetEntry(split[0]);
            }
            SyncString temp = new SyncString(split[0], split[1]);
            return temp;
        }

        throw new InvalidOperationException("Script is trying to parse a string that is not a SyncString." + target + " misses 'SyncString:'  syntax.");
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
            new SyncedStrings();
            Debug.LogWarning("Script added a synced string without creating a list for them first. List created. Insert new SyncedStrings() before " + m_callName);
            SyncedStrings.Instance.AddEntry(this);
        }
    }
}

public class SyncFloat : SyncVar
{
    public float m_value;

    /// <summary>
    /// Standard constructor for a SyncFloat
    /// </summary>
    /// <param name="name">Name of the SyncFloat</param>
    /// <param name="value">Value of the SyncFloat</param>
    public SyncFloat(string name, float value)
    {
        if (name.Contains("SyncFloat") || name.Contains(":") || name.Contains("|"))
            throw new ArgumentException("Scritp is trying to create a illigal SyncFloat with the name " + name);

        m_callName = name;
        m_value = value;
        AddToSyncedList();
    }
    /// <summary>
    /// Special constructor that will parse a string into a SyncFloat
    /// </summary>
    /// <param name="toParse">Target string. Should be written in special SyncFloat syntax, i.e. SyncFloat:name|value'</param>
    public SyncFloat(string toParse)
    {
        SyncFloat temp = Parse(toParse);
        m_callName = temp.m_callName;
        m_value = temp.m_value;
        AddToSyncedList();
    }

    /// <summary>
    /// Creates a string, that is ready to be send or to be parsed back into a SyncFloat
    /// </summary>
    /// <returns>string in SyncFloat syntax, i.e. SyncFloat:m_callName|m_value</returns>
    public override string ToString()
    {
        return "SyncFloat: " + m_callName + "|" + m_value.ToString();
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
                return SyncedFloats.Instance.GetEntry(split[0]);
            }
            SyncFloat temp = new SyncFloat(split[0], float.Parse(split[1]));
            return temp;
        }

        throw new InvalidOperationException("Script is trying to parse a float that is not a SyncFloat." + target + " misses 'SyncFloat:'  syntax.");
    }

    /// <summary>
    /// Will add this SyncFloat to the SyncedFloats instance. Also handles exception, if the instance is null.
    /// </summary>
    private void AddToSyncedList()
    {
        if (SyncedFloats.Instance != null)
        {
            SyncedFloats.Instance.AddEntry(this);
        }
        else
        {
            new SyncedFloats();
            Debug.LogWarning("Script added a synced float without creating a list for them first. List created. Insert new SyncedFloats() before " + m_callName);
            SyncedFloats.Instance.AddEntry(this);
        }
    }
}

public class SyncedStrings
{
    public static SyncedStrings Instance;

    private List<SyncString> m_vars = new List<SyncString>();

    public SyncedStrings()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddEntry(SyncString newEntry)
    {
        if (!Contains(newEntry.m_callName))
        {
            m_vars.Add(newEntry);
            Debug.Log("Added new SyncString " + newEntry.m_callName);
        } else
        {
            throw new ArgumentException("Can not sync multiple strings with the name: " + newEntry.m_callName);
        }
    }

    public void RemoveEntry(SyncString targetEntry)
    {
        if (Contains(targetEntry.m_callName))
        {
            m_vars.Add(targetEntry);
        }
        else
        {
            Debug.LogError("Script is trying to remove an entry that dose not exit: " + targetEntry.m_callName);
        }
    }

    public SyncString GetEntry(string target)
    {
        if(Contains(target))
        {
            foreach(SyncString str in m_vars)
            {
                if(str.m_callName == target)
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
            if (str.m_callName == name) return true;
        }

        return false;
    }
}

public class SyncedFloats
{
    public static SyncedFloats Instance;

    private List<SyncFloat> m_vars = new List<SyncFloat>();

    public SyncedFloats()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddEntry(SyncFloat newEntry)
    {
        if (!Contains(newEntry.m_callName))
        {
            m_vars.Add(newEntry);
        }
        else
        {
            throw new ArgumentException("Can not sync multiple Floats with the name: " + newEntry.m_callName);
        }
    }

    public void RemoveEntry(SyncFloat targetEntry)
    {
        if (Contains(targetEntry.m_callName))
        {
            m_vars.Add(targetEntry);
        }
        else
        {
            Debug.LogError("Script is trying to remove an entry that dose not exit: " + targetEntry.m_callName);
        }
    }

    public SyncFloat GetEntry(string target)
    {
        if (Contains(target))
        {
            foreach (SyncFloat str in m_vars)
            {
                if (str.m_callName == target)
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
            if (str.m_callName == name) return true;
        }

        return false;
    }
}
