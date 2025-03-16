using MelonLoader;
using CustomGameModesFramework;
using CustomGameModesFramework.Patches;
using Il2Cpp;
using UnityEngine;
using Il2CppCodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using HarmonyLib;

[assembly: MelonInfo(typeof(RocketRun.Main), "RocketRun!", "1.0.0", "MasterHell", null)]
[assembly: MelonGame("ZeoWorks", "Slendytubbies 3")]
[assembly: MelonColor(1, 255, 0, 0)]

namespace RocketRun
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }

    [GameModeInfo("ROCKET RUN!", "ROCKET RUN!", "12", "1-2", "1-6", "1-12")]
    public class RocketRun : CustomGameMode
    {
        private string _gameMode;

        public override void OnStart(RoomMultiplayerMenu instance)
        {
            _gameMode = StartPatch.GameMode;
        }

        public override void OnSetupRoom(RoomMultiplayerMenu instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            RespawnPlayerGUIPatch.ShowRespawnText = false;
            FriendlyFirePatch.IsFriendlyFireDisabled = false;
            OnGUIPatch.ShowLeaderText = true;
            OnGUIPatch.IsWaitingForPlayers = true;
            OnGUIPatch.RoundEndedText = $"{instance.HGKEEDKPGOD} Wins";
            instance.INBHDPAHOGC();
        }

        public override void OnStartGame(RoomMultiplayerMenu instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            instance.DKOBDBGFJMO = true;
            instance.INCDKGADOBA = new ObscuredString[] { "RPG" };
            instance.SpawnPlayer(instance.OJPBAOICLJK.teamName);
            GUI.enabled = false;
            instance.IOMIAHNBDOG = false;
        }

        public override void OnSpawnPlayer(RoomMultiplayerMenu instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            int index = UnityEngine.Random.Range(0, instance.OJPBAOICLJK.spawnPoints.Length);
            instance.CNLHJAICIBH = PhotonNetwork.NOOU(instance.BGOEEADMCBE.name, instance.OJPBAOICLJK.spawnPoints[index].position,
                instance.OJPBAOICLJK.spawnPoints[index].rotation, 0);
            instance.CNLHJAICIBH.name = PhotonNetwork.player.name;
            InitializePlayerSettings(instance);
            MelonCoroutines.Start(InitializeWeaponSettingsAsync(instance));
        }

        public override void OnRespawnPlayer(RagdollController instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            GameObject.FindWithTag("Network").SendMessage("SpawnPlayer", "Team A");
        }

        public override void OnRespawnPlayerGUI(RagdollController instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 15, 150f, 30f), $"Respawn in: {instance?.NADEDDKIIJL}");
        }

        public override void OnFixedUpdate(RoomMultiplayerMenu instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            if (string.IsNullOrEmpty(instance?.HGKEEDKPGOD))
                OnGUIPatch.RoundEndedText = "Masuka Wins :)";
            else
                OnGUIPatch.RoundEndedText = $"{instance.HGKEEDKPGOD} Wins";
        }

        public override void OnQuitRoom(RoomMultiplayerMenu instance)
        {
            if (_gameMode != "ROCKET RUN!")
                return;

            FriendlyFirePatch.IsFriendlyFireDisabled = true;
            OnGUIPatch.ShowLeaderText = false;
            OnGUIPatch.IsWaitingForPlayers = false;
        }

        private void InitializePlayerSettings(RoomMultiplayerMenu instance)
        {
            if (instance == null)
                return;

            FPScontroller fpsController = instance.CNLHJAICIBH.GetComponent<FPScontroller>();

            if (fpsController == null)
                return;

            fpsController.HKCDMBALAAK.WalkSpeed = 8f;
            fpsController.HKCDMBALAAK.RunSpeed = 15f;
            fpsController.AALHECCKHFD.baseHeight = 3f;
        }

        private IEnumerator InitializeWeaponSettingsAsync(RoomMultiplayerMenu instance)
        {
            yield return new WaitForSeconds(0.2f);

            FPScontroller fpsController = instance?.CNLHJAICIBH.GetComponent<FPScontroller>();
            WeaponManager weaponManager = fpsController?.GetComponent<WeaponPickUp>()?.AKBOKOLNGGL;
            WeaponScript weaponScript = weaponManager?.FGGENNCGLJI;

            if (weaponScript == null)
                yield break;

            weaponScript.MJIKHNADGAG.reloadTime = 0.5f;
            weaponScript.MJIKHNADGAG.ammoCount = int.MaxValue;

            yield break;
        }
    }

    [HarmonyPatch(typeof(ExplosionDamage), "AGIIFJLGHJB")]
    public class RPGDamageFix
    {
        [HarmonyPrefix]
        public static bool Prefix(ExplosionDamage __instance)
        {
            foreach (Collider collider in Physics.OverlapSphere(__instance.GFLEMICGHHP, __instance.OPGAMEEGIPH))
            {
                if (collider == null) continue;

                float distanceToCollider = Vector3.Distance(collider.ClosestPointOnBounds(__instance.GFLEMICGHHP), __instance.GFLEMICGHHP);
                float damageMultiplier = 1f - Mathf.Clamp01(distanceToCollider / __instance.OPGAMEEGIPH);
                float scaledDamage = damageMultiplier * __instance.FABMEIDIGFC;

                if (scaledDamage < 10f)
                    scaledDamage += 3f;

                if (collider.gameObject.CompareTag("Remote"))
                    ApplyDamage(collider, scaledDamage);
            }

            return true;
        }

        private static void ApplyDamage(Collider collider, float damage)
        {
            PlayerDamage playerDamage = collider?.GetComponent<PlayerDamage>();
            playerDamage?.E100050(damage, string.Empty);
        }
    }
}