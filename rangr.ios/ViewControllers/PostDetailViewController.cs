using System;
using UIKit;
using CoreLocation;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using App.Common;

using Google.Maps;
using ios_ui_lib;

namespace rangr.ios
{
    public class PostDetailViewController : BaseViewModelController<PostDetailsViewModel>
    {
        public override string TitleLabel {
            get { return "Detail"; }
        }

        private MapView map_view;
        private UILabel description;

        public PostDetailViewController()
        {
            view_model = new PostDetailsViewModel();
        }

        //According to http://blog.adamkemp.com/2014/11/ios-layout-gotchas-and-view-controller.html
        //Need to moveout layout code from here and move to the ViewDidLayoutSubviews
        //Never set your own Frame in a UIView or your own View.Frame in a UIViewController.
        //Do layout in LayoutSubviews in a UIView and in ViewDidLayoutSubviews in a UIViewController.
        //Don't use the screen's size for layout. Instead use Bounds in a UIView and View.Bounds in a UIViewController.
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Add(description = new UILabel());

            View.Add(map_view = LoadMap());

            ConfigureView();
        }

        public override void ViewDidLayoutSubviews()
        {

            View.ConstrainLayout(() => 
                description.Frame.Top == View.Frame.Top + parent_child_margin &&
                description.Frame.Left == View.Frame.Left + parent_child_margin &&
                description.Frame.Right == View.Frame.Right - parent_child_margin &&
                description.Frame.Height == finger_tip_diameter &&


                map_view.Frame.Top == description.Frame.Bottom + parent_child_margin &&
                map_view.Frame.Left == View.Frame.Left + parent_child_margin &&
                map_view.Frame.Right == View.Frame.Right - parent_child_margin &&
                map_view.Frame.Bottom == View.Frame.Bottom - parent_child_margin
            );
        }

        public void SetDetailItem(Post newDetailItem)
        {
            if (view_model.CurrentPost != newDetailItem)
            {
                view_model.CurrentPost = newDetailItem;
				
                // Update the view
                ConfigureView();
            }
        }

        private void ConfigureView()
        {
            // Update the user interface for the detail item
            if (IsViewLoaded && view_model.CurrentPost != null)
            {
                description.Text = view_model.CurrentPost.text;
            }
        }

        private MapView LoadMap()
        {
            var splits = view_model.CurrentPost.long_lat_acc_geo_string.Split(',');

            var camera = CameraPosition.FromCamera(latitude: double.Parse(splits[1]), longitude: double.Parse(splits[0]), zoom: 15);

            var map_View = MapView.FromCamera(CGRect.Empty, camera);
        
            map_View.MapType = MapViewType.Normal;
        
            map_View.MyLocationEnabled = false;
        
            map_View.Settings.SetAllGesturesEnabled(false);
        
            AddMarker(view_model.CurrentPost, map_View);
        
            AddCircle(view_model.CurrentPost, map_View);
        
            map_View.Animate(GetPosition(view_model.CurrentPost.long_lat_acc_geo_string));

            return map_View;
        }

        private void AddMarker(Post the_post, MapView the_map)
        {
            var marker = new Marker();
            marker.Title = the_post.text;
            marker.Position = GetPosition(the_post.long_lat_acc_geo_string);
            marker.Map = the_map;
        
            //.InvokeIcon(BitmapDescriptorFactory
            //.DefaultMarker(BitmapDescriptorFactory
            //.HueCyan)))
        }

        private void AddCircle(Post the_post, MapView the_map)
        {
            var circle = new Circle();
            circle.Position = GetPosition(the_post.long_lat_acc_geo_string);
            circle.Radius = GetAccurracy(the_post.long_lat_acc_geo_string);
            circle.StrokeWidth = 1.0f;
            circle.StrokeColor = UIColor.LightTextColor;
            circle.FillColor = UIColor.LightTextColor;
        
            circle.Map = the_map;
        }

        private CLLocationCoordinate2D GetPosition(string geo_string)
        {
            var array = geo_string.Split(',');
            return new CLLocationCoordinate2D(double.Parse(array[1]), double.Parse(array[0]));
        }

        private int GetAccurracy(string geo_string)
        {
            var array = geo_string.Split(',');
            return int.Parse(array[2]);
        }
    }
}

