using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Selimood.SpiritMechanics;

/// <summary>
/// Ruh Çiçeği seyahat ve Beyaz Taş mekanikleri.
/// Tuhaf Ruh Çiçeği E25'e, normal Ruh Çiçeği E25'ten Selim'in çiftliğine döndürür.
/// </summary>
public sealed class ModEntry : Mod
{
    private const string ModId = "Selimood.Selim";
    private const string StrangeFlowerId = ModId + "_TuhafRuhFlower";
    private const string NormalFlowerId = ModId + "_RuhFlower";
    private const string WhiteStoneId = ModId + "_WhiteStonePiece";
    private const string E25Hub = ModId + "_E25_Hub";
    private const string SelimFarm = ModId + "_SelimFarm";
    private const string TravelUnlockedKey = ModId + ".E25TravelUnlocked";
    private const string StrangeFlowerUsedKey = ModId + ".StrangeFlowerUsed";
    private const string WhiteStoneCollectedKey = ModId + ".WhiteStoneCollected";

    private ModConfig Config = null!;
    private bool pendingTravelCheck;
    private int pendingTravelTicks;
    private bool isWarping;
    private string? lastRemovedItemId;

    public override void Entry(IModHelper helper)
    {
        this.Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        helper.ConsoleCommands.Add("e25_status", "E25 seyahat ve Beyaz Taş durumunu gösterir.", this.StatusCommand);
        helper.ConsoleCommands.Add("e25_unlock", "Test için E25 sınırsız seyahatini açar.", this.UnlockCommand);

        this.Monitor.Log("Ruh Çiçeği seyahat helper kaynak projesi yüklendi.", LogLevel.Info);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.pendingTravelCheck = false;
        this.pendingTravelTicks = 0;
        this.isWarping = false;
        this.lastRemovedItemId = null;

        // Eski kayıtlarda sayaç yoksa, eldeki parçaları başlangıç değeri olarak al.
        if (!Game1.player.modData.ContainsKey(WhiteStoneCollectedKey))
        {
            int existing = this.CountWhiteStonePieces(Game1.player.Items);
            Game1.player.modData[WhiteStoneCollectedKey] = existing.ToString();
        }

        this.CheckWhiteStoneUnlock();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || this.isWarping)
            return;

        this.CheckWhiteStoneUnlock();

        if (!this.pendingTravelCheck)
            return;

        this.pendingTravelTicks++;
        if (this.pendingTravelTicks < 3)
            return;

        this.pendingTravelCheck = false;
        this.pendingTravelTicks = 0;

        // Eski item bilgisinin sonraki olaylara sarkmasını engelle.
        string? removedItem = this.lastRemovedItemId;
        this.lastRemovedItemId = null;

        if (Game1.activeClickableMenu is not null || !this.IsPlayerEating())
            return;

        this.TryTravelAfterEating(removedItem);
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer || !Context.IsWorldReady || this.isWarping)
            return;

        this.RecordWhiteStonePieces(e.Added);

        Item? removedFlower = e.Removed.LastOrDefault(item =>
            this.IsItem(item, StrangeFlowerId) || this.IsItem(item, NormalFlowerId));
        if (removedFlower is not null)
        {
            this.lastRemovedItemId = this.IsItem(removedFlower, StrangeFlowerId)
                ? StrangeFlowerId
                : NormalFlowerId;
            this.pendingTravelCheck = true;
            this.pendingTravelTicks = 0;
        }
    }

    private void TryTravelAfterEating(string? removedItemId)
    {
        if (Game1.player.currentLocation is null)
            return;

        string locationName = Game1.player.currentLocation.NameOrUniqueName;
        bool inE25 = locationName.StartsWith(ModId + "_E25_", StringComparison.Ordinal);

        if (inE25 && removedItemId == NormalFlowerId)
        {
            // Content Patcher ve helper aynı güvenli varış noktasını kullanır.
            this.Warp(SelimFarm, 46, 14);
            return;
        }

        if (!inE25 && removedItemId == StrangeFlowerId)
        {
            bool unlocked = Game1.player.modData.TryGetValue(TravelUnlockedKey, out string? value)
                && value == "true";
            bool alreadyUsed = Game1.player.modData.TryGetValue(StrangeFlowerUsedKey, out string? used)
                && used == "true";

            // İlk ziyaret ücretsizdir; sonraki ziyaretler 240 parça eşiğini ister.
            if (unlocked || !alreadyUsed || !this.Config.RequireWhiteStoneUnlock)
            {
                if (!unlocked)
                    Game1.player.modData[StrangeFlowerUsedKey] = "true";

                this.Warp(E25Hub, 47, 14);
            }
        }
    }

    private void RecordWhiteStonePieces(IEnumerable<Item> addedItems)
    {
        int added = this.CountWhiteStonePieces(addedItems);
        if (added <= 0)
            return;

        int previous = this.ReadIntModData(WhiteStoneCollectedKey);
        int total = previous + added;
        Game1.player.modData[WhiteStoneCollectedKey] = total.ToString();
        this.Monitor.Log($"Beyaz Taş ilerlemesi: {total}/{this.Config.WhiteStoneUnlockCount}.", LogLevel.Trace);
        this.CheckWhiteStoneUnlock();
    }

    private void CheckWhiteStoneUnlock()
    {
        if (!Context.IsWorldReady || Game1.player.modData.ContainsKey(TravelUnlockedKey))
            return;

        int count = this.ReadIntModData(WhiteStoneCollectedKey);
        if (count >= this.Config.WhiteStoneUnlockCount)
        {
            Game1.player.modData[TravelUnlockedKey] = "true";
            this.Monitor.Log($"E25 sınırsız seyahati açıldı ({count} Beyaz Taş Parçası).", LogLevel.Info);
            Game1.addHUDMessage(new HUDMessage("E25 seyahati kalıcı olarak açıldı.", HUDMessage.newQuest_type));
        }
    }

    private int ReadIntModData(string key)
    {
        return Game1.player.modData.TryGetValue(key, out string? value)
            && int.TryParse(value, out int result)
            ? Math.Max(0, result)
            : 0;
    }

    private int CountWhiteStonePieces(IEnumerable<Item> items)
    {
        return items.Where(item => item is not null && this.IsItem(item, WhiteStoneId))
            .Sum(item => item?.Stack ?? 0);
    }

    private void Warp(string locationName, int tileX, int tileY)
    {
        if (Game1.currentLocation is null)
            return;

        this.isWarping = true;
        try
        {
            Game1.warpFarmer(locationName, tileX, tileY, 2);
        }
        finally
        {
            this.isWarping = false;
        }
    }

    private bool IsPlayerEating()
    {
        Type type = Game1.player.GetType();
        foreach (string name in new[] { "isEating", "IsEating" })
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field?.GetValue(Game1.player) is bool fieldValue)
                return fieldValue;

            PropertyInfo? property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property?.GetValue(Game1.player) is bool propertyValue)
                return propertyValue;
        }

        foreach (string name in new[] { "itemToEat", "ItemToEat" })
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field?.GetValue(Game1.player) is Item)
                return true;

            PropertyInfo? property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property?.GetValue(Game1.player) is Item)
                return true;
        }

        return false;
    }

    private bool IsItem(Item item, string itemId)
    {
        return item.ItemId == itemId || item.QualifiedItemId == "(O)" + itemId;
    }

    private void StatusCommand(string command, string[] args)
    {
        bool unlocked = Context.IsWorldReady
            && Game1.player.modData.TryGetValue(TravelUnlockedKey, out string? value)
            && value == "true";
        int collected = Context.IsWorldReady ? this.ReadIntModData(WhiteStoneCollectedKey) : 0;
        this.Monitor.Log(
            $"E25 açık: {unlocked}; Beyaz Taş: {collected}/{this.Config.WhiteStoneUnlockCount}; "
            + $"gerçek kilit: {this.Config.RequireWhiteStoneUnlock}",
            LogLevel.Info);
    }

    private void UnlockCommand(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            this.Monitor.Log("Bu komut yalnızca kayıt açıkken kullanılabilir.", LogLevel.Warn);
            return;
        }

        Game1.player.modData[TravelUnlockedKey] = "true";
        this.Monitor.Log("Test için E25 sınırsız seyahati açıldı.", LogLevel.Info);
    }
}

public sealed class ModConfig
{
    /// <summary>Toplam 240 parça biriktirildiğinde kalıcı seyahat açılır.</summary>
    public int WhiteStoneUnlockCount { get; set; } = 240;

    /// <summary>İlk Tuhaf Ruh Çiçeği kullanımı ücretsizdir; sonraki kullanımlar eşik ister.</summary>
    public bool RequireWhiteStoneUnlock { get; set; } = true;
}
