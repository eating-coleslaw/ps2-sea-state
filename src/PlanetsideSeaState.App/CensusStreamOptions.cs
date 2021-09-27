using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App
{
    public class CensusStreamOptions
    {
        public const string CensusStream = "CensusStream";


        public bool DisableWebsocketMonitor { get; set; } = false;
        public bool DisableEventsRepository { get; set; } = false;

        public IEnumerable<string> Services { get; set; }
        public IEnumerable<string> Worlds { get; set; }
        public IEnumerable<string> Characters { get; set; }
        public bool LogicalAndCharactersWithWorlds { get; set; } = true;

        public IEnumerable<string> ExperienceIds { get; set; } //= new[]
        //{
        //    "7",      // Revive
        //    "2",      // Kill Player Assist
        //    "5",      // Heal Assis
        //    "15",     // Control Point - Defend
        //    "16",     // Control Point - Attack
        //    "36",     // Spot Kill
        //    "53",     // Squad Revive
        //    "54",     // Squad Spot Kill
        //    "56",     // Squad Spawn
        //    "293",    // Motion Detect
        //    "294",    // Squad Motion Spot
        //    "438",    // Shield Repair - other ID is player whose shield was repaired?
        //    "439",    // Squad Shield Repair - other ID is player whose shield was repaired?
        //    "550",    // Concussion Grenade Assist
        //    "551",    // Concussion Grenade Squad Assist
        //    "552",    // EMP Grenade Assist
        //    "553",    // EMP Grenade Squad Assist
        //    "554",    // Flashbang Assist
        //    "555",    // Flashbang Squad Assist
        //    "556",    // Objective Pulse Defend
        //    "557",    // Objective Pulse Capture
        //    "1393",   // Hardlight Cover - Blocking Exp (other ID is player who shot the barrier)

        //    // "272", // Convert Capture Point
        //    // "1394" // Draw Fire Award - doesn't seem to actually get sent
        //};
    }
}
