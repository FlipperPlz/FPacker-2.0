class CfgPatches
{
	class TestMod_scripts
	{
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = {"DZ_Scripts","DZ_Data"};
	};
};
class CfgVehicles
{
	class Inventory_Base;
	class TestModCube : Inventory_Base
	{
		scope = 2;
		displayName = "Cube";
		descriptionShort = "a Cube";
		model = "TestMod\data\cube.p3d";
		hiddenSelections[] = {"wood",""};
		hiddenSelectionsTextures[] = {"dz\gear\consumables\data\pile_of_planks_co.tga","#(argb,8,8,3)color(0.470588,0.470588,0.470588,1.0,co)","#(argb,4,4,2)color(0.470588,0.470588,0.470588,1.0,co)"};
		hiddenSelectionsMaterials[] = {"dz\gear\camping\data\fence_pile_of_planks.rvmat"};
		hologramMaterial = "tent_medium";
		hologramMaterialPath = "dz\gear\camping\data";
		itemSize[] = {4,4};
	};
	class TestModLine : Inventory_Base
	{
		scope = 2;
		displayName = "Line";
		descriptionShort = "a Line";
		model = "TestMod\data\line.p3d";
		hiddenSelections[] = {"wood",""};
		hiddenSelectionsTextures[] = {"dz\gear\consumables\data\pile_of_planks_co.tga","#(argb,8,8,3)color(0.470588,0.470588,0.470588,1.0,co)","#(argb,4,4,2)color(0.470588,0.470588,0.470588,1.0,co)"};
		hiddenSelectionsMaterials[] = {"dz\gear\camping\data\fence_pile_of_planks.rvmat"};
		hologramMaterial = "tent_medium";
		hologramMaterialPath = "dz\gear\camping\data";
		itemSize[] = {1,4};
	};
	class TestModSphere : Inventory_Base
	{
		scope = 2;
		displayName = "Sphere";
		descriptionShort = "a Sphere";
		model = "TestMod\data\sphere.p3d";
		hiddenSelections[] = {"wood",""};
		hiddenSelectionsTextures[] = {"dz\gear\consumables\data\pile_of_planks_co.tga","#(argb,8,8,3)color(0.470588,0.470588,0.470588,1.0,co)","#(argb,4,4,2)color(0.470588,0.470588,0.470588,1.0,co)"};
		hiddenSelectionsMaterials[] = {"dz\gear\camping\data\fence_pile_of_planks.rvmat"};
		hologramMaterial = "tent_medium";
		hologramMaterialPath = "dz\gear\camping\data";
		itemSize[] = {4,4};
	};
	class TestModTunnel : Inventory_Base
	{
		scope = 2;
		displayName = "Tunnel";
		descriptionShort = "a Tunnel";
		model = "TestMod\data\TunnelSectionsGeometry.p3d";
		hiddenSelections[] = {"wood",""};
		hiddenSelectionsTextures[] = {"dz\gear\consumables\data\pile_of_planks_co.tga","#(argb,8,8,3)color(0.470588,0.470588,0.470588,1.0,co)","#(argb,4,4,2)color(0.470588,0.470588,0.470588,1.0,co)"};
		hiddenSelectionsMaterials[] = {"dz\gear\camping\data\fence_pile_of_planks.rvmat"};
		hologramMaterial = "tent_medium";
		hologramMaterialPath = "dz\gear\camping\data";
		itemSize[] = {4,4};
	};
};