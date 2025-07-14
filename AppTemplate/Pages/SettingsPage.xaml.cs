using PlutoFramework.Templates.PageTemplate;

namespace AppTemplate.Pages;

public partial class SettingsPage : PageTemplate
{
	public SettingsPage()
	{
		InitializeComponent();

		BindingContext = new SettingsPageViewModel();
    }
}