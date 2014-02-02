if not CoolFrame then
CoolFrame = CreateFrame("FRAME");
end


NewMessage = 0; 
CoolFrame:RegisterEvent("CHAT_MSG_WHISPER");
CoolFrame:RegisterEvent("CHAT_MSG_BN_WHISPER");

CoolFrame:SetScript("OnEvent",
function(self,event,msg,author,language,status,msgid,unk,chatline,senderguid)
NewMessage = 1; 
Message = msg;
Author = author;

end);