using System;
using Harmony;
using System.Reflection;
using StardewModdingAPI;

namespace OnlyIridium
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance harmony = HarmonyInstance.Create("xiaoye97.OnlyIridium");
            //Object构造函数补丁
            HarmonyMethod ctorPostfix = new HarmonyMethod(typeof(QualityFix).GetMethod("ObjectCtorPostfix"));
            var ctors = typeof(StardewValley.Object).GetConstructors();
            foreach (var ctor in ctors)
            {
                harmony.Patch(ctor, null, ctorPostfix);
            }
            //Quality属性补丁
            HarmonyMethod qualityPrefix = new HarmonyMethod(typeof(QualityFix).GetMethod("QualitySetterPrefix"));
            MethodInfo setQuality = typeof(StardewValley.Object).GetProperty("Quality").GetSetMethod();
            harmony.Patch(setQuality, qualityPrefix, null);
            //添加物品到背包补丁
            HarmonyMethod addItemToInventoryPrefix = new HarmonyMethod(typeof(QualityFix).GetMethod("AddItemToInventoryPrefix"));
            MethodInfo[] addItemToInventorys =
            {
                typeof(StardewValley.Farmer).GetMethod("addItemToInventory", new Type[] { typeof(StardewValley.Item)}) ,
                typeof(StardewValley.Farmer).GetMethod("addItemToInventory", new Type[] { typeof(StardewValley.Item), typeof(int)})
            };
            foreach (var method in addItemToInventorys)
            {
                harmony.Patch(method, addItemToInventoryPrefix, null);
            }
        }

        public class QualityFix
        {
            [HarmonyPostfix]
            public static void ObjectCtorPostfix(StardewValley.Object __instance)
            {
                __instance.quality.Value = 4;
            }

            [HarmonyPrefix]
            public static void QualitySetterPrefix(int value)
            {
                value = 4;
            }

            [HarmonyPrefix]
            public static void AddItemToInventoryPrefix(StardewValley.Item item)
            {
                if (item is StardewValley.Object)
                {
                    ((StardewValley.Object)item).quality.Value = 4;
                }
            }
        }
    }
}