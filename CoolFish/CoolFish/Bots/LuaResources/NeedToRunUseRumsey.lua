local name; 

name = GetSpellInfo(45694); 
_,_,_,_,_,_,expires = UnitBuff("player",name); 


local count = 0; 
for i=0,4 do 
local numberOfSlots = GetContainerNumSlots(i); 
for j=1,numberOfSlots do 
local itemid = GetContainerItemID(i,j)
if itemid then 
if itemid ==  34832 then
local _,stackCounter = GetContainerItemInfo(i,j);
count = count + stackCounter; 
end 
end        
end 
end



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

  if count== 0 then
  expires= 0
  end