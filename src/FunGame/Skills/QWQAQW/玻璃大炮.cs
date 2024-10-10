﻿using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.Oshima.Core.FunGame.Skills
{
    public class 玻璃大炮 : Skill
    {
        public override long Id => (long)PassiveID.玻璃大炮;
        public override string Name => "玻璃大炮";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 玻璃大炮(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 玻璃大炮特效(this));
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 玻璃大炮特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"生命值高于 30% 时，受到额外的 [ {高于30额外伤害下限}~{高于30额外伤害上限}% ] 伤害，但是获得 [ 累计所受伤害的 {高于30的加成下限}~{高于30的加成上限}%  ] 伤害加成；生命值低于等于 30% 时，不会受到额外的伤害，仅能获得 [ 累计受到的伤害 {低于30的加成下限}~{低于30的加成上限}% ] 伤害加成。" +
            $"在没有受到任何伤害的时候，将获得 {常规伤害加成 * 100:0.##}% 伤害加成。" + (累计受到的伤害 > 0 ? $"（当前累计受到伤害：{累计受到的伤害}）" : "");
        public override bool TargetSelf => true;

        private double 累计受到的伤害 = 0;
        private double 这次的伤害加成 = 0;
        private double 受到伤害之前的HP = 0;
        private double 这次受到的额外伤害 = 0;
        private readonly double 常规伤害加成 = 0.35;
        private readonly int 高于30额外伤害上限 = 40;
        private readonly int 高于30额外伤害下限 = 20;
        private readonly int 高于30的加成上限 = 100;
        private readonly int 高于30的加成下限 = 80;
        private readonly int 低于30的加成上限 = 60;
        private readonly int 低于30的加成下限 = 40;

        private double 伤害加成(double damage)
        {
            double 系数 = 常规伤害加成;
            Character? character = Skill.Character;
            if (character != null && 累计受到的伤害 != 0)
            {
                if (character.HP > character.MaxHP * 0.3)
                {
                    系数 = Calculation.Round4Digits(1.0 + (new Random().Next(高于30的加成下限, 高于30的加成上限) + 0.0) / 100);
                }
                else
                {
                    系数 = Calculation.Round4Digits(1.0 + (new Random().Next(低于30的加成下限, 低于30的加成上限) + 0.0) / 100);
                }
                return Calculation.Round2Digits(系数 * 累计受到的伤害);
            }
            return Calculation.Round2Digits(系数 * damage);
        }

        public override void AlterExpectedDamageBeforeCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType)
        {
            if (character == Skill.Character)
            {
                这次的伤害加成 = 伤害加成(damage);
                damage = Calculation.Round2Digits(damage + 这次的伤害加成);
                WriteLine($"[ {character} ] 发动了玻璃大炮，获得了 {这次的伤害加成} 点伤害加成！");
                累计受到的伤害 = 0;
            }

            if (enemy == Skill.Character)
            {
                受到伤害之前的HP = enemy.HP;
                if (enemy.HP > enemy.MaxHP * 0.3)
                {
                    // 额外受到伤害
                    double 系数 = Calculation.Round4Digits((new Random().Next(高于30额外伤害下限, 高于30额外伤害上限) + 0.0) / 100);
                    这次受到的额外伤害 = Calculation.Round2Digits(damage * 系数);
                    damage = Calculation.Round2Digits(damage + 这次受到的额外伤害);
                    WriteLine($"[ {enemy} ] 的玻璃大炮触发，将额外受到 {这次受到的额外伤害} 点伤害！");
                }
                else 这次受到的额外伤害 = 0;
            }
        }

        public override void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            if (enemy == Skill.Character && damageResult != DamageResult.Evaded)
            {
                累计受到的伤害 = Calculation.Round2Digits(累计受到的伤害 + damage);
                if (enemy.HP < 0 && 受到伤害之前的HP - damage + 这次受到的额外伤害 > 0)
                {
                    enemy.HP = 10;
                    WriteLine($"[ {enemy} ] 的玻璃大炮触发，保护了自己不进入死亡！！");
                }
            }
        }
    }
}
