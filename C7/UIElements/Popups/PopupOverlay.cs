using Godot;
using ConvertCiv3Media;

public class PopupOverlay : HBoxContainer
{
	[Signal] public delegate void UnitDisbanded();
	Control currentChild = null;
	
	const int HTILE_SIZE = 61;
	const int VTILE_SIZE = 44;

	public enum PopupCategory {
		Advisor,
		Console,
		Info
	}
	
	private void HidePopup()
	{
		this.RemoveChild(currentChild);
		this.Hide();
	}

	public void PlaySound(AudioStreamSample wav)
	{
		AudioStreamPlayer player = GetNode<AudioStreamPlayer>("PopupSound");
		player.Stream = wav;
		player.Play();
	}

	public void ShowPopup(string dialogType, PopupCategory popupCategory)
	{
		if (dialogType.Equals("disband")) {
			DisbandConfirmation dbc = new DisbandConfirmation();
			AddChild(dbc);
			currentChild = dbc;
		}
		
		if (currentChild != null) {
			string soundFile = "";
			switch(popupCategory) {
				case PopupCategory.Advisor:
					soundFile = "Sounds/PopupAdvisor.wav";
					break;
				case PopupCategory.Console:
					soundFile = "Sounds/PopupConsole.wav";
					break;
				case PopupCategory.Info:
					soundFile = "Sounds/PopupInfo.wav";
					break;
				default:
					GD.PrintErr("Invalid popup category");
					break;
			}
			AudioStreamSample wav = Util.LoadWAVFromDisk(Util.Civ3MediaPath(soundFile));
			this.Visible = true;
			PlaySound(wav);
		}
		else {
			GD.PrintErr("Received request to show invalid dialog type " + dialogType);
		}
	}

	public static TextureRect GetPopupBackground(int width, int height)
	{		
		Image image = new Image();
		image.Create(width, height, false, Image.Format.Rgba8);

		Pcx popupborders = new Pcx(Util.Civ3MediaPath("Art/popupborders.pcx"));

		//The pop-up part is the tricky part
		Image topLeftPopup      = PCXToGodot.getImageFromPCX(popupborders, 251, 1, 61, 44);
		Image topCenterPopup    = PCXToGodot.getImageFromPCX(popupborders, 313, 1, 61, 44);
		Image topRightPopup     = PCXToGodot.getImageFromPCX(popupborders, 375, 1, 61, 44);
		Image middleLeftPopup   = PCXToGodot.getImageFromPCX(popupborders, 251, 46, 61, 44);
		Image middleCenterPopup = PCXToGodot.getImageFromPCX(popupborders, 313, 46, 61, 44);
		Image middleRightPopup  = PCXToGodot.getImageFromPCX(popupborders, 375, 46, 61, 44);
		Image bottomLeftPopup   = PCXToGodot.getImageFromPCX(popupborders, 251, 91, 61, 44);
		Image bottomCenterPopup = PCXToGodot.getImageFromPCX(popupborders, 313, 91, 61, 44);
		Image bottomRightPopup  = PCXToGodot.getImageFromPCX(popupborders, 375, 91, 61, 44);

		//Dimensions are 530x320.  The leaderhead takes up 110.  So the popup is 530x210.
		//We have multiples of... 62? For the horizontal dimension, 45 for vertical.
		//45 does not fit into 210.  90, 135, 180, 215.  Well, 215 is sorta closeish.
		//62, we got 62, 124, 248, 496, 558.  Doesn't match up at all.
		//Which means that partial textures can be used.  Lovely.

		//Let's try adding some helper functions so this can be refactored later into a more general-purpose popup popper
		int vOffset = 0;
		drawRowOfPopup(image, vOffset, width, topLeftPopup, topCenterPopup, topRightPopup);
		vOffset+=VTILE_SIZE;
		for (;vOffset < height - VTILE_SIZE; vOffset += VTILE_SIZE) {
			drawRowOfPopup(image, vOffset, width, middleLeftPopup, middleCenterPopup, middleRightPopup);
		}
		vOffset = height - VTILE_SIZE;
		drawRowOfPopup(image, vOffset, width, bottomLeftPopup, bottomCenterPopup, bottomRightPopup);

		ImageTexture texture = new ImageTexture();
		texture.CreateFromImage(image);

		TextureRect rect = new TextureRect();
		rect.Texture = texture;

		return rect;
	}
	

	private static void drawRowOfPopup(Image image, int vOffset, int width, Image left, Image center, Image right)
	{

		image.BlitRect(left, new Rect2(new Vector2(0, 0), new Vector2(left.GetWidth(), left.GetHeight())), new Vector2(0, vOffset));

		int leftOffset = HTILE_SIZE;
		for (;leftOffset < width - HTILE_SIZE; leftOffset += HTILE_SIZE)
		{
			image.BlitRect(center, new Rect2(new Vector2(0, 0), new Vector2(center.GetWidth(), center.GetHeight())), new Vector2(leftOffset, vOffset));
		}

		leftOffset = width - HTILE_SIZE;
		image.BlitRect(right, new Rect2(new Vector2(0, 0), new Vector2(right.GetWidth(), right.GetHeight())), new Vector2(leftOffset, vOffset));
	}
	
}


