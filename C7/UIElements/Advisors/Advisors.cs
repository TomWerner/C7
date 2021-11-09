using Godot;
using System;

/**
 * Handles managing the advisor screens.
 * Showing them, hiding them... maybe some other things eventually.
 * This is part of the effort to de-centralize from Game.cs and be more event driven.
 */
public class Advisors : CenterContainer
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Center the advisor container.  Following directions at https://docs.godotengine.org/en/stable/tutorials/gui/size_and_anchors.html?highlight=anchor
		//Also taking advantage of it being 1024x768, as the directions didn't really work.  This is not 100% ideal (would be great for a general-purpose solution to work),
		//but does work with the current graphics independent of resolution.
		this.MarginLeft = -512;
		this.MarginRight = -512;
		this.MarginTop = -384;
		this.MarginBottom = 384;
		this.Hide();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


	private void ShowLatestAdvisor()
	{
		GD.Print("Received request to show latest advisor");

		if (this.GetChildCount() == 0) {
			DomesticAdvisor advisor = new DomesticAdvisor();
			AddChild(advisor);
		}
		this.Show();
	}
	
	
	private void _on_Advisor_hide()
	{
		this.Hide();
	}
	
	private void OnShowSpecificAdvisor(string advisorType)
	{
		if (advisorType.Equals("F1")) {
			if (this.GetChildCount() == 0) {
				DomesticAdvisor advisor = new DomesticAdvisor();
				AddChild(advisor);
			}
			this.Show();
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (this.Visible) {
			if (@event is InputEventKey eventKey)
			{
				if (eventKey.Pressed)
				{
					if (eventKey.Scancode == (int)Godot.KeyList.Space || eventKey.Scancode == (int)Godot.KeyList.Enter)
					{
						GD.Print("Advisor received a space/enter event; stopping propagation.");
						GetTree().SetInputAsHandled();
					}
				}
			}
		}
	}
}
