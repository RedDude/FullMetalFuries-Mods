using System;

class CustomCodeObj
{
    public object[] objParams;
    public string code;

    public CustomCodeObj(string code, params object[] objParams)
    {
        this.objParams = objParams;
        this.code = code;
    }

    public void Dispose()
    {
        Array.Clear((Array) this.objParams, 0, this.objParams.Length);
        this.objParams = (object[]) null;
    }
}
