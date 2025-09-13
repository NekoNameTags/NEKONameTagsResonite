using ResoniteModLoader;

namespace NekoNametagsMod.Utils;
internal static class Logger {
	// Toggle this when you want verbose logs
	public static bool DebugEnabled { get; set; } = true;

	public static void Info(string message) {
		ResoniteMod.Msg($"[INFO] [NekoNametags] {message}");
	}

	public static void Raw(string message) {
		ResoniteMod.Msg(message);
	}

	public static void Error(string message, Exception ex = null) {
		var text = $"[ERROR] [NekoNametags] {message}";
		if (ex != null)
			text += $"\n{ex}";
		ResoniteMod.Msg(text);
	}

	public static void Debug(string message) {
		if (!DebugEnabled) return;
		ResoniteMod.Msg($"[DEBUG] [NekoNametags] {message}");
	}
}
