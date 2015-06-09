using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace CloneOrb2
{
    class Program
    {
        public static Menu Config;
        static Obj_AI_Base clone;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("Clone Orbwalker", "CloneOrb");
            Config.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            file.WriteLine(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name);
        }
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.Name.Equals(Player.Name))
            {
                return;
            }

            clone = sender as Obj_AI_Base;
            file.WriteLine(clone.Name);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.Name.Equals(Player.Name))
            {
                return;
            }

            clone = null;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                var pet = clone; //Player.Pet as Obj_AI_Base;
                if (pet == null || !pet.IsValid || pet.IsDead || pet.Health < 1)
                {
                    return;
                }

                var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical, true, null, pet.Position);
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, pet.Position);
                if ((pet.CanAttack && !pet.IsWindingUp && !pet.Spellbook.IsAutoAttacking &&
                      target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(pet))))
                {
                    pet.IssueOrder(GameObjectOrder.AutoAttackPet, target);
                }
                else
                {
                    pet.IssueOrder(GameObjectOrder.MovePet, (pet.Position + Orbwalking.GetRealAutoAttackRange(pet) * ((pet.Position - target.Position).Normalized())));
                }
            }
            catch { }
        }
    }
}
