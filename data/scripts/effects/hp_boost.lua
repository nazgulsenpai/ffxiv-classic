require("modifiers")

--Battle Voice grants HP_Boost and it sets max hp to 125% normal amount and heals for the difference between current
--This doesn't seem like the correct way to do this. If max HP changes between gainign and losing wont this break?
function onGain(target, effect)
    local newMaxHP = target.GetMaxHP() * 1.25;
    local healAmount = newMaxHP - target.GetMaxHP();

    target.SetMaxHP(newMaxHP);
    target.AddHP(healAmount);
end;

function onLose(target, effect)
    target.SetMaxHP(target.GetMaxHP() / 1.25);
end;