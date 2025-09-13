using HarmonyLib;
using NekoNametagsMod.Utils;
using ResoniteModLoader;

namespace NekoNametagsMod;
public class NekoNametagsMod : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.1";
	public override string Name => "NekoNametags";
	public override string Author => "NekoSuneVR";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://nekont.nekosunevr.co.uk";

	private static void ShowLogo() {
		Logger.Raw(@"=============================================================================================================");
		Logger.Raw(@"                                  /$$   /$$ /$$$$$$$$ /$$   /$$  /$$$$$$      ");
		Logger.Raw(@"                                  | $$$ | $$| $$_____/| $$  /$$/ /$$__  $$     ");
		Logger.Raw(@"                                  | $$$$| $$| $$      | $$ /$$/ | $$  \ $$     ");
		Logger.Raw(@"                                  | $$ $$ $$| $$$$$   | $$$$$/  | $$  | $$     ");
		Logger.Raw(@"                                  | $$  $$$$| $$__/   | $$  $$  | $$  | $$     ");
		Logger.Raw(@"                                  | $$\  $$$| $$      | $$\  $$ | $$  | $$     ");
		Logger.Raw(@"                                  | $$ \  $$| $$$$$$$$| $$ \  $$|  $$$$$$/     ");
		Logger.Raw(@"                                  | __/  \__/|________/|__/  \__/ \______/     ");
		Logger.Raw(@"                                                                                                              ");
		Logger.Raw(@"                                                     /\__ /\                                                 ");
		Logger.Raw(@"                                                    /`     '\                                                ");
		Logger.Raw(@"                                                    === 0  0 ===                                             ");
		Logger.Raw(@"                                                     \   --  /                                               ");
		Logger.Raw(@"                                                     /       \                                               ");
		Logger.Raw(@"                                                    /         \                                              ");
		Logger.Raw(@"                                                   |           |                                             ");
		Logger.Raw(@"                                                   \   ||  ||  /                                             ");
		Logger.Raw(@"                                                    \_oo__oo_ /#######o                                      ");
		Logger.Raw(@"                                                                                                              ");
		Logger.Raw(@"                    NEKONameTagsRESO is Nametag MOD (OLD VRC MOD NEKONameTagsVRC)- By NekoSuneVR                 ");
		Logger.Raw($"                   Version: {VERSION_CONSTANT} | Twitch: NekoSuneVR | TikTok: NekoSuneVR | YouTube: NekoSuneVR              ");
		Logger.Raw(@"=============================================================================================================");
	}

	public override void OnEngineInit() {
		new Harmony("nekosunevr.neko-nametags").PatchAll();
		ShowLogo();
	}
}


