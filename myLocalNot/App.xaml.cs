using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace myLocalNot;

public partial class App : Application
{

	public App()
	{
		InitializeComponent();

		// Local Notification tap event listener
		LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
       

		MainPage = new MainPage();
	}

	private void OnNotificationActionTapped(NotificationActionEventArgs e)
    	{
           if (e.IsDismissed)
           {
               // your code goes here
               return;
           }
	   if (e.IsTapped)
           {
               // your code goes here
               return;
           }
           // if Notification Action are setup
           switch (e.ActionId)
           {
               // your code goes here
           }
	}
	
}
