   M S B A M L   `   `   `  ���� F  BMediaPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:MediaPlayerMediaPlayer      MediaPlayer.Dashboard   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:MediaPlayer     -     � Title$  Media Player��$	��550��$
��1000�� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      MediaPlayer.Dashboard   playCommand   Command# Z   -      pauseCommand# Z   -      enterCommand# Z   -      spaceCommand# Z   -      previousCommand# Z   -      nextCommand# Z .���� .+�]� $
��True��.+��� $
��Left=�	 �� Orientation$	 Vertical=�$��0 6 0 0q�.+��� $	 
Horizontal=�.+��� -   $��addFile��$	��120��$��2q����� $	 
Horizontal=�.+��� $��./Icon/add-file.png��$��25��$��25���� $��Browser files���� -	   $��loadPlaylist��$	��120��$��2q�$��10 0 0 0q����� $	 
Horizontal=�.+��� $��./Icon/upload.png��$��25��$��25���� $��Load playlist���� -
   $��savePlaylist��$	��120��$��2q�$��10 0 0 0q����� $	 
Horizontal=�.+��� $��./Icon/save.png��$��25��$��25���� $��Save playlist���� $��My Playlists��$
��Bold"��� -   $��
lvPlaylist��$��10q�$��Visible=�$
��True��$	��250��
 � 	Resources
    playlistMenuContext��%g   &	        �� .@�m� $��	removeBtn��-   $��Remove��m� $��removeAllBtn��-   $��
Remove all������  �� 
TargetType" ��.���  �� Property � �� �� Value
 �������  � ��$ 	0,0,0,0.5q���  � ��$ 4 8 4 8q���  �@�ContextMenu# [   �� View �� .P��� $��#�� �� Width$ 40�� �� DisplayMemberBinding �� *Id+�� $��Title��$	 260�� �� *Title+�� $��Author��$	 100�� �� *Author+�� $��Recent playlists��$
��Bold"��� -   $��lvRecentPlaylist��$��	10 0 10 0q�$��Visible=�$
��True��$	��150������ " ��.���  � ��
 �������  � ��$ 	0,0,0,0.5q���  � ��$ 4 8 4 8q� �� .P��� $��#��$ 40�� �� *Id+�� $��Title��$	 180�� �� *Title+�� $��Author��$	 260�� �� *Author+�� $��Right=�$	 Vertical=�$��0 10 0 0q�.+�r� -   $��player��$	��520��$	��300�� r� LoadedBehavior$ Manual=�$y�Visible=��� -   $��	thumbnail��$	��200��$	��200��$��./icon/mp3.png��$��	0 50 0 50q�$y�	Collapsed=��� -   $��currentMediaName��$
��Bold"� �� TextAlignment$ Center=�$��0 8 0 8q��� $	 
Horizontal=�$��Center=�.+��� -   $��	shuffeBtn��
������ 
������ $��2q� � Cursor$
 Hand������ -   $��shuffleImage��$��./icon/shuffle.png��$��30��$��30���� -   $��previousBtn��
������ 
������ $��2q�$��8 0 0 0q�$
 Hand������ $��./icon/previous.png��$��30��$��30���� -   $��mainControlBtn��
������ 
������ $��2q�$��8 0 0 0q�$
 Hand������ -   $��mainControlImage��$��./icon/play.png��$��30��$��30���� -   $��nextBtn��
������ 
������ $��2q�$��8 0 0 0q�$
 Hand������ $��./icon/next.png��$��30��$��30���� -   $��	repeatBtn��
������ 
������ $��2q�$��8 0 0 0q�$
 Hand������ -   $��repeatImage��$��./icon/repeat.png��$��30��$��30��]� $	��420��.+��� -   $��
currentPos��$��00:00:00��$
��Left=�$��0 0 12 0q��� -   $��progressBar��$	��300���� -   $��currentLength��$��00:00:00��$��Right=�$��12 0 0 0q�