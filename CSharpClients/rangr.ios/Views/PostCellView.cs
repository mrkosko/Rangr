using System;
using CoreGraphics;
using Foundation;
using UIKit;
using rangr.common;
using ios_ui_lib;

namespace rangr.ios
{
    public partial class PostCellView : UITableViewCell
    {
        private bool didSetupConstraints;
        const string PlaceholderImagePath = "user-default-avatar.png";
        public static readonly NSString Key = new NSString("PostCellView");

        private UILabel time_ago;
        private UIImageView user_image;
        private UILabel user_name;
        private TopAlignedImageView post_image;
        private UILabel post_text;
        private UIView card_view;

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            if (this.didSetupConstraints)
            {
                return;
            }

            this.user_name.SetContentCompressionResistancePriority(Layout.RequiredPriority, UILayoutConstraintAxis.Vertical);
            this.time_ago.SetContentCompressionResistancePriority(Layout.RequiredPriority, UILayoutConstraintAxis.Vertical);
            this.post_text.SetContentCompressionResistancePriority(Layout.RequiredPriority, UILayoutConstraintAxis.Vertical);


            var user_image_width = HumanInterface.medium_square_image_length;
            var user_image_height = HumanInterface.medium_square_image_length;
            var sibling_sibling_margin = HumanInterface.sibling_sibling_margin;
            var parent_child_margin = HumanInterface.parent_child_margin;

            ContentView.ConstrainLayout(() => 
                card_view.Frame.Top == ContentView.Frame.Top + sibling_sibling_margin &&
                card_view.Frame.Left == ContentView.Frame.Left + sibling_sibling_margin &&
                card_view.Frame.Right == ContentView.Frame.Right - sibling_sibling_margin &&
                card_view.Frame.Bottom == ContentView.Frame.Bottom - sibling_sibling_margin
            );

            card_view.ConstrainLayout(() => 
                user_image.Frame.Width == user_image_width &&
                user_image.Frame.Height == user_image_height &&
                user_image.Frame.Top == card_view.Frame.Top + sibling_sibling_margin &&
                user_image.Frame.Left == card_view.Frame.Left + sibling_sibling_margin &&

                user_name.Frame.Left == user_image.Frame.Right + sibling_sibling_margin &&
                user_name.Frame.Right == card_view.Frame.Right - sibling_sibling_margin &&
                user_name.Frame.Top == card_view.Frame.Top + sibling_sibling_margin &&

                time_ago.Frame.Left == user_image.Frame.Right + sibling_sibling_margin &&
                time_ago.Frame.Right == card_view.Frame.Right - sibling_sibling_margin &&
                time_ago.Frame.Top == user_name.Frame.Bottom + sibling_sibling_margin &&
                time_ago.Frame.Bottom <= user_image.Frame.Bottom &&

                post_image.Frame.Left == card_view.Frame.Left &&
                post_image.Frame.Right == card_view.Frame.Right &&
                post_image.Frame.Top == user_image.Frame.Bottom + parent_child_margin &&
                post_image.Frame.Height == post_image.Frame.Width &&

                post_text.Frame.Left == card_view.Frame.Left + sibling_sibling_margin &&
                post_text.Frame.Right == card_view.Frame.Right - sibling_sibling_margin &&
                post_text.Frame.Top == post_image.Frame.Bottom + parent_child_margin &&
                post_text.Frame.Bottom == card_view.Frame.Bottom - sibling_sibling_margin

            );

            this.didSetupConstraints = true;
        }

        public void BindDataToCell(Post post)
        {
            user_name.Text = post.user_display_name;
            post_text.Text = post.text;
            user_image.Image = UIImage.FromBundle(PlaceholderImagePath);
            time_ago.Text = TimeAgoConverter.Current.Convert(post.date);
            post_image.LoadUrl(string.Format("{0}/images/{1}",Resources.baseUrl, post.image_id));
            //post_image.LoadUrl ("https://igcdn-photos-b-a.akamaihd.net/hphotos-ak-xpf1/t51.2885-15/10665470_857323967625561_1501882457_n.jpg");
        }

        public PostCellView()
        {
            this.create_view();
        }

        public PostCellView(IntPtr handle)
            : base(handle)
        {
            this.create_view();
        }

        private void create_view()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = UIColor.LightGray;
            ContentView.BackgroundColor = UIColor.LightGray;


            ContentView.AddSubview(card_view = new UIView(){
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White
            });

            card_view.Layer.CornerRadius = 5;
            card_view.Layer.MasksToBounds = true;


            card_view.AddSubview(user_image = new UIImageView(){
                TranslatesAutoresizingMaskIntoConstraints = false
            });

            card_view.AddSubview(user_name = new UILabel(){
                TextColor = UIColor.Black,
                Font = UIFont.BoldSystemFontOfSize(17.0f),
                TextAlignment = UITextAlignment.Left,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation,
                TranslatesAutoresizingMaskIntoConstraints = false
            });

            card_view.AddSubview(time_ago = new UILabel(){
                TextColor = UIColor.Black,
                Font = UIFont.SystemFontOfSize(10.0f),
                TextAlignment = UITextAlignment.Left,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation,
                TranslatesAutoresizingMaskIntoConstraints = false
            });

            card_view.AddSubview(post_image = new TopAlignedImageView {
                ClipsToBounds = true,
            });

            card_view.AddSubview(post_text = new UILabel(){
                TextColor = UIColor.Black,
                Font = UIFont.PreferredSubheadline,
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TranslatesAutoresizingMaskIntoConstraints = false
            });


            //post_image.apply_simple_border();

        }
    }
}

