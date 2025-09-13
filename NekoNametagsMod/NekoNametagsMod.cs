using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Runtime.InteropServices;

using Elements.Core;

using FrooxEngine;
using FrooxEngine.UIX;

using HarmonyLib;

using NekoNametagsMod.Utils;

using Newtonsoft.Json;

using ResoniteModLoader;

namespace NekoNametagsMod;
public class NekoNametagsMod : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0";
	public override string Name => "NekoNametags";
	public override string Author => "NekoSuneVR";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://nekosunevr.co.uk";

	internal static HttpClient Http = new();
	internal static List<Json.User> CachedUsers { get; set; }

	public override void OnEngineInit() {
		new Harmony("nekosunevr.neko-nametags").PatchAll();
	}

	private static async Task LoadPlates() {
		try {
			using (WebClient wc = new WebClient()) {
				CachedUsers = JsonConvert.DeserializeObject<List<Json.User>>(wc.DownloadString("https://nekont.nekosunevr.co.uk/api/resonite/nametags"));
				Log($"Loaded {CachedUsers.Count} nametags.");
			}
		} catch (Exception ex) {
			Log($"[NekoNametags] Failed to load tags: {ex.Message}");
		}
	}

	internal static async Task EnsurePlatesLoaded() {
		if (CachedUsers == null || CachedUsers.Count == 0)
			await LoadPlates();
	}

	internal static async Task RefreshTagsAsync(UserRoot ur, User user) {
		try {
			await LoadPlates(); // always fetch latest
			await UserRoot_OnStart_Patch.AttachNametagAsync(ur, user); // rebuild the plates for this user
		} catch (Exception ex) {
			Log($"[NekoNametags] Failed to refresh nametags: {ex}");
		}
	}

	internal static void Log(string msg) => ResoniteModLoader.ResoniteMod.Msg(msg);
}

// ---------- Patch when a UserRoot is attached (spawn) ----------
[HarmonyPatch(typeof(UserRoot))]
class UserRoot_OnStart_Patch {
	[HarmonyPostfix]
	[HarmonyPatch("OnStart")]
	public static void Postfix(UserRoot __instance) {
		var user = __instance?.ActiveUser;
		if (user == null)
			return;

		// Only attach to yourself
		if (!user.IsLocalUser)   // or:  if (user.UserID != Engine.Current.LocalUser.UserID)
			return;

		_ = AttachNametagAsync(__instance, user);
	}

	public static async Task ForceRefresh(UserRoot ur) {
		var user = ur.ActiveUser;
		if (user != null && user.IsLocalUser)
			await NekoNametagsMod.RefreshTagsAsync(ur, user);
	}

	public static async Task AttachNametagAsync(UserRoot ur, User user) {
		await NekoNametagsMod.EnsurePlatesLoaded();


		var entry = NekoNametagsMod.CachedUsers
			?.FirstOrDefault(u => u.UserId == user.UserID);

		if (entry == null || entry.NamePlatesText == null || entry.NamePlatesText.Length == 0)
			return;

		// Wait for the head slot
		Slot headSlot = null;
		for (int attempts = 0; attempts < 100 && (headSlot = ur?.HeadSlot) == null; attempts++)
			await Task.Yield();

		if (headSlot == null) return;

		// Folder for all plates
		var tagsFolder = headSlot.FindChild(s => s.Name == "NekoNameTags")
						 ?? headSlot.AddSlot("NekoNameTags");

		// Clean up any old children so we don’t stack duplicates
		foreach (var child in tagsFolder.Children.ToList()) {
			if (child != null && !child.IsDestroyed)
				child.Destroy();
		}

		for (int i = 0; i < entry.NamePlatesText.Length; i++) {
			string text = entry.NamePlatesText[i];
			string plateName = $"NameTag{i + 1}";

			var plateSlot = tagsFolder.FindChild(s => s.Name == plateName);
			if (plateSlot == null || plateSlot.IsDestroyed) {
				plateSlot = tagsFolder.AddSlot(plateName);
				plateSlot.PersistentSelf = false;
				plateSlot.LocalPosition = new float3(0, 0.85f + 0.1f * i, 0);
			}

			// Billboard so it always faces camera
			var look = plateSlot.GetComponent<LookAtUser>()
				?? plateSlot.AttachComponent<LookAtUser>();

			look.TargetAtLocalUser.Value = true;      // each viewer sees it facing them

			// Optional: adjust rotation or invert if text appears backwards
			look.Invert.Value = false;                // or true if mirrored

			// Rotate 180° around the Y-axis so the text isn’t mirrored
			look.RotationOffset.Value = floatQ.Euler(0, -180f, 0); // -180°

			var rend = plateSlot.GetComponent<TextRenderer>() ?? plateSlot.AttachComponent<TextRenderer>();
			if (rend == null) continue;

			rend.Size.Value = 0.5f;
			rend.BoundsAlignment.Value = Alignment.MiddleCenter;

			//Disable Text Wrapping
			text.Replace("#rainbow#", "");
			text.Replace("#animationtag#", "");

			/*if (text.Contains("#rainbow#"))
				_ = Animations.RainbowLoop(rend, text.Replace("#rainbow#", ""));
			else if (text.Contains("#animationtag#"))
				_ = Animations.Typewriter(rend, text.Replace("#animationtag#", ""));
			else {*/
			float r = 1, g = 1, b = 1, a = 1;
				if (entry.Color != null && entry.Color.Length >= 3) {
					r = entry.Color[0] / 255f;
					g = entry.Color[1] / 255f;
					b = entry.Color[2] / 255f;
					if (entry.Color.Length >= 4)
						a = entry.Color[3] / 255f;
				}
				rend.Color.Value = new colorX(r, g, b, a);
				rend.Text.Value = text;
			//}
		}

		NekoNametagsMod.Log($"[NekoNameTagsResonite/{NekoNametagsMod.VERSION_CONSTANT}] Nametags attached for {user.UserName}");
	}
}

// ---------- Patch when the user leaves ----------
[HarmonyPatch(typeof(UserRoot), "OnDestroy")]
public static class UserLeavePatch {
	[HarmonyPrefix]
	public static void Prefix(UserRoot __instance) {
		var folder = __instance.HeadSlot?.FindChild("NekoNameTags");
		if (folder != null && !folder.IsDestroyed) {
			NekoNametagsMod.Log($"[NekoNameTagsResonite/{NekoNametagsMod.VERSION_CONSTANT}] Removing nametags for user.");
			folder.Destroy();
		} else {
			NekoNametagsMod.Log($"[NekoNameTagsResonite/{NekoNametagsMod.VERSION_CONSTANT}] No nametags found to remove.");
		}
	}
}
