using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using XMLData.Item;
using Newtonsoft.Json.Serialization;

namespace HioldMod
{

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
            try
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





                int crectDamage = 0;
                //修正strength
                if (strength == 65535)
                {
                    EntityPlayer _player = (EntityPlayer)_world.GetEntity(attackerEntityId);
                    int dmg = (int)EffectManager.GetValue(PassiveEffects.EntityDamage, _player.inventory.holdingItemItemValue, 0f, _player);
                    Log.Out("[HioldDamageFixer] 修正 " + dmg);
                    crectDamage = dmg;
                }
                else
                {
                    crectDamage = strength;
                    return true;
                }



                Entity entity = _world.GetEntity(entityId);
                if (_world == null)
                {
                    return false;
                }
                if (damageTyp == EnumDamageTypes.Falling && _world.GetPrimaryPlayer() != null && _world.GetPrimaryPlayer().entityId == entityId)
                {
                    return false;
                }

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
                        Strength = (int)crectDamage,
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
            catch (Exception e)
            {
                throw e;
                return true;
            }
        }
    }
}