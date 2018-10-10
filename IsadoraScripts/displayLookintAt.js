function main()
{

	var outArr = [0,0,0,0,0,0,0,0,0,0,0];

	switch(arguments[0])
	{
		case "G_01_Cotton_Mill1":
			outArr[0] = 1;
			break;
		case "G_02_Richmond_School1":
			outArr[1] = 1;
			break;
		case "G_03_Sugar_Refinery1":
			outArr[2] = 1;
			break;
		case "G_04_XYZ1":
			outArr[3] = 1;
			break;
		case "G_07_French_Cable_Wharf1":
			outArr[4] = 1;
			break;
		case "09 Halifax Graving Dock new maya":
			outArr[5] = 1;
			break;
		case "Group1":
			outArr[6] = 1;
			break;
		case "power plant":
			outArr[7] = 1;
			break;
		case "veith house":
			outArr[8] = 1;
			break;
		case "06 Irving Shipyard and Assembly Hall with images":
			outArr[9] = 1;
			break;
		case "mulgrave park":
			outArr[10] = 1;
			break;
		default:
			break;
	}
	return(outArr);
}