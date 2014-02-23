LureName = nil; 
for i=0,4 do numberOfSlots = GetContainerNumSlots(i); 
for j=1,numberOfSlots do itemid = GetContainerItemID(i,j) 
if itemid == 67407 or itemid == 69907 or itemid == 6529 or 
itemid == 6530 or itemid == 6811 or itemid == 7307 or 
itemid == 46006 or itemid == 6533 or itemid == 6532 or 
itemid == 34861 or itemid == 62673 or itemid == 68049 then 
            
LureName = GetItemInfo(itemid)
Count = 1
end 
end 
end