function main()
{
	if(arguments[1] == 0)
		return;

	var xPos;
	var yPos;
	var scale;
	var bypass = false;
	switch(arguments[0])
	{
		case "G_01_Cotton_Mill1":
			xPos = -50;
			yPos = -4.3;
			scale = 50;
			break;
		case "G_02_Richmond_School1":
			xPos = -13.7;
			yPos = 18.4;
			scale = 40.7;
			break;
		case "G_03_Sugar_Refinery1":
			xPos = -28.1;
			yPos = -37.8;
			scale = 40.7;
			break;
		case "G_04_XYZ1":
			xPos = -20.7;
			yPos = 23.6;
			scale = 27.8;
			break;
		case "G_07_French_Cable_Wharf1":
			xPos = 13.1;
			yPos = -24.5;
			scale = 35.3;
			break;
		case "09 Halifax Graving Dock new maya":
			xPos = -28.9;
			yPos = -45.9;
			scale = 52.4;
			break;
		case "Group1":
			xPos = -30;
			yPos = 4.1;
			scale = 32.6;
			break;
		case "power plant":
			xPos = -18.1;
			yPos = -23.6;
			scale = 61.1;
			break;
		case "veith house":
			xPos = -35.3;
			yPos = -30.8;
			scale = 33.2;
			break;
		case "06 Irving Shipyard and Assembly Hall with images":
			xPos = -20.7;
			yPos = -17.4;
			scale = 78.6;
			break;
		case "mulgrave park":
			xPos = -16.4;
			yPos = 3.5;
			scale = 80.3;
			break;
		default:
			bypass = true;
			break;
	}
	return[xPos,yPos,scale,bypass];

}