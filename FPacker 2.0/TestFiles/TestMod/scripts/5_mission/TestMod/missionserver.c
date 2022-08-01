modded class MissionServer
{
    void MissionServer()
    {
        Print( "Loaded Server Test Mission" );
		GetPBOtestGame().TestPrint();
		GetPBOtestWorld().TestPrint();
		GetPBOtestMission().TestPrint();
    }
}