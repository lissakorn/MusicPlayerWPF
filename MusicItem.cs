using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp_Tags
{
    internal class MusicItem:NotifyPropertyChanged
    {
        private bool isPlaying = false;

        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public TimeSpan Time { get; set; }
        public string Genre { get; set; }
        public bool IsLiked { get; set; }
        private BitmapImage albumArt;
        private int number;

        public int Number
        {
            get => number;
            set
            {
                if (number != value)
                {
                    number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }
        public BitmapImage AlbumArt
        {
            get { return albumArt; }
            set
            {
                if (albumArt != value)
                {
                    albumArt = value;
                    OnPropertyChanged(nameof(AlbumArt));
                }
            }
        }
        public MusicItem()
        {

        }
        public MusicItem(string filePath,  string title, string artist, string album, int number, string genre, BitmapImage albumArt, bool isLiked)
        {
            FilePath = filePath;
            Title = title;
            Artist = artist;
            Album = album;
            Number = number;
            Genre = genre;
            AlbumArt = albumArt;
            IsLiked = isLiked;
        }

    }
}
