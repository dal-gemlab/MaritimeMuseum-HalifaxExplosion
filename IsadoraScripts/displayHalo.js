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
			yPos = 0;
			scale = 25.3;
			break;
		case "G_02_Richmond_School1":
			xPos = -13.8;
			yPos = 18.4;
			scale = 14.4;
			break;
		case "G_03_Sugar_Refinery1":
			xPos = -26.8;
			yPos = -34.1;
			scale = 17.9;
			break;
		case "G_04_XYZ1":
			xPos = -22.9;
			yPos = 32.4
			scale = 10;
			break;
		case "G_07_French_Cable_Wharf1":
			xPos = 12.2;
			yPos = -25.3;
			scale = 14;
			break;
		case "09 Halifax Graving Dock new maya":
			xPos = -27.7;
			yPos = -45.8;
			scale = 27.9;
			break;
		case "Group1":
			xPos = -30.2;
			yPos = 4.3;
			scale = 11.2;
			break;
		case "power plant":
			xPos = -18.3;
			yPos = -23.1;
			scale = 22.2;
			break;
		case "veith house":
			xPos = -37.6;
			yPos = -23.6;
			scale = 10;
			break;
		case "06 Irving Shipyard and Assembly Hall with images":
			xPos = -19.4;
			yPos = -16.9;
			scale = 34.8;
			break;
		case "mulgrave park":
			xPos = -16.5;
			yPos = 3.5;
			scale = 36.8;
			break;
		default:
			bypass = true;
			break;
	}
	return[xPos,yPos,scale,bypass];

}