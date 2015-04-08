﻿using System;
using UIKit;
using System.Drawing;
using general_shared_lib;
using CoreGraphics;

namespace ios_ui_lib
{
    public class CropperViewController : UIViewController
    {
        private string selected_image_path { get; set;}

        private UIImageView imageView;
        private CropperView cropperView;
        private UIPanGestureRecognizer pan;
        private UIPinchGestureRecognizer pinch;
        private UITapGestureRecognizer doubleTap;

        public CropperViewController (string the_selected_image_path)
        {
            selected_image_path = Guard.IsNotNull(the_selected_image_path, "the_selected_image_path");
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            using (var image = UIImage.FromFile (selected_image_path)) 
            {
                imageView = new UIImageView (new CGRect (0, 0, image.Size.Width, image.Size.Height));
                imageView.Image = image;
            }

            cropperView = new CropperView { Frame = View.Frame };
            View.AddSubviews (imageView, cropperView);

            nfloat dx = 0;
            nfloat dy = 0;

            pan = new UIPanGestureRecognizer(g=>{
                if ((g.State == UIGestureRecognizerState.Began || g.State == UIGestureRecognizerState.Changed) && (g.NumberOfTouches == 1)) {

                    var p0 = g.LocationInView (View);

                    if (dx == 0)
                        dx = p0.X - cropperView.Origin.X;

                    if (dy == 0)
                        dy = p0.Y - cropperView.Origin.Y;

                    var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

                    cropperView.Origin = p1;
                } else if (g.State == UIGestureRecognizerState.Ended) {
                    dx = 0;
                    dy = 0;
                }
            });

            nfloat s0 = 1;

            pinch = new UIPinchGestureRecognizer (g=>{
                nfloat s = g.Scale;
                nfloat ds = (nfloat)Math.Abs (s - s0);
                nfloat sf = 0;
                const float rate = 0.5f;

                if (s >= s0) {
                    sf = 1 + ds * rate;
                } else if (s < s0) {
                    sf = 1 - ds * rate;
                }
                s0 = s;

                cropperView.CropSize = new CGSize (cropperView.CropSize.Width * sf, cropperView.CropSize.Height * sf);  

                if (g.State == UIGestureRecognizerState.Ended) {
                    s0 = 1;
                } 
            });

            doubleTap = new UITapGestureRecognizer ((gesture) => {
                Crop();

            }) { 
                NumberOfTapsRequired = 2, NumberOfTouchesRequired = 1 
            };

            cropperView.AddGestureRecognizer (pan);
            cropperView.AddGestureRecognizer (pinch);
            cropperView.AddGestureRecognizer (doubleTap);   
        }

        private void Crop()
        {
            var inputCGImage = UIImage.FromFile (selected_image_path).CGImage;

            var image = inputCGImage.WithImageInRect (cropperView.CropRect);
            using (var croppedImage = UIImage.FromImage (image)) {

                imageView.Image = croppedImage;
                imageView.Frame = cropperView.CropRect;
                imageView.Center = View.Center;

                cropperView.Origin = new CGPoint (imageView.Frame.Left, imageView.Frame.Top);
                cropperView.Hidden = true;
            }
        }
    }
}

