using Elements.Core;
using FrooxEngine;
using HarmonyLib;

namespace NekoNametagsMod.Utils;
[HarmonyPatch(typeof(UserRoot))]
public static class UserRootPatch {
	[HarmonyPostfix, HarmonyPatch("OnStart")]

	public static void OnSpawn(UserRoot __instance) {
		_ = SafeAttachNametagAsync(__instance);
	}
	private static async Task SafeAttachNametagAsync(UserRoot __instance) {
		try {
			var user = __instance?.ActiveUser;
			if (user == null) return;

			// Mark local client as having the mod
			if (user.IsLocalUser)
				ModHandshake.MarkLocalUser(__instance);

			// Load plates JSON if needed
			await NameTagDataLoader.EnsurePlatesLoaded();

			// Look up record by UserId
			var record = NameTagDataLoader.CachedUsers?
				.FirstOrDefault(e => e.UserId == user.UserID);

			// pick text & colour
			string[] lines;
			colorX col;

			if (record != null) {
				lines = record.NamePlatesText?.ToArray()
					?? new[] { user.UserName };

				// RGBA 0–255 -> 0–1
				if (record.Color != null && record.Color.Length >= 3) {
					float r = record.Color[0] / 255f;
					float g = record.Color[1] / 255f;
					float b = record.Color[2] / 255f;
					float a = 1f;
					if (record.Color.Length >= 4)
						a = record.Color[3] / 255f;
					col = new colorX(r, g, b, a);
				} else {
					col = colorX.White;
				}
			} else {
				lines = new[] { user.UserName };
				col = colorX.White;
			}

			// wait for head slot
			Slot head = null;
			for (int i = 0; i < 100 && (head = __instance.HeadSlot) == null; i++)
				await Task.Yield();
			if (head == null) return;

			NameTagBuilder.Create(head, lines, col);
		} catch (Exception ex) {
			Logger.Error("SafeAttachNametagAsync: ", ex);
		}
	}

	[HarmonyPrefix, HarmonyPatch("OnDestroy")]
	public static void OnLeave(UserRoot __instance) {
		var f = __instance?.HeadSlot?.FindChild("NekoNameTags");
		if (f != null && !f.IsDestroyed) f.Destroy();
	}
}
