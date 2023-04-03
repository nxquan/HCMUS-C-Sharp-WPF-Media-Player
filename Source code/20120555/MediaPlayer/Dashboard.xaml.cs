using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Dashboard : Window, INotifyPropertyChanged
    {
        private ObservableCollection<MediaFile> _playlist = new ObservableCollection<MediaFile>();
        private ObservableCollection<MediaFile> _recentPlaylist = new ObservableCollection<MediaFile>();

        private bool _playing = false;
        private bool _isRepeat = false;
        private bool _isShuffle = false;
        private int _currentIndexPlaying = -1;

        private bool _isHasLastMedia = false;
        private bool _isFirstTime = true;
        private MediaFile? _lastMedia;
        private double _lastPositionInSeconds;
        private double _lastTotalOfMedia = 0;
        private string _lastPositionInText = "";

        private DispatcherTimer _timer = new DispatcherTimer();
        public event PropertyChangedEventHandler? PropertyChanged;

        //Command for shourtcut
        public static RoutedCommand playCommand { get; set; } = new RoutedCommand();
        public static RoutedCommand pauseCommand { get; set; } = new RoutedCommand();

        public static RoutedCommand enterCommand { get; set; } = new RoutedCommand();
        public static RoutedCommand spaceCommand { get; set; } = new RoutedCommand();

        public static RoutedCommand nextCommand { get; set; } = new RoutedCommand();
        public static RoutedCommand previousCommand { get; set; } = new RoutedCommand();

        public Dashboard()
        {
            playCommand.InputGestures.Add(new KeyGesture(Key.Play));
            pauseCommand.InputGestures.Add(new KeyGesture(Key.Pause));
            previousCommand.InputGestures.Add(new KeyGesture(Key.Left));
            nextCommand.InputGestures.Add(new KeyGesture(Key.Right));
            enterCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            spaceCommand.InputGestures.Add(new KeyGesture(Key.Space));

            InitializeComponent();

            loadRecentPlaylistAndLastPlayedMedia();

            lvPlaylist.ItemsSource = _playlist;
            lvRecentPlaylist.ItemsSource = _recentPlaylist;
            this.DataContext = this;
        }

        //load file, load playlist and save playlist
        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            var addFileScreen = new OpenFileDialog();
            addFileScreen.Multiselect = true;

            if(addFileScreen.ShowDialog() == true)
            {
                foreach(var path in addFileScreen.FileNames)
                {
                    var info = new FileInfo(path);
                    
                    var mediFile = new MediaFile()
                    {
                        Id = _playlist.Count,
                        Title = info.Name,
                        Author = "Unknown",
                        Path = path,
                    };

                    if(path.Contains(".mp3"))
                    {
                        mediFile.Type = "mp3";
                    }
                    else if(path.Contains(".mp4"))
                    {
                        mediFile.Type = "mp4";
                    }


                    _playlist.Add(mediFile);
                }    
            }
        }

        private void savePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var saveScreen = new SaveFileDialog();
            saveScreen.Filter = "text file|*.txt";

            if (saveScreen.ShowDialog() == true)
            {
                StreamWriter sw = new StreamWriter(File.Create(saveScreen.FileName));
                int sizeOfPlaylist = _playlist.Count;
                for (int i = 0; i < sizeOfPlaylist; i++)
                {
                    if (i == sizeOfPlaylist - 1)
                    {
                        sw.Write(_playlist[i].Path);
                    }
                    else
                    {
                        sw.Write(_playlist[i].Path + "\n");
                    }
                }

                sw.Dispose();
            }
        }

        private void loadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var openScreen = new OpenFileDialog();
            openScreen.Filter = "text fill|*.txt";

            if (openScreen.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(openScreen.FileName);

                foreach (var line in lines)
                {
                    var info = new FileInfo(line);

                    var mediFile = new MediaFile()
                    {
                        Id = _playlist.Count,
                        Title = info.Name,
                        Author = "Unknown",
                        Path = line,
                    };

                    if (line.Contains(".mp3"))
                    {
                        mediFile.Type = "mp3";
                    }
                    else if (line.Contains(".mp4"))
                    {
                        mediFile.Type = "mp4";
                    }

                    _playlist.Add(mediFile);
                }
            }
        }

        private void loadRecentPlaylistAndLastPlayedMedia()
        {
            //Load last played media
            var linesLastMedia = File.ReadAllLines("./last-played-media.txt");
            if(linesLastMedia.Length >= 4)
            {
                string path = linesLastMedia[0];
                var info = new FileInfo(path);
                _lastMedia = new MediaFile()
                {
                    Path = path,
                    Title = info.Name,
                    Author = "Unknown",
                };
                if(path.Contains(".mp3"))
                {
                    _lastMedia.Type = "mp3";
                    thumbnail.Source = new BitmapImage(new Uri("/icon/mp3.png", UriKind.Relative));
                }
                else if(path.Contains(".mp4"))
                {
                    thumbnail.Source = new BitmapImage(new Uri("/icon/video.png", UriKind.Relative));
                    _lastMedia.Type = "mp4";
                }

                thumbnail.Visibility = Visibility.Visible;
                player.Visibility = Visibility.Collapsed;

                _lastPositionInSeconds = Math.Ceiling(double.Parse(linesLastMedia[1]));
                _lastPositionInText = linesLastMedia[2];

                progressBar.Maximum = double.Parse(linesLastMedia[3]);
                progressBar.Value = _lastPositionInSeconds;
                currentPos.Text = _lastPositionInText;
                _isHasLastMedia = true;
            }
            else
            {
                currentMediaName.Text = "Please choose your media";
            }

            //Load recent playlist
            var lines = File.ReadAllLines("./store-recent-playlist.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                var info = new FileInfo(lines[i]);

                var mediFile = new MediaFile()
                {
                    Id = _playlist.Count,
                    Title = info.Name,
                    Author = "Unknown",
                    Path = lines[i],
                };

                if (lines[i].Contains(".mp3"))
                {
                    mediFile.Type = "mp3";
                }
                else if (lines[i].Contains(".mp4"))
                {
                    mediFile.Type = "mp4";
                }
                _recentPlaylist.Add(mediFile);
            }
        }
       
        private void _timer_Tick(object? sender, EventArgs e)
        {
            int hours = player.Position.Hours;
            int minutes = player.Position.Minutes;
            int seconds = player.Position.Seconds;

            currentPos.Text = TimeConverter(hours, minutes, seconds);
            progressBar.Value = player.Position.TotalSeconds;
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            int hours = player.NaturalDuration.TimeSpan.Hours;
            int minutes = player.NaturalDuration.TimeSpan.Minutes;
            int seconds = player.NaturalDuration.TimeSpan.Seconds;

            currentLength.Text = TimeConverter(hours, minutes, seconds);
            if(!_isHasLastMedia)
            {
                _recentPlaylist.Add(_playlist[_currentIndexPlaying]);
            }
            _lastTotalOfMedia = player.NaturalDuration.TimeSpan.TotalSeconds;
            progressBar.Maximum = _lastTotalOfMedia;
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeat == false)
            {
                if(!_isHasLastMedia)
                {
                    if (_isShuffle)
                    {
                        _currentIndexPlaying = new Random().Next(0, _playlist.Count - 1);
                    }
                    else
                    {
                        _currentIndexPlaying = _currentIndexPlaying < _playlist.Count - 1 ? _currentIndexPlaying + 1 : 0;
                    }
                    changeUIAndPlayMedia();
                }
                else
                {
                    if (_isShuffle)
                    {
                        if(!_playlist.Contains(_lastMedia))
                        {
                            _playlist.Add(_lastMedia);
                        }
                        _currentIndexPlaying = new Random().Next(0, _playlist.Count - 1);
                        changeUIAndPlayMedia();
                    }
                }
            }else
            {
                if (_isHasLastMedia)
                {
                    if (!_playlist.Contains(_lastMedia))
                    {
                        _playlist.Add(_lastMedia);
                    }
                    _currentIndexPlaying = _playlist.Count - 1;
                }
                changeUIAndPlayMedia();
            }
           
        }

        private void changeUIAndPlayMedia()
        {
            changeDisplayMedia();
            player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
            _playing = true;
            currentMediaName.Text = _playlist[_currentIndexPlaying].Title;
            playMedia();
        }

        //Change UI when data change
        private string TimeConverter(int hours, int minutes, int seconds)
        {
            var newFormat = new StringBuilder();
            if (hours > 9)
            {
                newFormat.Append(hours);
            }
            else
            {
                newFormat.Append($"0{hours}");
            }
            newFormat.Append(":");
            if (minutes > 9)
            {
                newFormat.Append(minutes);
            }
            else
            {
                newFormat.Append($"0{minutes}");
            }
            newFormat.Append(":");
            if (seconds > 9)
            {
                newFormat.Append(seconds);
            }
            else
            {
                newFormat.Append($"0{seconds}");
            }

            return newFormat.ToString();
        }

        private void changeDisplayMedia()
        {
            if (!_isHasLastMedia)
            {
                if (_playlist[_currentIndexPlaying].Type == "mp3")
                {
                    player.Visibility = Visibility.Collapsed;
                    thumbnail.Source = new BitmapImage(new Uri("/icon/mp3.png", UriKind.Relative));
                    thumbnail.Visibility = Visibility.Visible;
                }
                else if (_playlist[_currentIndexPlaying].Type == "mp4")
                {
                    player.Visibility = Visibility.Visible;
                    thumbnail.Visibility = Visibility.Collapsed;
                }
                progressBar.Value = 0;
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += _timer_Tick;
            }
        }

        private void progressBar_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            int value = (int)progressBar.Value;
            TimeSpan newPos = TimeSpan.FromSeconds(value);
            player.Position = newPos;
        }
        //Handler for control buttons:
        private void mainControlBtn_Click(object sender, RoutedEventArgs e)
        {
            playHanlder();
        }

        private void playHanlder()
        {
            if(_isHasLastMedia)
            {
                if (_isFirstTime)
                {
                    player.Source = new Uri(_lastMedia!.Path, UriKind.Absolute);
                    player.Position = TimeSpan.FromSeconds(_lastPositionInSeconds);
                }
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += _timer_Tick;
                if(_lastMedia.Type == "mp4")
                {
                    player.Visibility = Visibility.Visible;
                    thumbnail.Visibility = Visibility.Collapsed;
                }
                
                logicPlayAndPause();
                _isFirstTime = false;
            }
            else if (_currentIndexPlaying != -1)
            {
                if (player.Source == null)
                {
                    player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
                }
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += _timer_Tick;
                logicPlayAndPause();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tập tin cần phát", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void logicPlayAndPause()
        {
            if (_playing)
            {
                pauseMedia();
            }
            else
            {
                playMedia();
            }
            _playing = !_playing;
        }

        private void playMedia()
        {
            player.Play();
            BitmapImage bitmap = new BitmapImage(new Uri("/icon/pause.png", UriKind.Relative));
            mainControlImage.Source = bitmap;
            _timer.Start();
        }

        private void pauseMedia()
        {
            player.Pause();
            BitmapImage bitmap = new BitmapImage(new Uri("/icon/play.png", UriKind.Relative));
            mainControlImage.Source = bitmap;
            _timer.Stop();
        }

        private void previousHandler()
        {
            if (_currentIndexPlaying != -1)
            {
                if (_currentIndexPlaying > 0)
                {
                    _currentIndexPlaying--;
                }
                else if (_currentIndexPlaying == 0)
                {
                    _currentIndexPlaying = _playlist.Count - 1;
                }
                changeDisplayMedia();
                player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
                _playing = true;
                currentMediaName.Text = _playlist[_currentIndexPlaying].Title;
                playMedia();
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            previousHandler();
        }

        private void nextHandler()
        {
            if (_currentIndexPlaying != -1)
            {
                if (_currentIndexPlaying < _playlist.Count - 1)
                {
                    _currentIndexPlaying++;
                }
                else if (_currentIndexPlaying == _playlist.Count - 1)
                {
                    _currentIndexPlaying = 0;
                }
                changeDisplayMedia();
                player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
                _playing = true;
                currentMediaName.Text = _playlist[_currentIndexPlaying].Title;
                playMedia();
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            nextHandler();
        }

        private void repeatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isRepeat)
            {
                BitmapImage bitmap = new BitmapImage(new Uri("/icon/repeat.png", UriKind.Relative));
                repeatImage.Source = bitmap;
            }
            else
            {
                BitmapImage bitmap = new BitmapImage(new Uri("/icon/active-repeat.png", UriKind.Relative));
                repeatImage.Source = bitmap;
            }
            _isRepeat = !_isRepeat;
        }

        private void shuffeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isShuffle)
            {
                BitmapImage bitmap = new BitmapImage(new Uri("/icon/shuffle.png", UriKind.Relative));
                shuffleImage.Source = bitmap;
            }
            else
            {
                BitmapImage bitmap = new BitmapImage(new Uri("/icon/active-shuffle.png", UriKind.Relative));
                shuffleImage.Source = bitmap;
            }
            _isShuffle = !_isShuffle;
        }

        private void lvPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isHasLastMedia = false;
            _currentIndexPlaying = lvPlaylist.SelectedIndex;
            if(_currentIndexPlaying >=0 && _currentIndexPlaying < _playlist.Count)
            {
                changeDisplayMedia();
                player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
                _playing = true;
                currentMediaName.Text = _playlist[_currentIndexPlaying].Title;
                playMedia();
            }
        }

        private void lvRecentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isHasLastMedia = false;
            int index = lvRecentPlaylist.SelectedIndex;
            if (index != -1)
            {
                MediaFile seletedItem = _recentPlaylist[index];
                if (!_playlist.Contains(seletedItem))
                {
                    _playlist.Add(seletedItem);
                    _currentIndexPlaying = _playlist.Count - 1;
                }
                else
                {
                    _currentIndexPlaying = _playlist.IndexOf(seletedItem);
                }
                changeDisplayMedia();
                player.Source = new Uri(_playlist[_currentIndexPlaying].Path, UriKind.Absolute);
                _playing = true;
                currentMediaName.Text = _playlist[_currentIndexPlaying].Title;
                playMedia();
            }

        }

        //Remove one item and remove all in playlist
        private void removeAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = MessageBox.Show("Warning", "Are you sure to remove all media files ?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(messageBox == MessageBoxResult.Yes)
            {
                pauseMedia();
                player.Source = null;
                _playing = false;
                currentMediaName.Text = "Please choose your media";
                _playlist.Clear();
            }
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            int deletedIndex = lvPlaylist.SelectedIndex;
            var messageBox = MessageBox.Show("Warning", $"Are you sure to remove {_playlist[deletedIndex].Title} ?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBox == MessageBoxResult.Yes)
            {
                pauseMedia();
                player.Source = null;
                _playing = false;
                currentMediaName.Text = "Please choose your media";
                _playlist.Remove(_playlist[deletedIndex]);
            }
        }

        //command for global shortcuts
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playHanlder();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void previousCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

        }

        private void previousCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            previousHandler();
        }

        private void nextCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void nextCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            nextHandler();
        }

        private void playCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void playCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playHanlder();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

            //Write last played media
            if(_recentPlaylist.Count > 0)
            {
                StreamWriter swLastMedia = new StreamWriter("./last-played-media.txt");
                swLastMedia.Write(_recentPlaylist[_recentPlaylist.Count - 1].Path + "\n");
                swLastMedia.Write(progressBar.Value + "\n");
                swLastMedia.Write(currentPos.Text + "\n");
                swLastMedia.Write(_lastTotalOfMedia);
                swLastMedia.Dispose();
            }

            //Write recent playlist
            StreamWriter sw = new StreamWriter("./store-recent-playlist.txt");
            for (int i = 0; i < _recentPlaylist.Count; i++)
            {
                if(i == _recentPlaylist.Count - 1)
                {
                    sw.Write(_recentPlaylist[i].Path);
                }
                else
                {
                    sw.Write(_recentPlaylist[i].Path + "\n");
                }
            }

            sw.Dispose();
        }

        private void pauseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentIndexPlaying != -1)
            {
                pauseMedia();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tập tin cần phát", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void enterCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playHanlder();
        }

        private void spaceCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playHanlder();
        }

    }
}
