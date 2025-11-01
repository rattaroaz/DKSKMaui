namespace DKSKMaui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) 
		{ 
			Title = "DKSK Official - Painting Contractor",
			Width = 1200,
			Height = 800
		};
	}
}
