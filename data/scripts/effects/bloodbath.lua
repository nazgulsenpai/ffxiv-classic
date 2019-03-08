require("modifiers");

--Absorb HP on next WS or ability
function onHit(effect, attacker, defender, action, actionContainer)

    --1.21: Absorb HP amount no longer affected by player VIT rating.
    --Bloodbath seems based on both defener and attacker's stats, even after 1.21.
    --Miser's Mistriss seems to resist the effect, whereas nael gets absorbed more than 100%
    --Garuda resists a small amount
    --Unclear what it's based on.
    --Possibly magic resist? Slashing resist?

    --For now using 1.0 as baseline since that seems to be the average
    if action.commandType == CommandType.Weaponskill or action.commandType == CommandType.Ability then
        local absorbModifier = 1.0
        local absorbAmount = action.amount * absorbModifier;

        attacker.AddHP(absorbAmount);
        --30332: You absorb hp from target
        actionContainer.AddHPAction(defender.actorId, 30332, absorbAmount)
        --Bloodbath is lost after absorbing hp
        actionContainer.AddAction(defender.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;