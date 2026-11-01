using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Selimood.SpiritMechanics;

/// <summary>
/// Ruh Çiçeği mekaniklerinin kaynak proje girişi.
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

    private ModConfig Config = null!;
    private bool pendingTravelCheck;
    private int pendingTravelTicks;
    private bool isWarping;

    public override void Entry(IModHelper helper)
    {
        this.Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        helper.ConsoleCommands.Add("e25_status", "E25 seyahat durumunu gösterir.", this.StatusCommand);
        helper.ConsoleCommands.Add("e25_unlock", "Test için E25 sınırsız seyahatini açar.", this.UnlockCommand);

        this.Monitor.Log("Ruh Çiçeği seyahat helper kaynak projesi yüklendi.", LogLevel.Info);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.pendingTravelCheck = false;
        this.pendingTravelTicks = 0;
        this.isWarping = false;
        this.CheckWhiteStoneUnlock();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || this.isWarping)
            return;

        this.CheckWhiteStoneUnlock();

        if (!this.pendingTravelCheck)
            return;

        // InventoryChanged fires when an item is removed. Delay by a few ticks so
        // eating animation/menu state can be observed before deciding to warp.
        this.pendingTravelTicks++;
        if (this.pendingTravelTicks < 3)
            return;

        this.pendingTravelCheck = false;
        this.pendingTravelTicks = 0;

        if (Game1.activeClickableMenu is not null || !this.IsPlayerEating())
            return;

        // The item-use hook is intentionally conservative. If the runtime exposes
        // neither eating flag nor itemToEat, no accidental warp is performed.
        this.TryTravelAfterEating();
        this.lastRemovedItemId = null;
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer || !Context.IsWorldReady || this.isWarping)
            return;

        Item? removedFlower = e.Removed.LastOrDefault(item => this.IsItem(item, StrangeFlowerId) || this.IsItem(item, NormalFlowerId));
        if (removedFlower is not null)
        {
            this.lastRemovedItemId = this.IsItem(removedFlower, StrangeFlowerId) ? StrangeFlowerId : NormalFlowerId;
            this.pendingTravelCheck = true;
            this.pendingTravelTicks = 0;
        }
    }

    private void TryTravelAfterEating()
    {
        if (Game1.player.currentLocation is null)
            return;

        string locationName = Game1.player.currentLocation.NameOrUniqueName;
        bool inE25 = locationName.StartsWith(ModId + "_E25_", StringComparison.Ordinal);

        // Normal flower returns the player from any E25 region to the farm.
        if (inE25 && this.LastRemovedWas(NormalFlowerId))
        {
            this.Warp(SelimFarm, 25, 16);
            return;
        }

        // Strange flower opens E25 once; after 240 White Stone Pieces the
        // persistent unlock allows repeated travel.
        if (!inE25 && this.LastRemovedWas(StrangeFlowerId))
        {
            bool unlocked = Game1.player.modData.TryGetValue(TravelUnlockedKey, out string? value) && value == "true";
            bool alreadyUsed = Game1.player.modData.TryGetValue(StrangeFlowerUsedKey, out string? used) && used == "true";
            if (unlocked || !alreadyUsed || !this.Config.RequireWhiteStoneUnlock)
            {
                if (!unlocked)
                    Game1.player.modData[StrangeFlowerUsedKey] = "true";
                this.Warp(E25Hub, 46, 14);
            }
        }
    }

    private string? lastRemovedItemId;

    private bool LastRemovedWas(string itemId) => this.lastRemovedItemId == itemId;

    private void CheckWhiteStoneUnlock()
    {
        if (!Context.IsWorldReady || Game1.player.modData.ContainsKey(TravelUnlockedKey))
            return;

        int count = Game1.player.Items
            .Where(item => item is not null && this.IsItem(item, WhiteStoneId))
            .Sum(item => item?.Stack ?? 0);

        if (count >= Config.WhiteStoneUnlockCount)
        {
            Game1.player.modData[TravelUnlockedKey] = "true";
            this.Monitor.Log($"E25 sınırsız seyahati açıldı ({count} Beyaz Taş Parçası).", LogLevel.Info);
            Game1.addHUDMessage(new HUDMessage("E25 seyahati kalıcı olarak açıldı.", HUDMessage.newQuest_type));
        }
    }

    private void Warp(string locationName, int tileX, int tileY)
    {
        if (Game1.currentLocation is null)
            return;

        this.isWarping = true;
        Game1.warpFarmer(locationName, tileX, tileY, 2);
        this.isWarping = false;
    }

    private bool IsPlayerEating()
    {
        // Stardew revisions expose this state as either a field or property.
        // Reflection keeps the source compatible with Android SMAPI builds where
        // the member visibility/name differs.
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

        // Some Stardew builds expose the pending food as itemToEat instead of
        // exposing a readable eating flag.
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
        bool unlocked = Context.IsWorldReady && Game1.player.modData.TryGetValue(TravelUnlockedKey, out string? value) && value == "true";
        this.Monitor.Log($"E25 açık: {unlocked}; Beyaz Taş eşiği: {this.Config.WhiteStoneUnlockCount}; test erişimi: {this.Config.AllowFarmTestWarp}", LogLevel.Info);
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
    /// <summary>240 parçaya ulaşıldığında kalıcı seyahat açılır.</summary>
    public int WhiteStoneUnlockCount { get; set; } = 240;

    /// <summary>Android testinde doğrudan çiftlik test warp'ını açık tutar.</summary>
    public bool AllowFarmTestWarp { get; set; } = true;

    /// <summary>Gerçek tasarım kilidini test etmek için true yapılabilir.</summary>
    public bool RequireWhiteStoneUnlock { get; set; } = false;
}
