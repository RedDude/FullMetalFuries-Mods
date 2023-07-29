using Brawler2D;
using FullModdedFuriesAPI.Mods.MercenaryClass;

public class Dialogue_Test_01 //: IDialogueObj
{
    public void RunDialogue(GameController game, int hostControllerIndex, string stageName)
    {
        DialogueScreen dialogueScreen = game.ScreenManager.getDialogueScreen();
        dialogueScreen.ClearAllDialogue();
        dialogueScreen.SetPortraits("Portrait_Shopkeep_Sprite", "", "", "Lisa", "", "");
        dialogueScreen.AddLeftDialogue(ModEntry.modHelper.Translation.Get("equipment.dummy.tris.description"), 0.0f, true);
        dialogueScreen.SetPortraits("Portrait_Shopkeep_Sprite", "", "", "Lisa", "", "");
        dialogueScreen.AddLeftDialogue(ModEntry.modHelper.Translation.Get("equipment.dummy.erin.description"), 0.0f, true);
        dialogueScreen.SetPortraits("", "", "", "", "", "");
        dialogueScreen.AddRightDialogue("LOC_ID_CUTSCENE_CAT_FOOD_3", 1.0f, true);
        game.ScreenManager.DisplayScreen(ScreenType.Dialogue, hostControllerIndex);
    }
}
