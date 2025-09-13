using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace NekoNametagsMod.Utils {
	internal static class NameTagDataLoader {
		/// <summary>Cached nametag records from the API.</summary>
		internal static List<Json.User> CachedUsers { get; private set; }

		/// <summary>
		/// Makes sure the nametag data has been loaded from the API.
		/// </summary>
		internal static async Task EnsurePlatesLoaded() {
			if (CachedUsers == null || CachedUsers.Count == 0)
				await LoadPlates();
		}

		/// <summary>
		/// Downloads and caches the nametag JSON.
		/// </summary>
		private static async Task LoadPlates() {
			try {
				using var wc = new WebClient();
				string json = await wc.DownloadStringTaskAsync(
					"https://nekont.nekosunevr.co.uk/api/resonite/nametags");

				CachedUsers = JsonConvert.DeserializeObject<List<Json.User>>(json);
				Logger.Info($"Loaded {CachedUsers?.Count ?? 0} nametags.");
			} catch (Exception ex) {
				Logger.Error($"Failed to load tags", ex);
				CachedUsers = new List<Json.User>();
			}
		}
	}
}
