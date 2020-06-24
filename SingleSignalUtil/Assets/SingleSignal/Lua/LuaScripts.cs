using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

// you can customize more.
public class LuaScript
{
    public Script m_script = null;
    public DynValue m_coroutine;
    private TextAsset m_luacodeTextAsset;

    private const string MODULE_PATH = "Modules/?.lua";

    public LuaScript(string file, string[] modules = null)
    {
        m_luacodeTextAsset = Resources.Load<TextAsset>(file);
        m_script = new Script();
        m_script.Options.DebugPrint = s => { Debug.Log(s); }; // if you want the Lua print function work, you should set up this.
        ((ScriptLoaderBase)m_script.Options.ScriptLoader).ModulePaths = new string[] { MODULE_PATH };
        if (modules != null)
        {
            for(int i = 0; i< modules.Length; i++)
            {
                string m = modules[i];
                m_script.RequireModule(m);
            }
        }
    }

    // TODO:Implement loading lua modules
    public LuaScript(TextAsset textAsset)
    {
        m_luacodeTextAsset = textAsset;
    }

    public void Compile()
    {
        m_script.DoString(m_luacodeTextAsset.text);
    }
}
