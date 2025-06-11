using Id3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;


namespace WpfApp_Tags
{
    internal class MediaCollection : NotifyPropertyChanged
    {
        private ObservableCollection<MusicItem> items  = new ObservableCollection<MusicItem>();
        public ObservableCollection<MusicItem> likedItems  = new ObservableCollection<MusicItem>();

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private MediaElement mediaElement;
        private int fileLengthInSeconds = 0;
        private int currentPositionInSeconds = 0;
        private string footer = string.Empty;
        private string artist = string.Empty;
        private MusicItem activeMusicItem;
       

        int number = 0;

        public string Footer
        {
            get { return footer; }
            set
            {
                if(footer != value)
                {
                    footer = value;
                    OnPropertyChanged(nameof(Footer));
                }
            }
        }

        public string Artist
        {
            get { return artist; }
            set
            {
                if (artist != value)
                {
                    artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }

        public ObservableCollection<MusicItem> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
         public ObservableCollection<MusicItem> Liked
        {
            get { return likedItems; }
            set
            {
                if (likedItems != value)
                {
                    likedItems = value;
                    OnPropertyChanged(nameof(Liked));
                }
            }
        }

        public int FileLengthInSeconds
        {
            get { return fileLengthInSeconds; }
            set
            {
                if (fileLengthInSeconds != value)
                {
                    fileLengthInSeconds = value;
                    OnPropertyChanged(nameof(FileLengthInSeconds));
                }
            }
        }

        public int CurrentPositionInSeconds
        {
            get { return currentPositionInSeconds; }
            set
            {
                if (currentPositionInSeconds != value)
                {
                    currentPositionInSeconds = value;
                    OnPropertyChanged(nameof(CurrentPositionFormatted));
                    OnPropertyChanged(nameof(CurrentPositionInSeconds));
                }
            }
        }

        public MusicItem ActiveMusicItem
        {
            get { return activeMusicItem; }
            set
            {
                if (activeMusicItem != value)
                {
                    activeMusicItem = value;
                    OnPropertyChanged(nameof(ActiveMusicItem));
                }
            }
        }

        public ICommand PlayListItemCommand { get; set; }
        public ICommand PauseListItemCommand { get; set; }
        public ICommand RemoveListItemCommand { get; set; }

        public ICommand StopCommand { get; set; }
        public ICommand OpenFilesCommand { get; set; }
        public ICommand AddFilesCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand PlayListItemPanelCommand { get; set; }
        public ICommand PauseListItemPanelCommand { get; set; }
        public ICommand NextItemPanelCommand { get; set; }
        public ICommand PreviousItemPanelCommand { get; set; }
        public ICommand ShuffleItemPanelCommand { get; set; }
        public ICommand LikedListItemCommand { get; set; }
        public ICommand DeleteFromLikedListItem { get; set; }

        public MediaCollection()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            PlayListItemCommand = new RelayCommand<MusicItem>(PlayListItem);
            PauseListItemCommand = new RelayCommand<MusicItem>(PauseListItem);
            RemoveListItemCommand = new RelayCommand<MusicItem>(RemoveListItem);
            LikedListItemCommand = new RelayCommand<MusicItem>(LikedItems);
            DeleteFromLikedListItem = new RelayCommand<MusicItem>(DeleteFromLiked);
            OpenFilesCommand = new RelayCommand(OpenFiles);
            AddFilesCommand = new RelayCommand(AddFiles);
            PlayListItemPanelCommand = new RelayCommand(PlayListItem_Panel);
            PauseListItemPanelCommand = new RelayCommand(PauseListItem_Panel);
            NextItemPanelCommand = new RelayCommand(NextItem);
            PreviousItemPanelCommand = new RelayCommand(PreviousItem);
            ShuffleItemPanelCommand = new RelayCommand(ShuffleItem);
          
        }
        public string CurrentPositionFormatted
        {
            get
            {
                int minutes = currentPositionInSeconds / 60;
                int seconds = currentPositionInSeconds % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentPositionInSeconds = (int)mediaElement.Position.TotalSeconds;
        }

        public void SetMediaElement(MediaElement mediaElement)
        {
            this.mediaElement = mediaElement;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            FileLengthInSeconds = (int)mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            
        }
        
        public void SelectCurrentPlayingItem()
        {
            ActiveMusicItem = Items.FirstOrDefault(i => i.IsPlaying);
        }
        private void Renumber(ObservableCollection<MusicItem> collection)
        {
            int number = 1;
            foreach (var item in collection)
            {
                item.Number = number++;
            }
        }

        private void PlayListItem(MusicItem musicItem)
        {
            foreach(MusicItem item in Items)
            {
                item.IsPlaying = false;
            }
            musicItem.IsPlaying = true;

            mediaElement.Source = new Uri(musicItem.FilePath);
            mediaElement.Play();
            dispatcherTimer.Start();

            SelectCurrentPlayingItem();
        }

        private void PlayListItem_Panel()
        {
            if (ActiveMusicItem != null)
                PlayListItem(ActiveMusicItem);
            else if (Items.Count > 0)
                PlayListItem(Items[0]);
            OnPropertyChanged(nameof(ActiveMusicItem));
        }
        private void PauseListItem(MusicItem musicItem)
        {
            musicItem.IsPlaying = false;

            mediaElement.Pause();
            dispatcherTimer.Stop();
        }

        private void PauseListItem_Panel()
        {
            if (ActiveMusicItem != null && ActiveMusicItem.IsPlaying)
                PauseListItem(ActiveMusicItem);
            OnPropertyChanged(nameof(ActiveMusicItem));
        }
        private void StopListItem(MusicItem musicItem)
        {
            musicItem.IsPlaying = false;

            mediaElement.Stop();
            dispatcherTimer.Stop();
        }

        private void RemoveListItem(MusicItem musicItem)
        {
            if (musicItem.IsPlaying)
            {
                StopListItem(musicItem);
            }
            Items.Remove(musicItem);
            Liked.Remove(musicItem);
            
            Renumber(Items);
            Renumber(Liked);
          

        }
        private void LikedItems(MusicItem musicItem)
        {
            if (!likedItems.Contains(musicItem))
            {
                musicItem.IsLiked = true;
                likedItems.Add(musicItem);

                Renumber(Liked);
               
            }
        }

        private void DeleteFromLiked(MusicItem musicItem)
        {
            if (Liked.Contains(musicItem))
            {
                musicItem.IsLiked = false;
                Liked.Remove(musicItem);

                Renumber(Liked);
            }
        }

        private void ShuffleItem()
        {
            Random random = new Random();
            int randomIndexMusic = random.Next(Items.Count);
            PlayListItem(Items[randomIndexMusic]);

        }
        private void ClearItems()
        {
            Items.Clear();
        } 
        private void NextItem()
        {
            if (Items.Count == 0) return;
            int currentItem = Items.IndexOf(ActiveMusicItem);
            int nextItem = currentItem + 1;

            if (nextItem >= Items.Count) nextItem = 0;
            PlayListItem(Items[nextItem]);
        }
        private void PreviousItem()
        {
            if (Items.Count == 0) return;
            int curremtItem = Items.IndexOf(ActiveMusicItem);
            int previousItem = curremtItem - 1;

            if (previousItem < 0) previousItem = Items.Count - 1;
            PlayListItem(Items[previousItem]);
        }
      
        private void OpenFiles()
        {
            this.ClearItems();      
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Music"; // Default file name
            dialog.DefaultExt = ".mp3"; // Default file extension
            dialog.Filter = "Music files (.mp3)|*.mp3"; // Filter files by extension
            dialog.Multiselect = true;

            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            
            if (result == true)
            {
                List<string> files = dialog.FileNames.ToList();
                CreateMusicItems(files);
                ExtractID3Tags();
            }
        }
        private void AddFiles()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Music"; // Default file name
            dialog.DefaultExt = ".mp3"; // Default file extension
            dialog.Filter = "Music files (.mp3)|*.mp3"; // Filter files by extension
            dialog.Multiselect = true;

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                List<string> files = dialog.FileNames.ToList();
                CreateMusicItems(files);
                ExtractID3Tags();
            }
        }

        private void CreateMusicItems(List<string> filesPath)
        {
            number = Items.Count + 1;
            foreach(string path in filesPath)
            {
                Items.Add(new MusicItem() { FilePath = path, Number = number++ });
            }
        }

        private void ExtractID3Tags()
        {
            string unknownTag = "Unknown";
           
            foreach (MusicItem musicItem in Items)
            {
                try
                {
                    var file = TagLib.File.Create(musicItem.FilePath);
                    var pictures = file.Tag.Pictures;
 
                    musicItem.Title = !string.IsNullOrWhiteSpace(file.Tag.Title) ? file.Tag.Title : unknownTag;

                    musicItem.Artist = (file.Tag.Performers != null && file.Tag.Performers.Length > 0 && !string.IsNullOrWhiteSpace(file.Tag.Performers[0]))
                        ? file.Tag.Performers[0]
                        : unknownTag;

                    musicItem.Album = !string.IsNullOrWhiteSpace(file.Tag.Album) ? file.Tag.Album : unknownTag;

                    musicItem.Genre = (file.Tag.Genres != null && file.Tag.Genres.Length > 0 && !string.IsNullOrWhiteSpace(file.Tag.Genres[0]))
                        ? file.Tag.Genres[0]
                        : unknownTag;

                    musicItem.Time = file.Properties?.Duration ?? TimeSpan.Zero;

                    if (file.Tag.Pictures.Length > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                var bin = file.Tag.Pictures[0].Data.Data;
                                using (var stream = new MemoryStream(bin))
                                {
                                    var image = new BitmapImage();
                                    image.BeginInit();
                                    image.StreamSource = stream;
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.EndInit();
                                    image.Freeze(); 
                                    musicItem.AlbumArt = image;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error loading image: {ex.Message}");
                                musicItem.AlbumArt = LoadDefaultAlbumArt();
                            }
                        });
                    }
                    else
                    {
                        musicItem.AlbumArt = LoadDefaultAlbumArt();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing file: {ex.Message}");
                    musicItem.Title = unknownTag;
                    musicItem.Artist = unknownTag;
                    musicItem.Album = unknownTag;
                    musicItem.Genre = unknownTag;
                    musicItem.Time = TimeSpan.Zero;
                    musicItem.AlbumArt = LoadDefaultAlbumArt();
                }
            }
        }

        private BitmapImage LoadDefaultAlbumArt()
        {
            var defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.UriSource = new Uri("pack://application:,,,/Images/default_cover.jpg");
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            defaultImage.Freeze();
            return defaultImage;
        }


    }
}
