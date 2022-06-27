using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using XMLData.Item;

namespace HioldMod
{
    public class APIHiold : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            //注册事件
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
        }


        private static void GameStartDone()
        {
            Harmony harmony = new Harmony("net.hiold.patch.damagefixer");
            MethodInfo original = AccessTools.Method(typeof(EntityAlive), "ProcessDamageResponseLocal");
            if (original == null)
            {
                Log.Out(string.Format("[HioldDamageFixer] 注入失败: EntityAlive.ProcessDamageResponseLocal 未找到"));
            }
            else
            {
                MethodInfo prefix = typeof(APIHiold).GetMethod("ProcessDamageResponseLocal_fix");
                if (prefix == null)
                {
                    Log.Out(string.Format("[HioldDamageFixer] 注入失败: Injections.SendXmlsToClient_postfix"));
                    return;
                }
                harmony.Patch(original, new HarmonyMethod(prefix), null);
            }

            /*
            Log.Out("[HioldDamageFixer] Hook了原版");
            Harmony harmony = new Harmony("net.hiold.patch.damagefixer");
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly anticheat = null;
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].GetName().Name.Contains("Naiwazi_AntiCheat"))
                {
                    Log.Out(string.Format("[HioldDamageFixer] 发现Naiwazi_AntiCheat改用适配注入"));
                    anticheat = assemblies[i];
                }
            }

            
            if (anticheat != null)
            {
                Type zHandler = anticheat.GetType("NAIWAZI.AntiCheat.z", false, true);
                MethodInfo original = AccessTools.Method(zHandler, "b03c91d7970040f397b608a7fcc3f2c1");
                if (original == null)
                {
                    Log.Out(string.Format("[HioldDamageFixer] 注入失败: NAIWAZI.AntiCheat.z.b03c91d7970040f397b608a7fcc3f2c1 未找到"));
                }
                else
                {
                    MethodInfo prefix = typeof(APIHiold).GetMethod("b03c91d7970040f397b608a7fcc3f2c1_fix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[HioldDamageFixer] 注入失败: APIHiold.b03c91d7970040f397b608a7fcc3f2c1_fix"));
                        return;
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }
                Log.Out("[HioldDamageFixer] Hook了NAIWAZI.AntiCheat");

            }
            else
            {
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
                Log.Out("[HioldDamageFixer] Hook了原版");
            }
            */
            Log.Out("[HioldDamageFixer] Hook初始化完毕");
        }



        public static void b03c91d7970040f397b608a7fcc3f2c1_fix(ref DamageResponse _dmResponse)
        {
            Log.Out("[HioldDamageFixer] 发现溢出的伤害，拦截并修正数据");
            Log.Out("[HioldDamageFixer] 伤害值 " + _dmResponse.Strength);
            var ownerEntityId = Traverse.Create(_dmResponse.Source).Field("ownerEntityId").GetValue<int>();
            EntityPlayer _player = (EntityPlayer)(EntityPlayer)GameManager.Instance.World.GetEntity(ownerEntityId);
            int damage = (int)EffectManager.GetValue(PassiveEffects.EntityDamage, _player.inventory.holdingItemItemValue, 0f, _player, null, default(FastTags), true, true, true, true, 1, true);
            Log.Out("[HioldDamageFixer] 修正 " + damage);
            if (damage == 65535)
            {
                damage++;
            }
            _dmResponse.Strength = damage;
        }


        public static void ProcessDamageResponseLocal_fix(ref DamageResponse _dmResponse)
        {
            if (_dmResponse.Strength == 65535)
            {
                Log.Out("[HioldDamageFixer] 发现溢出的伤害，拦截并修正数据");
                Log.Out("[HioldDamageFixer] 伤害值 " + _dmResponse.Strength);
                var ownerEntityId = Traverse.Create(_dmResponse.Source).Field("ownerEntityId").GetValue<int>();
                EntityPlayer _player = (EntityPlayer)(EntityPlayer)GameManager.Instance.World.GetEntity(ownerEntityId);
                int damage = (int)EffectManager.GetValue(PassiveEffects.EntityDamage, _player.inventory.holdingItemItemValue, 0f, _player, null, default(FastTags), true, true, true, true, 1, true);
                Log.Out("[HioldDamageFixer] 修正 " + damage);
                if (damage == 65535)
                {
                    damage++;
                }
                _dmResponse.Strength = damage;
            }
        }

        public static bool ProcessDamageResponse_fix(DamageResponse _dmResponse, EntityAlive __instance)
        {
            //修正数据
            if (_dmResponse.Strength == 65535)
            {
                Log.Out("[HioldDamageFixer] 发现溢出的伤害，拦截并修正数据");
                Log.Out("[HioldDamageFixer] 伤害值 " + _dmResponse.Strength);
                var ownerEntityId = Traverse.Create(_dmResponse.Source).Field("ownerEntityId").GetValue<int>();
                EntityPlayer _player = (EntityPlayer)__instance.world.GetEntity(ownerEntityId);
                int damage = (int)EffectManager.GetValue(PassiveEffects.EntityDamage, _player.inventory.holdingItemItemValue, 0f, _player, null, default(FastTags), true, true, true, true, 1, true);
                Log.Out("[HioldDamageFixer] 修正 " + damage);
                if (damage == 65535)
                {
                    damage++;
                }
                _dmResponse.Strength = damage;
                __instance.ProcessDamageResponse(_dmResponse);
                return false;
            }
            else
            {
                return true;
            }

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