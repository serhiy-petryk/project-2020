using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DGUI.Common;

namespace DGUI.Mwi
{
    public class WindowTabViewModel: INotifyPropertyChanged
    {
        private const double MAX_THUMBNAIL_SIZE = 180;

        public string Title => MwiChild.Title;
        public bool IsSelected => MwiChild.Container.ActiveMwiChild == MwiChild;

        public ImageSource Icon => MwiChild.Icon;
        public Image Image => new Image {Source = Icon};

        public ImageSource Thumbnail => MwiChild.Thumbnail;
        public double ThumbnailWidth => GetThumbnailSize().X;
        public double ThumbnailHeight => GetThumbnailSize().Y;
        public RelayCommand CmdClose { get; }
        public readonly MwiChild MwiChild;

        public WindowTabViewModel(MwiChild o)
        {
            MwiChild = o;
            CmdClose = new RelayCommand(CloseTab);
        }

        public void Activate()
        {
            MwiChild.Container.ActiveMwiChild = MwiChild;

            if (MwiChild.WindowState == WindowState.Minimized || MwiChild.DetachedHost?.WindowState == WindowState.Minimized)
            {
                MwiChild.ToggleMinimize(null);
                MwiChild.Focused = true;
            }
            else if (MwiChild.IsWindowed)
            {
                if (MwiChild.Focused)
                    MwiChild.Focused = false;
                MwiChild.Focused = true;
            }
        }

        public void RefreshThumbnail() =>
            OnPropertyChanged(new[] {nameof(Thumbnail), nameof(ThumbnailWidth), nameof(ThumbnailHeight)});

        public override string ToString() => MwiChild.Title;

        private void CloseTab(object p) => MwiChild.Close();

        private Point GetThumbnailSize()
        {
            var width = Thumbnail?.Width ?? 0;
            var height = Thumbnail?.Height ?? 0;
            var maxSize = Math.Max(width, height);
            var factor = maxSize > MAX_THUMBNAIL_SIZE ? MAX_THUMBNAIL_SIZE / maxSize : 1;
            return new Point(width * factor, height * factor);
        }

        //===========  INotifyPropertyChanged  =======================
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string[] propertyNames) => propertyNames.AsParallel().ForAll(propertyName =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

        #endregion
    }
}
