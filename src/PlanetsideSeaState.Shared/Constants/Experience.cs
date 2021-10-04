using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Shared.Constants
{
    public sealed class Experience
    {
        public const int Revive = 7;
        public const int KillPlayerAssist = 2;
        public const int HealAssist = 5;
        public const int ControlPointDefend = 15;
        public const int ControlPointAttack = 16;
        public const int Resupply = 34;
        public const int SpotKill = 36;
        public const int SquadHeal = 51;
        public const int SquadRevive = 53;
        public const int SquadSpotKill = 54;
        public const int SquadResupply = 55;
        public const int ShieldRepair = 438;
        public const int SquadShieldRepair = 439;
        public const int ConcussionGrenadeAssist = 550;
        public const int ConcussionGrenadeSquadAssist = 551;
        public const int EmpGrenadeAssist = 552;
        public const int EmpGrenadeSquadAssist = 553;
        public const int FlashbangAssist = 554;
        public const int FlashbangSquadAssist = 555;

        public static bool IsHeal(int expId)
        {
            return expId == HealAssist || expId == SquadHeal;
        }

        public static bool IsRevive(int expId)
        {
            return expId == Revive || expId == SquadRevive;
        }

        public static bool IsResupply(int expId)
        {
            return expId == Resupply || expId == SquadResupply;
        }

        public static bool IsFriendlyIdentifier(int expId)
        {
            return IsHeal(expId) || IsRevive(expId) || IsResupply(expId);
        }
    }
}
