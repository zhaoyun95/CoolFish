local _, englishClass = UnitClass("player") 
local name; 

if englishClass == "SHAMAN" then 
name = GetSpellInfo(546)
TargetUnit("player") 
CastSpellByName(name)
TargetLastTarget()
else 
if englishClass == "DEATHKNIGHT" then 
name = GetSpellInfo(3714) 
TargetUnit("player") 
CastSpellByName(name)
TargetLastTarget()
else 
if englishClass == "WARLOCK" and GetSpecialization() == 1 then 
if UnitPower("player",7)
< 1  then
  name= GetSpellInfo(101976)
  CastSpellByName( name)
  while UnitChannelInfo("player") do
  end

  name= GetSpellInfo(74434);
  CastSpellByName( name)
  _, duration= GetSpellCooldown(74434)

  while duration>
  0 do
  _,duration = GetSpellCooldown(74434)
  end

  name = GetSpellInfo(5697)
  TargetUnit("player")
  CastSpellByName(name)
  TargetLastTarget()

  end

  else
  if englishClass == "PRIEST" then
  name = GetSpellInfo(1706)
  TargetUnit("player")
  CastSpellByName(name)
  TargetLastTarget()
  else
  name = GetSpellInfo(124036);
  RunMacroText("/use " .. name)
  end
  end
  end
  end