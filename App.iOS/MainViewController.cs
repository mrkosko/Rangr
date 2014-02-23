﻿using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;
using App.Core.Portable.Device;
using App.Core.Portable.Models;
using App.Common.Shared;

namespace App.iOS
{
	public partial class MainViewController : UITableViewController
	{
		IGeoLocation _geoLocationInstance;
		ISession _sessionInstance;
		UITextView textView;
		ConsoleView consoleView;
		Global _global;
		DataSource dataSource;

		public MainViewController () : base ("MainViewController", null)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Main", "Main");

			// Custom initialization
			consoleView = new ConsoleView (){
				//Frame = View.Frame
			};
				
			textView = consoleView.Console;

			_geoLocationInstance = GeoLocation.GetInstance ();
			_sessionInstance = Session.GetInstance ();
			//_postRepository = new PostRepository (new HttpRequest ());
			_global = Global.Current;
		}

		public DetailViewController DetailViewController {
			get;
			set;
		}

		public CreatePostViewController CreatePostViewController {
			get;
			set;
		}

		public UIViewController ConsoleViewController {
			get;
			set;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void Initialize ()
		{
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Add), false);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => {
				//if (this.NewPostViewController == null)
				this.CreatePostViewController = new CreatePostViewController ();
				//this.NewPostViewController.ModalInPopover = true;

				// Pass the selected object to the new view controller.
				this.NavigationController.PushViewController (this.CreatePostViewController, true);

			};


			NavigationItem.SetLeftBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Action), false);
			NavigationItem.LeftBarButtonItem.Clicked += (object sender, EventArgs e) => {
				if(ConsoleViewController == null)
				{
					this.ConsoleViewController = new UIViewController ();
					consoleView.Frame = ConsoleViewController.View.Frame;
					this.ConsoleViewController.Add(consoleView);
				}
				// Pass the selected object to the new view controller.
				this.NavigationController.PushViewController (this.ConsoleViewController, true);

			};

			TableView.Source = dataSource = new DataSource (this);


			var traceWriter = new TextViewWriter (SynchronizationContext.Current, textView);


			_global.client = CommonClient.GetInstance (traceWriter, _geoLocationInstance, _sessionInstance, SynchronizationContext.Current);

			Action<Post> routine = (post) => {
				dataSource.Objects.Insert (0, post.Text);

				using (var indexPath = NSIndexPath.FromRowSection (0, 0))
					TableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
			};

			_global.client.OnConnectionAborted ((client) => {
				client.Start (routine);
			});

			_global.client.Start (routine);

			//			UIAlertView alert = new UIAlertView();
			//			alert.Title = "Add Something";
			//			alert.AddButton("One");
			//			alert.AddButton("Two");
			//			alert.AddButton("Three");
			//			alert.Message = "Enter something:";
			//			alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;//UIAlertViewStyle.LoginAndPasswordInput;
			//			alert.Show();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}

		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("Cell");
			readonly List<object> objects = new List<object> ();
			readonly MainViewController controller;

			public DataSource (MainViewController controller)
			{
				this.controller = controller;
			}

			public IList<object> Objects {
				get { return objects; }
			}
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return objects.Count;
			}
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (CellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}

				cell.TextLabel.Text = objects [indexPath.Row].ToString ();

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					// Delete the row from the data source.
					objects.RemoveAt (indexPath.Row);
					controller.TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				} else if (editingStyle == UITableViewCellEditingStyle.Insert) {
					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
				}
			}
			/*
			// Override to support rearranging the table view.
			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
			{
			}
			*/
			/*
			// Override to support conditional rearranging of the table view.
			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the item to be re-orderable.
				return true;
			}
			*/
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (controller.DetailViewController == null)
					controller.DetailViewController = new DetailViewController ();

				controller.DetailViewController.SetDetailItem (objects [indexPath.Row]);

				// Pass the selected object to the new view controller.
				controller.NavigationController.PushViewController (controller.DetailViewController, true);
			}
		}
	}
}
