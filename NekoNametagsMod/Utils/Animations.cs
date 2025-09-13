using System.Text.RegularExpressions;

using Elements.Core;

using FrooxEngine;

namespace NekoNametagsMod.Utils {
	public static class Animations {
		private static readonly ConsoleColor[] Rainbow =
		{
			ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green,
			ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta
		};

		public static colorX ToColor(ConsoleColor c) =>
			c switch {
				ConsoleColor.Red => new colorX(1, 0, 0, 1),
				ConsoleColor.Green => new colorX(0, 1, 0, 1),
				ConsoleColor.Blue => new colorX(0, 0, 1, 1),
				ConsoleColor.Cyan => new colorX(0, 1, 1, 1),
				ConsoleColor.Magenta => new colorX(1, 0, 1, 1),
				ConsoleColor.Yellow => new colorX(1, 1, 0, 1),
				_ => colorX.White
			};

		public static async Task RainbowLoop(TextRenderer tr, string text, int delayMs = 250) {
			if (tr == null) return;
			int idx = 0;
			while (tr != null && tr.Slot != null && !tr.Slot.IsDestroyed) {
				tr.Text.Value = text;
				tr.Color.Value = ToColor(Rainbow[idx % Rainbow.Length]);
				idx++;
				await Task.Delay(delayMs);
			}
		}

		public static async Task Typewriter(TextRenderer tr, string text, int delayMs = 80) {
			if (tr == null) return;
			var clean = Regex.Replace(text, "<.*?>", "");
			int i = 0;
			while (tr != null && tr.Slot != null && !tr.Slot.IsDestroyed && i <= clean.Length) {
				for (i = 1; i <= clean.Length; i++) {
					tr.Text.Value = clean.Substring(0, i);
					await Task.Delay(delayMs);
				}
				for (i = clean.Length; i >= 0; i--) {
					tr.Text.Value = clean.Substring(0, i);
					await Task.Delay(delayMs);
				}
			}
		}
	}
}
