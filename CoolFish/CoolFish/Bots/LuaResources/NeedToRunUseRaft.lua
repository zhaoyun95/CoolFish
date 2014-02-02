local _, englishClass = UnitClass("player") 
local name; 

if englishClass == "SHAMAN" then 
name = GetSpellInfo(546);  _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
expires = expires-GetTime() 
end 
else 
if englishClass == "DEATHKNIGHT" then 
name = GetSpellInfo(3714); _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
expires = expires-GetTime() 
end 
else 
if englishClass == "WARLOCK" and GetSpecialization() == 1 then 
name = GetSpellInfo(5697); _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
expires = expires-GetTime() 
end 
		
else 
if englishClass == "PRIEST" then
name = GetSpellInfo(1706); _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
expires = expires-GetTime() 
end 

else
name = GetSpellInfo(124036); _,_,_,_,_,_,expires = UnitBuff("player",name); 
if expires then 
expires = expires-GetTime() 
end 
end
end 
end 
end
if expires then 
if expires
< 30 then
  expires= 1
  else
  expires= 0
  end
  else
  expires= 1
  end