﻿using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.Oshima.Core.FunGame.Skills
{
    public class 血之狂欢 : Skill
    {
        public override long Id => (long)SuperSkillID.血之狂欢;
        public override string Name => "血之狂欢";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";
        public override double EPCost => 100;
        public override double CD => 45;
        public override double HardnessTime { get; set; } = 7;

        public 血之狂欢(Character? character = null) : base(SkillType.SuperSkill, character)
        {
            Effects.Add(new 血之狂欢特效(this));
        }
    }

    public class 血之狂欢特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"获得 40% 吸血，持续 {Duration} 时间。";
        public override bool TargetSelf => true;
        public override bool Durative => true;
        public override double Duration => 30;

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (character == Skill.Character && damageResult != DamageResult.Evaded && character.HP < character.MaxHP)
            {
                double 实际吸血 = Calculation.Round2Digits(0.4 * damage);
                character.HP += 实际吸血;
                WriteLine($"[ {character} ] 回复了 {实际吸血} 点生命值！");
            }
        }

        public override void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
        {
            RemainDuration = Duration;
            if (!caster.Effects.Contains(this))
            {
                caster.Effects.Add(this);
                OnEffectGained(caster);
            }
        }
    }
}
