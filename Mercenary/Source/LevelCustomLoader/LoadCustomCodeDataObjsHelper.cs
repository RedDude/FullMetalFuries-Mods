using System.Collections.Generic;
using Brawler2D;

public static class LoadCustomCodeDataObjsHelper
{
    internal static IEnumerable<object> LoadCustomCodeObjs(
        List<CustomCodeObj> codeDataList,
        GameController game,
        BrawlerScreen brawlerScreen)
    {
        CodeParser.g.ResetVariables(game);
        yield return null;
        foreach (var codeData in codeDataList)
        {
            foreach (var obj in CodeParser.g.ParseCode(codeData.code, game, brawlerScreen, codeData.objParams))
                yield return null;
        }
        for (int i = 0; i < codeDataList.Count; ++i)
        {
            codeDataList[i].Dispose();
            codeDataList[i] = null;
            yield return null;
        }
        codeDataList.Clear();
        yield return null;
    }
}
