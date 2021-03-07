// A role needs...
// - string Name
// - string Description
// - color Color
// - float SpawnChance
// - CustomRPC SetRPC

using UnityEngine;

namespace NebulaRoles.Roles
{
    public class Role
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }
        public float SpawnChance { get; set; }
        public CustomRPC SetRPC { get; set; }

        public Role()
        {
            this.Name = "NewRole";
            this.Description = "A new custom role";
            this.Color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            this.SpawnChance = 0.0f;
            this.SetRPC = CustomRPC.SetJester;
        }
    }
}