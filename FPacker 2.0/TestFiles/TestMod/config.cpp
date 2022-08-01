class CfgPatches
{
	class TestMod_scripts
	{
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = {"DZ_Data"};
	};
};
class CfgMods
{
	class TestMod
	{
		dir = "TestMod";
		picture = "";
		action = "";
		hideName = 0;
		hidePicture = 1;
		name = "TestMod";
		credits = "";
		author = "Xairo";
		authorID = "0";
		version = "0.1";
		extra = 0;
		type = "mod";
		dependencies[] = {"Engine","Game","World","Mission"};
        class defs
        {
            class engineScriptModule
            {
                value = "";
                files[]={"TestMod/scripts/0_shared", "TestMod/scripts/1_Core", "TestMod/scripts/com1", "TestMod/scripts/lpt1"};
            };
            class gameScriptModule
            {
                value="";
                files[] ={"TestMod/scripts/0_shared", "TestMod/scripts/3_Game", "TestMod/scripts/com3", "TestMod/scripts/lpt3"};
            };            
            class worldScriptModule
            {
                value="";
                files[]= {"TestMod/scripts/0_shared", "TestMod/scripts/4_World", "TestMod/scripts/com4", "TestMod/scripts/lpt4"};
            };
            class missionScriptModule
            {
                value="";
                files[] = 
                {
                	"TestMod/scripts/0_shared","TestMod/scripts/5_Mission","TestMod/scripts/com5", "TestMod/scripts/lpt5"
                };
            };
        };
	};
};
