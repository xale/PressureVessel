using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace xale.Subnautica.PressureVessel.Craftables;

public class VehicleModuleBuilder
{
    private PrefabInfo prefabInfo;

    private TechType cloneTemplateTechType = TechType.VehicleStorageModule;

    private EquipmentType equipmentType = EquipmentType.VehicleModule;
    private QuickSlotType quickSlotType = QuickSlotType.Passive;

    private List<CraftData.Ingredient> recipeIngredients;
    private int craftAmount = 1;
    private List<TechType> linkedItems = new List<TechType>();
    private string[] fabricatorPath;
    private float craftingTime;

    private Action<Vehicle, int> onAdded;
    private Action<Vehicle, int> onRemoved;
    private Action<Vehicle, int, float, float> onUsed;

    private VehicleModuleBuilder() { }

    public static VehicleModuleBuilder WithTechType(
        String techTypeId, String techTypeName, String techTypeDescription)
    {
        VehicleModuleBuilder vehicleModuleBuilder = new VehicleModuleBuilder();
        vehicleModuleBuilder.prefabInfo =
            PrefabInfo.WithTechType(techTypeId, techTypeName, techTypeDescription);
        return vehicleModuleBuilder;
    }

    public VehicleModuleBuilder SetIcon(Atlas.Sprite icon)
    {
        this.prefabInfo.WithIcon(icon);
        return this;
    }

    public VehicleModuleBuilder SetCloneTemplateTechType(TechType cloneTemplateTechType)
    {
        this.cloneTemplateTechType = cloneTemplateTechType;
        return this;
    }

    public VehicleModuleBuilder SetEquipmentType(EquipmentType equipmentType)
    {
        this.equipmentType = equipmentType;
        return this;
    }

    public VehicleModuleBuilder SetQuickSlotType(QuickSlotType quickSlotType)
    {
        this.quickSlotType = quickSlotType;
        return this;
    }

    public VehicleModuleBuilder SetRecipeIngredients(params CraftData.Ingredient[] ingredients)
    {
        this.recipeIngredients = new List<CraftData.Ingredient>(ingredients);
        return this;
    }

    public VehicleModuleBuilder SetCraftAmount(int craftAmount)
    {
        this.craftAmount = craftAmount;
        return this;
    }

    public VehicleModuleBuilder SetLinkedItems(params TechType[] linkedItems)
    {
        this.linkedItems = new List<TechType>(linkedItems);
        return this;
    }

    public VehicleModuleBuilder SetFabricatorPath(params string[] path)
    {
        this.fabricatorPath = path;
        return this;
    }

    public VehicleModuleBuilder SetCraftingTimeSeconds(float craftingTime)
    {
        this.craftingTime = craftingTime;
        return this;
    }

    public VehicleModuleBuilder SetOnModuleAdded(Action<Vehicle, int> onAdded)
    {
        this.onAdded = onAdded;
        return this;
    }

    public VehicleModuleBuilder SetOnModuleRemoved(Action<Vehicle, int> onRemoved)
    {
        this.onRemoved = onRemoved;
        return this;
    }

    public VehicleModuleBuilder SetOnModuleUsed(Action<Vehicle, int, float, float> onUsed)
    {
        this.onUsed = onUsed;
        return this;
    }

    public TechType RegisterAndGetTechType()
    {
        CustomPrefab prefab = new CustomPrefab(this.prefabInfo);
        prefab.SetGameObject(new CloneTemplate(this.prefabInfo, this.cloneTemplateTechType));
        prefab
            // TODO(xale): modified for debugging - revert following Nautilus fix
            .SetVehicleUpgradeModule(
                EquipmentType.SeamothModule, this.quickSlotType)
            // .SetVehicleUpgradeModule(
            //     CheckNotNull(this.equipmentType), CheckNotNull(this.quickSlotType))
            .WithOnModuleAdded(this.onAdded)
            .WithOnModuleRemoved(this.onRemoved)
            .WithOnModuleUsed(this.onUsed);

        RecipeData recipe = new RecipeData()
        {
            Ingredients =
                new List<CraftData.Ingredient>(
                    CheckNotEmpty<List<CraftData.Ingredient>, CraftData.Ingredient>(
                        this.recipeIngredients)),
            craftAmount = this.craftAmount,
            LinkedItems = new List<TechType>(this.linkedItems),
        };
        prefab.SetRecipe(recipe)
            .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
            .WithStepsToFabricatorTab(CheckNotEmpty(this.fabricatorPath))
            .WithCraftingTime(CheckNotNull(this.craftingTime));

        prefab.Register();

        TechType techType = this.prefabInfo.TechType;
        CraftDataHandler.RemoveFromGroup(
            TechGroup.Resources, TechCategory.BasicMaterials, techType);
        CraftDataHandler.AddToGroup(
            TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, techType);

        return techType;
    }

    private static T CheckNotNull<T>([NotNull] T value)
    {
        if (value != null) { return value; }
        throw new NullReferenceException("Expected non-null value");
    }

    private static E CheckNotEmpty<E,V>([NotNull] E enumerable) where E : IEnumerable<V>
    {
        if (CheckNotNull(enumerable).Count() > 0) { return enumerable; }
        throw new ArgumentException("Expected non-empty collection");
    }

    private static V[] CheckNotEmpty<V>([NotNull] V[] items)
    {
        if (CheckNotNull(items).Length > 0) { return items; }
        throw new ArgumentException("Expected non-empty array");
    }
}
