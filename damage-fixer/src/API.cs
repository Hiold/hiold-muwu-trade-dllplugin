using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using XMLData.Item;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HioldMod
{

    public class JsonContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {

            IList<JsonProperty> lst = base.CreateProperties(type, memberSerialization);
            List<JsonProperty> result = new List<JsonProperty>();
            foreach (JsonProperty tmp in lst)
            {
                if (!string.IsNullOrEmpty(tmp.PropertyName))
                {
                    result.Add(tmp);
                }
            }
            return result;
        }
    }

    public class APIHiold : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            Harmony harmony = new Harmony("net.hiold.patch.damagefixer");
            //xml发送拦截
            MethodInfo original = AccessTools.Method(typeof(NetPackageDamageEntity), "ProcessPackage");
            if (original == null)
            {
                Log.Out(string.Format("[HioldDamageFixer] 注入失败: NetPackageDamageEntity.ProcessPackage 未找到"));
            }
            else
            {
                MethodInfo prefix = typeof(APIHiold).GetMethod("ProcessPackage_fix");
                if (prefix == null)
                {
                    Log.Out(string.Format("[HioldDamageFixer] 注入失败: Injections.SendXmlsToClient_postfix"));
                    return;
                }
                harmony.Patch(original, new HarmonyMethod(prefix), null);
            }

            //Harmony.CreateAndPatchAll(typeof(PostPrefix));
            Log.Out("[HioldDamageFixer] Hook初始化完毕");
        }


        public static bool ProcessPackage_fix(World _world, GameManager _callbacks, NetPackageDamageEntity __instance)
        {
            Log.Out("[HioldDamageFixer] 拦截到伤害执行");
            //获取重要参数
            var damageTyp = Traverse.Create(__instance).Field("damageTyp").GetValue<EnumDamageTypes>();
            var entityId = Traverse.Create(__instance).Field("entityId").GetValue<int>();
            var damageSrc = Traverse.Create(__instance).Field("damageSrc").GetValue<EnumDamageSource>();
            var attackerEntityId = Traverse.Create(__instance).Field("attackerEntityId").GetValue<int>();
            var dirV = Traverse.Create(__instance).Field("dirV").GetValue<Vector3>();
            var hitTransformName = Traverse.Create(__instance).Field("hitTransformName").GetValue<string>();
            var hitTransformPosition = Traverse.Create(__instance).Field("hitTransformPosition").GetValue<Vector3>();
            var uvHit = Traverse.Create(__instance).Field("uvHit").GetValue<Vector2>();
            var bIgnoreConsecutiveDamages = Traverse.Create(__instance).Field("bIgnoreConsecutiveDamages").GetValue<bool>();
            var damageMultiplier = Traverse.Create(__instance).Field("damageMultiplier").GetValue<float>();
            var bIsDamageTransfer = Traverse.Create(__instance).Field("bIsDamageTransfer").GetValue<bool>();
            var bonusDamageType = Traverse.Create(__instance).Field("bonusDamageType").GetValue<byte>();
            var attackingItem = Traverse.Create(__instance).Field("attackingItem").GetValue<ItemValue>();
            var strength = Traverse.Create(__instance).Field("strength").GetValue<ushort>();
            var movementState = Traverse.Create(__instance).Field("movementState").GetValue<int>();
            var hitDirection = Traverse.Create(__instance).Field("hitDirection").GetValue<int>();
            var hitBodyPart = Traverse.Create(__instance).Field("hitBodyPart").GetValue<int>();
            var bPainHit = Traverse.Create(__instance).Field("bPainHit").GetValue<bool>();
            var bFatal = Traverse.Create(__instance).Field("bFatal").GetValue<bool>();
            var bCritical = Traverse.Create(__instance).Field("bCritical").GetValue<bool>();
            var random = Traverse.Create(__instance).Field("random").GetValue<float>();
            var bCrippleLegs = Traverse.Create(__instance).Field("bCrippleLegs").GetValue<bool>();
            var bDismember = Traverse.Create(__instance).Field("bDismember").GetValue<bool>();
            var bTurnIntoCrawler = Traverse.Create(__instance).Field("bTurnIntoCrawler").GetValue<bool>();
            var StunType = Traverse.Create(__instance).Field("StunType").GetValue<byte>();
            var StunDuration = Traverse.Create(__instance).Field("StunDuration").GetValue<float>();
            var ArmorSlot = Traverse.Create(__instance).Field("ArmorSlot").GetValue<EnumEquipmentSlot>();
            var ArmorSlotGroup = Traverse.Create(__instance).Field("ArmorSlotGroup").GetValue<EnumEquipmentSlotGroup>();
            var ArmorDamage = Traverse.Create(__instance).Field("ArmorDamage").GetValue<int>();
            Log.Out("[HioldDamageFixer] 伤害值 " + strength);




            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MaxDepth = 3;
            settings.ContractResolver = new JsonContractResolver();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;


            Log.Out("[HioldDamageFixer] __instance " + JsonConvert.SerializeObject(__instance, settings));

            if (strength == 65535)
            {
                EntityPlayer _player = (EntityPlayer)_world.GetEntity(attackerEntityId);

                Log.Out("[HioldDamageFixer] _player " + JsonConvert.SerializeObject(_player, settings));
                ItemClass icc = _player.inventory.holdingItemData.item;
                Log.Out("[HioldDamageFixer] 当前手持武器为 " + icc.GetItemName());
                Log.Out("[HioldDamageFixer] holdingItemData " + JsonConvert.SerializeObject(_player.inventory.holdingItemData, settings));
                ItemData _itemd = icc;
                DataItem<AttributesData> fixData = Traverse.Create(_itemd).Field("pAttributes").GetValue<DataItem<AttributesData>>();
                Log.Out("[HioldDamageFixer] 当前手持武器pAttributes " + fixData);
                Log.Out("[HioldDamageFixer] 当前手持武器pAttributes.Value " + fixData.Value);
                Log.Out("[HioldDamageFixer] 修正伤害值应为 " + fixData.Value.EntityDamage.Value);
            }




            if (_world == null)
            {
                return false;
            }
            if (damageTyp == EnumDamageTypes.Falling && _world.GetPrimaryPlayer() != null && _world.GetPrimaryPlayer().entityId == entityId)
            {
                return false;
            }
            Entity entity = _world.GetEntity(entityId);
            if (entity != null)
            {
                DamageSource damageSource = new DamageSourceEntity(damageSrc, damageTyp, attackerEntityId, dirV, hitTransformName, hitTransformPosition, uvHit);
                damageSource.SetIgnoreConsecutiveDamages(bIgnoreConsecutiveDamages);
                damageSource.DamageMultiplier = damageMultiplier;
                damageSource.bIsDamageTransfer = bIsDamageTransfer;
                damageSource.BonusDamageType = (EnumDamageBonusType)bonusDamageType;
                damageSource.AttackingItem = attackingItem;
                entity.ProcessDamageResponse(new DamageResponse
                {
                    Strength = (int)strength,
                    ModStrength = 0,
                    MovementState = movementState,
                    HitDirection = (Utils.EnumHitDirection)hitDirection,
                    HitBodyPart = (EnumBodyPartHit)hitBodyPart,
                    PainHit = bPainHit,
                    Fatal = bFatal,
                    Critical = bCritical,
                    Random = random,
                    Source = damageSource,
                    CrippleLegs = bCrippleLegs,
                    Dismember = bDismember,
                    TurnIntoCrawler = bTurnIntoCrawler,
                    Stun = (EnumEntityStunType)StunType,
                    StunDuration = StunDuration,
                    ArmorSlot = ArmorSlot,
                    ArmorSlotGroup = ArmorSlotGroup,
                    ArmorDamage = ArmorDamage
                });
            }
            return false;
        }
    }
}