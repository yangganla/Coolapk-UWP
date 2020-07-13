﻿using CoolapkUWP.Helpers;
using CoolapkUWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CoolapkUWP.Controls.DataTemplates
{
    public partial class Feed : ResourceDictionary
    {
        public Feed() => InitializeComponent();

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            if (e != null && !UIHelper.IsOriginSource(sender, e.OriginalSource)) { return; }
            if ((s.DataContext as ICanCopy)?.IsCopyEnabled ?? false) { return; }

            if (e != null) { e.Handled = true; }

            UIHelper.OpenLinkAsync(s.Tag as string);
        }

        private async void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            void DisabledCopy()
            {
                if ((sender as FrameworkElement).DataContext is ICanCopy i)
                {
                    i.IsCopyEnabled = false;
                }
            }

            FrameworkElement element = sender as FrameworkElement;
            switch (element.Name)
            {
                case "makeReplyButton":
                    var item = Microsoft.Toolkit.Uwp.UI.Extensions.VisualTree.FindAscendant<ListViewItem>(element);
                    var ctrl = item.FindName("makeFeed") as MakeFeedControl;
                    ctrl.Visibility = ctrl.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    DisabledCopy();
                    break;

                case "likeButton":
                    await DataHelper.MakeLikeAsync(element.Tag as ICanChangeLikModel,
                                                    element.Dispatcher,
                                                    (SymbolIcon)element.FindName("like1"),
                                                    (SymbolIcon)element.FindName("like2"));
                    DisabledCopy();
                    break;

                case "reportButton":
                    DisabledCopy();
                    UIHelper.Navigate(typeof(Pages.BrowserPage), new object[] { false, $"https://m.coolapk.com/mp/do?c=feed&m=report&type=feed&id={element.Tag}" });
                    break;

                default:
                    DisabledCopy();
                    UIHelper.OpenLinkAsync((sender as FrameworkElement).Tag as string);
                    break;
            }
        }

        private void ListViewItem_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (UIHelper.IsOriginSource(sender, e.OriginalSource))
            {
                if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
                {
                    OnTapped(sender, null);
                }
                else if (e.Key == Windows.System.VirtualKey.Menu)
                {
                    ListViewItem_RightTapped(sender, null);
                }
            }
        }

        private void makeFeed_MakedFeedSuccessful(object sender, System.EventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ICanChangeReplyNum m)
            {
                m.Replynum = (int.Parse(m.Replynum) + 1).ToString();
            }
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement s = (FrameworkElement)sender;
            var b = s.FindName("moreButton") as Button;
            b.Flyout.ShowAt(s);
        }

        private void relaRLis_ItemClick(object sender, ItemClickEventArgs e)
        {
            UIHelper.OpenLinkAsync(((Models.RelationRowsItem)e.ClickedItem).Url);
        }
    }
}