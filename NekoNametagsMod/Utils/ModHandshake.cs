using Elements.Assets;
using Elements.Core;

using FrooxEngine;

using static System.Net.WebRequestMethods;

namespace NekoNametagsMod.Utils;
public static class ModHandshake {
	private const string FLAG_NAME = "NekoNameTagsModToggle";

	// Called once when *your* UserRoot is started
	public static void MarkLocalUser(UserRoot ur) {
		if (ur?.ActiveUser == null || !ur.ActiveUser.IsLocalUser)
			return;

		// Put a small marker slot under your own root
		var marker = ur.Slot.FindChild(s => s.Name == FLAG_NAME)
					 ?? ur.Slot.AddSlot(FLAG_NAME);
		marker.PersistentSelf = false;   // don’t sync / save
	}

	// Local check: only tells *this* client whether it has the mod
	public static bool LocalHasMod(UserRoot ur) {
		return ur?.Slot?.FindChild(s => s.Name == FLAG_NAME) != null;
	}
}
