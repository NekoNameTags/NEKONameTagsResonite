using Elements.Core;

using FrooxEngine;

namespace NekoNametagsMod.Utils;
public static class NameTagBuilder {
	public static void Create(Slot headSlot, string[] lines, colorX col) {
		// Create/find folder under head for nametags
		var folder = headSlot.FindChild(s => s.Name == "NekoNameTags")
					 ?? headSlot.AddSlot("NekoNameTags", true);

		// Clear old children
		foreach (var child in folder.Children.ToArray())
			if (child != null && !child.IsDestroyed)
				child.Destroy();

		for (int i = 0; i < lines.Length; i++) {
			var plate = folder.AddLocalSlot($"NameTag{i + 1}", true);
			plate.LocalPosition = new float3(0, 0.85f + 0.1f * i, 0);

			// make text always face local camera
			var look = plate.AttachComponent<LookAtUser>();
			look.TargetAtLocalUser.Value = true;
			look.RotationOffset.Value = floatQ.Euler(0, -180, 0);

			var text = plate.AttachComponent<TextRenderer>();
			text.Text.Value = lines[i];
			text.Color.Value = col;
			text.BoundsAlignment.Value = Alignment.MiddleCenter;
			text.Size.Value = 0.5f;
		}
	}
}
