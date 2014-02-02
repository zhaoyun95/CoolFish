local name; 

name = GetSpellInfo(125167); 
_,_,_,_,_,_,expires = UnitBuff("player",name); 

if expires then 
expires = expires-GetTime()

if expires
< 30 then
  expires= 1
  else
  expires= 0
  end
  else
  expires= 1
  end