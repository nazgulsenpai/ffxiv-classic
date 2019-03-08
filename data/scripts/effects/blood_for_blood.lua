require("battleUtils")

--Takes 10% of hp rounded down when using a weaponskill
--Random guess, but increases damage by 10% (12.5% traited)?
function onPreAction(effect, caster, target, skill, action, actionContainer)
    if skill.commandType == CommandType.Weaponskill then
        local hpToRemove = math.floor(caster.GetHP() * 0.10);
        local modifier = 1.10;

        if effect.GetTier() == 2 then
            modifier = 1.125;
        end

        action.amount = action.amount * modifier;
        caster.DelHP(hpToRemove);

        --Remove status and add message 
        actionContainer.AddAction(target.statusEffects.RemoveForBattleAction(effect));
    end
end;